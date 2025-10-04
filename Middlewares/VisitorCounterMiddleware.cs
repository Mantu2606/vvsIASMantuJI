using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.Middlewares
{
    public class VisitorCounterMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public VisitorCounterMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            string remoteIpAddress = context.Connection.RemoteIpAddress.MapToIPv4().ToString();

            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                remoteIpAddress = context.Request.Headers["X-Forwarded-For"];
            }
            var routeData = context.GetRouteData();

            if (routeData != null)
            {
                var areaName = routeData.Values["area"] as string;
                var controllerName = routeData.Values["controller"] as string;
                var actionName = routeData.Values["action"] as string;
                var fullUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
                if (controllerName != null && actionName != null && areaName == null)
                {
                    Console.WriteLine($"Controller: {controllerName}, Action: {actionName}, Area: {areaName}, Full URL: {fullUrl}");
                        dbContext.Add(new Visitor
                        {
                            ClientIp = remoteIpAddress,
                            Controller = controllerName,
                            Action = actionName,
                            FullURL = fullUrl,
                            CreatedAt = DateTime.Now,
                            LastModified = DateTime.Now,
                        });
                    dbContext.SaveChanges();
                }
            }
            await _requestDelegate(context);
        }
    }
}
