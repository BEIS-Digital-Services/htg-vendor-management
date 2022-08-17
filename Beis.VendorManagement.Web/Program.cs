using Beis.HelpToGrow.Common.Helpers;
using Beis.HelpToGrow.Common.Services.HealthChecks;
using Beis.VendorManagement.Web.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(configBuilder =>
{
    var connectionString = configBuilder.Build().GetConnectionString("AppConfig");
    if (connectionString != null)
    {
        configBuilder.AddAzureAppConfiguration(connectionString);
    }
});

builder.Services.AddMvc(o => o.EnableEndpointRouting = false);
var nonce = Guid.NewGuid().ToString();
// Add services to the container.
builder.Services.RegisterAllServices(builder.Configuration, nonce);
builder.Services.AddHealthChecks()
.AddDbContextCheck<HtgVendorSmeDbContext>("VendorSme Database")
.AddCheck<DependencyInjectionHealthCheckService>("Dependency Injection");

var app = builder.Build();
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/LocalDevelopmentError");
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

    var allowedScripts = string.Join(" ", builder.Configuration.GetSection("AllowedScripts").Value.Split(','));
    context.Response.Headers.Add("Content-Security-Policy", $"script-src 'self' 'unsafe-eval' 'nonce-{nonce}' {allowedScripts}");

    context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    await next();
});
app.UseMvc(r => r.MapRoute("default", "{controller=Home}/{action=Index}"));
app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = HealthCheckJsonResponseWriter.Write
});

app.Run();