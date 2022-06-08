using Beis.Htg.VendorSme.Database;
using Beis.VendorManagement.Repositories;
using Beis.VendorManagement.Web.Filters;
using Beis.VendorManagement.Web.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Net.Mime;

namespace Beis.VendorManagement.Web.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static void RegisterAllServices(this IServiceCollection services, IConfiguration configuration, string nonce, bool callMockApi = false)
        {
            services.AddSession(options => options.Cookie.IsEssential = false);
            services.AddLogging(options => options.AddConsole());
            services.AddApplicationInsightsTelemetry(configuration["AzureMonitorInstrumentationKey"]);
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options =>
                {
                    configuration.Bind("AzureAdB2C", options);
                    options.Events ??= new OpenIdConnectEvents();
                    options.Events.OnRedirectToIdentityProvider += UpdateRedirectUri;
                });
            services.AddControllersWithViews().AddMicrosoftIdentityUI();
            services.AddControllers(options =>
            {
                options.Filters.Add(new HttpResponseExceptionFilter());
                options.Filters.Add<AuthorizationPrivilegeFilter>();
            }).AddFluentValidation(fv =>
            {
                fv.DisableDataAnnotationsValidation = true;
                fv.ImplicitlyValidateChildProperties = true;
                fv.RegisterValidatorsFromAssemblyContaining<Program>();
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
            services.AddDbContext<HtgVendorSmeDbContext>(options => options.UseNpgsql(configuration["HelpToGrowDbConnectionString"]));
            services.AddDataProtection().PersistKeysToDbContext<HtgVendorSmeDbContext>();
            services.RegisterServices(callMockApi);
            services.RegisterRepositories();
            services.AddRazorPages();
            services.AddOptions();
            services.RegisterOptions(configuration, nonce);
            services.AddMediatR(typeof(Program));
            services.AddAutoMapper(c => c.AddProfile<AutoMap>(), typeof(Program));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        private static Task UpdateRedirectUri(RedirectContext context)
        {
            // check if http has found in RedirectUri and replace it with https 
            context.ProtocolMessage.RedirectUri = context.ProtocolMessage.RedirectUri.Replace("http:", "https:");

            // Don't remove this line
            return Task.FromResult(0);
        }

        private static void RegisterServices(this IServiceCollection services, bool callMockNotifyApi)
        {
            services.AddScoped<IActivateAccountService, ActivateAccountService>();
            services.AddScoped<IAccountHomeService, AccountHomeService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IManageUsersService, ManageUsersService>();
            services.AddScoped<IVendorCompanyService, VendorCompanyService>();
            services.AddScoped<IPricingService, PricingService>();

            // Added for unit test support as the actual notify email service shouldn't be triggered. The IAsyncNotificationClient is mocked for unit tests
            if (callMockNotifyApi)
            {
                services.AddScoped<INotifyService>(ctx => new NotifyService(
                    ctx.GetRequiredService<IVendorCompanyService>(),
                    ctx.GetRequiredService<IAsyncNotificationClient>(),
                    ctx.GetRequiredService<IOptions<NotifyService.NotifyServiceOptions>>()));
            }
            else
            {
                services.AddScoped<INotifyService>(ctx => new NotifyService(
                    ctx.GetRequiredService<IVendorCompanyService>(),
                    ctx.GetRequiredService<IOptions<NotifyService.NotifyServiceOptions>>()));
            }

        }

        private static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICompanyUserRepository, CompanyUserRepository>();
            services.AddScoped<IProductFiltersRepository, ProductFiltersRepository>();
            services.AddScoped<IManageUsersRepository, ManageUsersRepository>();
            services.AddScoped<ISettingsProductFiltersCategoriesRepository, SettingsProductFiltersCategoriesRepository>();
            services.AddScoped<ISettingsProductFiltersRepository, SettingsProductFiltersRepository>();
            services.AddScoped<ISettingsProductTypesRepository, SettingsProductTypesRepository>();
            services.AddScoped<ISettingsProductCapabilitiesRepository, SettingsProductCapabilitiesRepository>();
            services.AddScoped<IProductCapabilitiesRepository, ProductCapabilitiesRepository>();
            services.AddScoped<IVendorCompanyRepository, VendorCompanyRepository>();
            services.AddScoped<IPricingRepository, PricingRepository>();
        }

        private static void RegisterOptions(this IServiceCollection services, IConfiguration configuration, string nonce = "")
        {
            services.Configure<LayoutOptions>(o =>
            {
                o.AutoLogoutDuration = configuration.GetValue<int>("AutoLogoutDurationInSeconds", 0);
                o.Nonce = nonce;
            });
            services.Configure<ProductLogoOptions>(o => o.LogoPath = configuration["ProductLogoPath"]);
            services.Configure<ProductSubmitReviewDetailsPostHandler.ProductSubmitReviewDetailsPostHandlerOptions>(configuration.GetSection("EmailConfig"));
            services.Configure<RemoveUserPostHandler.RemoveUserPostHandlerOptions>(configuration.GetSection("EmailConfig"));
            services.Configure<ConfirmPrimaryUserChangePostHandler.PrimaryUserChangePostHandlerOptions>(configuration.GetSection("EmailConfig"));
            services.Configure<PrimaryUserChangePostHandler.PrimaryUserChangePostHandlerOptions>(configuration.GetSection("EmailConfig"));
            services.Configure<NotifyService.NotifyServiceOptions>(configuration.GetSection("NotifyServiceConfig"));
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.Secure = CookieSecurePolicy.Always;
            });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto |
                                           ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedHost;
                options.ForwardedHostHeaderName = "X-Original-Host";
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }
    }
}