using System;
using FossTech.Data;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace FossTech
{
    public class ThemeViewLocationExpander : IViewLocationExpander
    {
        
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            string currentTheme = context.ActionContext.HttpContext.Items["CurrentTheme"]?.ToString();

            // Check if the current request is for an area
            var area = context.ActionContext.RouteData.Values["area"]?.ToString();
            var isAreaRequest = !string.IsNullOrEmpty(area);

            if (!string.IsNullOrEmpty(currentTheme) && !isAreaRequest)
            {
                // Prepend the theme-specific folder to the view search locations.
                viewLocations = new string[] { $"/Views/{currentTheme}/{{1}}/{{0}}.cshtml" }
                    .Concat(viewLocations);

                // Prepend the theme-specific shared folder for layouts.
                viewLocations = new string[] { $"/Views/{currentTheme}/Shared/{{0}}.cshtml" }
                    .Concat(viewLocations);
            }

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var _memoryCache = context.ActionContext.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

            // Try to get the theme from the cache
            if (!_memoryCache.TryGetValue("CurrentTheme", out string currentTheme))
            {
                // If the theme is not found in the cache, retrieve it from the database
                var dbContext = context.ActionContext.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                var theme = dbContext.Themes.FirstOrDefault(x => x.Status);

                // Set the current theme in the context.Values dictionary.
                currentTheme = theme?.Name ?? "Oxford Theme";

                // Cache the theme for future requests
                _memoryCache.Set("CurrentTheme", currentTheme, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10), // Cache duration (e.g., 10 minutes)
                });
            }

            // Set the current theme in the context.Values dictionary.
            context.Values["CurrentTheme"] = currentTheme.ToLower();
            
        }
    }

}
