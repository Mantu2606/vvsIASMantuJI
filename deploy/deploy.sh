#!/usr/bin/env bash
set -euo pipefail

# Required env vars from caller (GitHub Action via ssh):
# - MSSQL_SA_PASSWORD
# - APP_PORT (default 8080)
# - APP_ENV (e.g., Production)
# - APP_DOMAIN (optional; e.g., dev.vvsias.com)
# - CERTBOT_EMAIL (optional; for Let's Encrypt)

APP_PORT=${APP_PORT:-8080}
APP_ENV=${APP_ENV:-Production}
APP_DOMAIN=${APP_DOMAIN:-}
CERTBOT_EMAIL=${CERTBOT_EMAIL:-kishorgujar95@gmail.com}
APP_NAME="vvsias"
APP_USER="www-data"
APP_GROUP="www-data"
APP_ROOT="/var/www/${APP_NAME}"
SERVICE_NAME="${APP_NAME}.service"
MSSQL_CONTAINER_NAME="mssql_express"
MSSQL_PORT=1433

if [[ -z "${MSSQL_SA_PASSWORD:-}" ]]; then
  echo "MSSQL_SA_PASSWORD not set" >&2
  exit 1
fi

echo "[Deploy] Ensure application directories exist"
mkdir -p ${APP_ROOT}/current
mkdir -p ${APP_ROOT}/shared

echo "[Deploy] Load app bundle to current/"
rm -rf ${APP_ROOT}/current/*
tar -xzf /tmp/app.tar.gz -C ${APP_ROOT}/current
chown -R ${APP_USER}:${APP_GROUP} ${APP_ROOT}

echo "[Deploy] Start or recreate MSSQL container (publicly accessible)"
docker rm -f ${MSSQL_CONTAINER_NAME} 2>/dev/null || true
docker run -d --name ${MSSQL_CONTAINER_NAME} \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_PID=Express" \
  -e "MSSQL_SA_PASSWORD=${MSSQL_SA_PASSWORD}" \
  -u 10001:0 \
  -p ${MSSQL_PORT}:1433 \
  -v /var/opt/mssql:/var/opt/mssql \
  --restart unless-stopped \
  mcr.microsoft.com/mssql/server:2022-latest

echo "[Deploy] Create systemd service for the app"
cat >/etc/systemd/system/${SERVICE_NAME} <<SERVICE
[Unit]
Description=ASP.NET Core App (${APP_NAME})
After=network.target docker.service
Wants=network-online.target

[Service]
WorkingDirectory=${APP_ROOT}/current
ExecStart=/usr/bin/dotnet ${APP_ROOT}/current/vvsias.com.dll --urls=http://0.0.0.0:${APP_PORT}
Restart=always
RestartSec=5
SyslogIdentifier=${APP_NAME}
Environment=ASPNETCORE_ENVIRONMENT=${APP_ENV}
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
User=${APP_USER}
Group=${APP_GROUP}

[Install]
WantedBy=multi-user.target
SERVICE

echo "[Deploy] Reload systemd and start service"
systemctl daemon-reload
systemctl enable ${SERVICE_NAME}
systemctl restart ${SERVICE_NAME}

echo "[Deploy] Open firewall for app port ${APP_PORT}"
ufw allow ${APP_PORT}/tcp || true

if [[ -n "${APP_DOMAIN}" ]]; then
  echo "[Deploy] Configure Nginx for domain ${APP_DOMAIN} -> 127.0.0.1:${APP_PORT}"
  cat >/etc/nginx/sites-available/${APP_DOMAIN} <<NGINX
server {
    listen 80;
    listen [::]:80;
    server_name ${APP_DOMAIN};

    location / {
        proxy_pass http://127.0.0.1:${APP_PORT};
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection $connection_upgrade;
    }
}
NGINX

  ln -sf /etc/nginx/sites-available/${APP_DOMAIN} /etc/nginx/sites-enabled/${APP_DOMAIN}
  nginx -t
  systemctl reload nginx

  if [[ -n "${CERTBOT_EMAIL}" ]]; then
    echo "[Deploy] Obtain/renew TLS certificate via Certbot for ${APP_DOMAIN}"
    if ! command -v certbot >/dev/null 2>&1; then
      apt-get update -y
      apt-get install -y certbot python3-certbot-nginx
    fi
    certbot --nginx -d ${APP_DOMAIN} --redirect --agree-tos -m ${CERTBOT_EMAIL} --non-interactive || true
  fi
fi

echo "[Deploy] Done. App should be reachable on port ${APP_PORT} and via domain ${APP_DOMAIN:-<no-domain>}. MSSQL on ${MSSQL_PORT}."


