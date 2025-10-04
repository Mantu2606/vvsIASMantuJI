using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.Linq;
using FossTech.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Caching.Memory;

namespace FossTech.Middlewares
{
    public class ThemeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;

        public ThemeMiddleware(RequestDelegate next, IMemoryCache memoryCache)
        {
            _next = next;
            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            string userTheme = null;

            // Try to get the theme from the cache
            if (!_memoryCache.TryGetValue("CurrentTheme", out userTheme))
            {
                // If the theme is not found in the cache, retrieve it from the database
                var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                var theme = await dbContext.Themes.Where(x => x.Status).FirstOrDefaultAsync();

                // Set the current theme in the cache with a cache duration (e.g., 10 minutes)
                userTheme = theme?.Name ?? "Oxford Theme";
                _memoryCache.Set("CurrentTheme", userTheme, TimeSpan.FromMinutes(10));
            }

            // Set the theme in the response cookie for future requests
            context.Response.Cookies.Append("theme", userTheme, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true
            });

            // Set the current theme in the HttpContext.Items
            context.Items["CurrentTheme"] = userTheme.ToLower();
            await _next(context);
        }
    }
}
