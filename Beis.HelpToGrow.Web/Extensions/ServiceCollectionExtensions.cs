using Beis.HelpToGrow.Core.Repositories;
using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.HelpToGrow.Core.Repositories.Interface.Pricing;
using Beis.HelpToGrow.Core.Repositories.Pricing;
using Beis.HelpToGrow.Web.Handlers.ManageUsers;
using Beis.HelpToGrow.Web.Handlers.ProductSubmit;
using Beis.HelpToGrow.Web.Options;
using Beis.HelpToGrow.Web.Services;
using Beis.HelpToGrow.Web.Services.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Notify.Interfaces;

namespace Beis.HelpToGrow.Web.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static void RegisterServices(this IServiceCollection services, bool callMockNotifyApi)
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

        internal static void RegisterRepositories(this IServiceCollection services)
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

        internal static void RegisterOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ProductLogoOptions>(o => o.LogoPath = configuration["ProductLogoPath"]);
            services.Configure<ProductSubmitReviewDetailsPostHandler.ProductSubmitReviewDetailsPostHandlerOptions>(configuration.GetSection("EmailConfig"));
            services.Configure<RemoveUserPostHandler.RemoveUserPostHandlerOptions>(configuration.GetSection("EmailConfig"));
            services.Configure<ConfirmPrimaryUserChangePostHandler.PrimaryUserChangePostHandlerOptions>(configuration.GetSection("EmailConfig"));
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