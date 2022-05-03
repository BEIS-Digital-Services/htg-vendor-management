using Beis.Htg.VendorSme.Database;
using Beis.VendorManagement.Web.Extensions;
using Beis.VendorManagement.Web.Filters;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Web
{
    public class Startup
    {
        private readonly bool _callMockNotifyApi;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, bool callMockNotifyApi = false)
        {
            _configuration = configuration;
            AutoLogoutDurationInSeconds = _configuration.GetValue("AutoLogoutDurationInMinutes", 0) * 60;
            _callMockNotifyApi = callMockNotifyApi;
        }

        public static int AutoLogoutDurationInSeconds { get; private set; }

        public static string Nonce { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = false;
            });

            services.AddLogging(options => { options.AddConsole(); });

            services.AddApplicationInsightsTelemetry(_configuration["AzureMonitorInstrumentationKey"]);

            services.AddMvc(o => o.EnableEndpointRouting = false);

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options =>
                {
                    _configuration.Bind("AzureAdB2C", options);
                    options.Events ??= new OpenIdConnectEvents();
                    options.Events.OnRedirectToIdentityProvider += OnRedirectToIdentityProviderFunc;

                    if (_configuration["Environment"].Equals(Environments.Production, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Task RedirectToIdentityProvider(RedirectContext ctx)
                        {
                            ctx.ProtocolMessage.RedirectUri = "https://www.manage-your-technology-vendor-account.service.gov.uk/signin-oidc";
                            return Task.FromResult(0);
                        }

                        options.Events = new OpenIdConnectEvents
                        {
                            OnRedirectToIdentityProvider = RedirectToIdentityProvider
                        };
                    }
                });

            services.AddControllersWithViews().AddMicrosoftIdentityUI();

            services.AddControllers(options =>
            {
                options.Filters.Add(new HttpResponseExceptionFilter());
                options.Filters.Add<AuthorizationPrivilegeFilter>();
            })
                .AddFluentValidation(fv =>
                {
                    fv.DisableDataAnnotationsValidation = true;
                    fv.ImplicitlyValidateChildProperties = true;
                    fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var result = new BadRequestObjectResult(context.ModelState);

                        result.ContentTypes.Add(MediaTypeNames.Application.Json);
                        result.ContentTypes.Add(MediaTypeNames.Application.Xml);

                        return result;
                    };
                });

            services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            services.AddDbContext<HtgVendorSmeDbContext>(options => options.UseNpgsql(_configuration["HelpToGrowDbConnectionString"]));
            services.AddDataProtection().PersistKeysToDbContext<HtgVendorSmeDbContext>();
            services.RegisterServices(_callMockNotifyApi);
            services.RegisterRepositories();
            services.AddRazorPages();
            services.AddOptions();
            services.RegisterOptions(_configuration);
            services.AddMediatR(typeof(Startup));
            services.AddAutoMapper(c => c.AddProfile<AutoMap>(), typeof(Startup));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        private async Task OnRedirectToIdentityProviderFunc(RedirectContext context)
        {
            // check if http has found in RedirectUri and replace it with https 
            if (context.ProtocolMessage.RedirectUri.Contains("http:"))
                context.ProtocolMessage.RedirectUri = context.ProtocolMessage.RedirectUri.Replace("http:", "https:");

            // Don't remove this line
            await Task.CompletedTask.ConfigureAwait(false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
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
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

                Nonce = Guid.NewGuid().ToString();
                var allowedScripts = string.Join(" ", _configuration.GetSection("AllowedScripts").Value.Split(','));
                context.Response.Headers.Add("Content-Security-Policy", $"script-src 'self' 'unsafe-eval' 'nonce-{Nonce}' {allowedScripts}");

                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                await next();
            });

            app.UseMvc(r => r.MapRoute("default", "{controller=Home}/{action=Index}"));
        }
    }
}