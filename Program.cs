using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;
using FossTech.Helpers;
using FossTech.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using DinkToPdf.Contracts;
using DinkToPdf;
using AspNetCore.SEOHelper;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using FossTech.Middlewares;
using FossTech;
using Microsoft.AspNetCore.Mvc.Razor;
using Stripe;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.Google;
using FossTech.Configurations;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "FossTech";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});

builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.IOTimeout = TimeSpan.FromMinutes(60);
});

builder.Services.AddControllersWithViews().
    AddNewtonsoftJson(x =>
    {
        x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

builder.Services.AddSingleton<IRazorPayConfiguration>(builder.Configuration.GetSection("RazorPayConfiguration")
            .Get<RazorPayConfiguration>());

var mvcBuilder = builder.Services.AddRazorPages();

#if DEBUG
mvcBuilder.AddRazorRuntimeCompilation();
#endif

builder.Services.AddSingleton<IEmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = new PathString("/Account/Login/");
})

.AddGoogle("Google", options =>
{
    options.ClientId = "964201490595-8bkr1krccc4b2ib7m79fuo4lj36lknrh.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.Scope.Add("profile");
    options.Events.OnCreatingTicket = (context) =>
    {
        var picture = context.User.GetProperty("picture").GetString();
        context.Identity.AddClaim(new Claim("picture", picture));
        return Task.CompletedTask;
    };
});

builder.Services.AddSingleton<IMySmsShopConfiguration>(builder.Configuration.GetSection("MySmsShopConfiguration")
              .Get<MySmsShopConfiguration>());

builder.Services.AddSingleton<IPhonePeConfiguration>(builder.Configuration.GetSection("PhonePeConfiguration")
    .Get<PhonePeConfiguration>());
builder.Services.AddHttpClient<PhonePeService>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<ITemplateHelper, TemplateHelper>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddMemoryCache();
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationExpanders.Clear();
    options.ViewLocationExpanders.Add(new ThemeViewLocationExpander());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage(); 
}

SeedData.InitializeAsync(app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);

app.UseHttpsRedirection();

app.UseMiddleware<ThemeMiddleware>();

app.UseStaticFiles(new StaticFileOptions()
{
    HttpsCompression = Microsoft.AspNetCore.Http.Features.HttpsCompressionMode.Compress,
    OnPrepareResponse = (context) =>
    {
        var headers = context.Context.Response.GetTypedHeaders();
        headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(30)
        };
    }
});

app.UseXMLSitemap(app.Environment.ContentRootPath);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware(typeof(VisitorCounterMiddleware));

app.UseSession();

app.MapControllerRoute(
    name: "Dashboard",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "FossTech",
    pattern: "{area:exists}/{controller=FossTech}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
