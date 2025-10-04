#!/usr/bin/env bash
set -euo pipefail

# Variables
APP_USER="www-data"
APP_GROUP="www-data"
MSSQL_CONTAINER_NAME="mssql_express"
MSSQL_PORT=1433

echo "[Setup] Updating apt and installing prerequisites..."
export DEBIAN_FRONTEND=noninteractive
apt-get update -y
apt-get install -y curl ca-certificates gnupg lsb-release apt-transport-https software-properties-common ufw nginx

echo "[Setup] Install Docker if missing..."
if ! command -v docker >/dev/null 2>&1; then
  curl -fsSL https://get.docker.com | sh
  systemctl enable docker
  systemctl start docker
fi

echo "[Setup] Allow MSSQL port on UFW and ensure SSH/HTTP/HTTPS open..."
ufw allow 22/tcp || true
ufw allow 80/tcp || true
ufw allow 443/tcp || true
ufw allow ${MSSQL_PORT}/tcp || true

echo "[Setup] Pull MSSQL Server 2022 image (Express equivalent configuration)..."
docker pull mcr.microsoft.com/mssql/server:2022-latest

echo "[Setup] Ensure docker starts on boot"
systemctl enable docker

echo "[Setup] Ensure Nginx starts on boot and is running"
systemctl enable nginx
systemctl restart nginx

echo "[Setup] Create data directory for MSSQL (owned by SQL Server user 10001)"
mkdir -p /var/opt/mssql
# SQL Server 2022 runs as non-root user with UID 10001 by default
chown -R 10001:0 /var/opt/mssql || true
chmod -R 770 /var/opt/mssql || true

echo "[Setup] Install .NET runtime if missing (ASP.NET Core Runtime)"
if ! dotnet --info >/dev/null 2>&1; then
  # Install Microsoft package repo
  wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
  dpkg -i packages-microsoft-prod.deb
  rm -f packages-microsoft-prod.deb
  apt-get update -y
  # Install ASP.NET Core Runtime 9.0
  apt-get install -y aspnetcore-runtime-9.0
fi

echo "[Setup] Finished setup."


