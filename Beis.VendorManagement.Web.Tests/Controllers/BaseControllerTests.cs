using AutoFixture;
using Beis.Htg.VendorSme.Database;
using Beis.VendorManagement.Web.Extensions;
using Beis.VendorManagement.Web.Handlers.Home;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using System.Text;

namespace Beis.VendorManagement.Web.Tests.Controllers
{
    public abstract class BaseControllerTests
    {
        protected Mock<HtgVendorSmeDbContext> MockHtgVendorSmeDbContext { get; }

        protected Mock<IAsyncNotificationClient> MockNotificationClient { get; }

        protected Mock<ILogger<IndexGetHandler>> MockIndexGetLogger { get; }

        protected Mock<HttpContext> MockHttpContext { get; }

        protected ServiceProvider ServiceProvider { get; }

        protected Fixture AutoFixture { get; }

        protected const long TestProductId = 1;
        protected const long TestProductPriceId = 1;
        protected const long TestPrimaryUserId = 2;
        protected const long TestUserId = 1;
        protected const long TestNonExistingUserId = 5;
        protected const long TestCompanyId = 12345;
        protected const string TestUserEmailAddress = "testuser@test.com";
        protected const string TestIpAddresses = "192.168.1.1;192.168.1.10";
        protected const string Adb2CId1 = "adb2c1";
        protected const string Adb2CId2 = "adb2c2";
        protected const string Adb2CId3 = "adb2c3";

        private const string TestRegistrationId = "12345";
        private const int TestApplicationStatus = 241;
        private const string TestAccessSecret = "12345";

        protected BaseControllerTests()
        {
            MockHttpContext = new Mock<HttpContext>();
            MockHtgVendorSmeDbContext = new Mock<HtgVendorSmeDbContext>();
            MockNotificationClient = new Mock<IAsyncNotificationClient>();
            AutoFixture = new Fixture();
            AutoFixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"AutoLogOutDurationInMinutes", "10"},
                    {"ProductLogoPath", "/uploads/"},
                    {"NotifyServiceConfig:NotifyEmailKey", "testNotifyApiKey"},
                    {"NotifyServiceConfig:EmailVerificationLink", "testEmailVerificationLink"},
                    {"EmailConfig:IdsEmail", "testIdsEmail"},
                    {"EmailConfig:VendorProductSubmittedConfirmTemplateId", "testVendorProductSubmittedConfirmTemplateId"},
                    {"EmailConfig:VendorAdditionalUserTemplateId", "testVendorAdditionalUserTemplateId"},
                    {"EmailConfig:VendorUserDeletedTemplateId", "testVendorUserDeletedTemplateId"}
                })
                .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.RegisterAllServices(configuration, Guid.NewGuid().ToString(), true);
            serviceCollection.AddScoped(options => MockHtgVendorSmeDbContext.Object);
            serviceCollection.AddScoped(options => MockNotificationClient.Object);
            MockIndexGetLogger = new Mock<ILogger<IndexGetHandler>>();
            serviceCollection.AddScoped(options => MockIndexGetLogger.Object);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        protected void SetHttpContext(ControllerContext controllerContext)
        {
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest.Setup(x => x.Scheme).Returns("http");
            mockHttpRequest.Setup(x => x.Host).Returns(new HostString("localhost"));
            MockHttpContext.Setup(r => r.Request).Returns(mockHttpRequest.Object);

            var mockSession = new Mock<ISession>();
            var val = Encoding.UTF8.GetBytes(TestUserId.ToString());
            mockSession.Setup(r => r.TryGetValue("loggedinUserId", out val)).Returns(true);
            MockHttpContext.Setup(r => r.Session).Returns(mockSession.Object);
            controllerContext.HttpContext = MockHttpContext.Object;
        }

        protected void SetUserIdentity(IEnumerable<Claim> claims, bool isAuthenticated = true)
        {
            var claimsIdentity = isAuthenticated ? new ClaimsIdentity(claims, "basic") : new ClaimsIdentity(claims);

            var claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity> { claimsIdentity });
            MockHttpContext.Setup(r => r.User).Returns(claimsPrincipal);
        }

        protected void SetupVendorCompanyUser()
        {
            var vendorCompanyUsers = new List<vendor_company_user>
            {
                AutoFixture.Build<vendor_company_user>()
                    .With(x => x.userid , TestUserId)
                    .With(x => x.companyid, TestCompanyId)
                    .With(x => x.primary_contact, false)
                    .With(x => x.adb2c, Adb2CId1).Create(),
                AutoFixture.Build<vendor_company_user>()
                    .With(x => x.userid , TestPrimaryUserId)
                    .With(x => x.companyid, TestCompanyId)
                    .With(x => x.primary_contact, true)
                    .With(x => x.adb2c, Adb2CId2).Create(),
                AutoFixture.Build<vendor_company_user>()
                    .With(x => x.userid , 3)
                    .With(x => x.companyid, 54321)
                    .With(x => x.primary_contact, true)
                    .With(x => x.adb2c, Adb2CId3).Create()

            };
            var vendorCompanyUsersDbSet = vendorCompanyUsers.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.vendor_company_users).Returns(vendorCompanyUsersDbSet.Object);
        }

        protected void SetupVendorCompanies()
        {

            var vendorCompanies = new List<vendor_company>
            {
                AutoFixture.Build<vendor_company>()
                    .With(x => x.vendorid, TestCompanyId)
                    .With(x => x.registration_id, TestRegistrationId)
                    .With(x => x.access_secret, TestAccessSecret)
                    .With(x => x.application_status, TestApplicationStatus)
                    .With(x => x.vendor_company_users, new List<vendor_company_user>
                    {
                        AutoFixture.Build<vendor_company_user>().With(x => x.companyid, TestCompanyId).Create()
                    }).Create()
            };
            var vendorCompaniesDbSet = vendorCompanies.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.vendor_companies).Returns(vendorCompaniesDbSet.Object);
        }

        protected void SetupProducts(string logo = "logo.jpg", string otherCapabilities = "other_capabilities")
        {
            var products = new List<product>
            {
                AutoFixture.Build<product>()
                    .With(x => x.product_id, 1)
                    .With(x => x.vendor_id, TestCompanyId)
                    .With(x => x.status, 1)
                    .With(x=> x.product_type, 1)
                    .With(x => x.product_logo, logo)
                    .With(x =>x.other_capabilities, otherCapabilities).Create()
            };
            var productsDbSet = products.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.products).Returns(productsDbSet.Object);
        }

        protected void SetupSettingsProductTypes()
        {
            var settingsProductTypes = new List<settings_product_type>
            {
                AutoFixture.Build<settings_product_type>()
                    .With(x => x.id, 1)
                    .With(x => x.item_name, "item name").Create()
            };
            var settingsProductTypesDbSet = settingsProductTypes.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.settings_product_types).Returns(settingsProductTypesDbSet.Object);
        }

        protected void SetupSettingsProductCapabilities()
        {
            var settingsProductCapabilities = new List<settings_product_capability>
            {
                AutoFixture.Build<settings_product_capability>()
                    .With(x => x.capability_id, 1)
                    .With(x => x.capability_name, "capability name")
                    .With(x => x.product_type, 1).Create()
            };
            var settingsProductCapabilitiesDbSet = settingsProductCapabilities.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.settings_product_capabilities).Returns(settingsProductCapabilitiesDbSet.Object);
        }

        protected void SetupProductCapabilities()
        {
            var productCapabilities = new List<product_capability>
            {
                AutoFixture.Build<product_capability>()
                    .With(x => x.capability_id, 1)
                    .With(x => x.product_id, 1).Create()
            };
            var productCapabilitiesDbSet = productCapabilities.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.product_capabilities).Returns(productCapabilitiesDbSet.Object);
        }

        protected void SetupSettingsProductFilters()
        {
            var settingsProductFiltersFilters = new List<settings_product_filter>
            {
                AutoFixture.Build<settings_product_filter>()
                    .With(x => x.filter_id, 1)
                    .With(x => x.filter_name, "filter name")
                    .With(x => x.filter_type, 1).Create(),
                AutoFixture.Build<settings_product_filter>()
                    .With(x => x.filter_id, 2)
                    .With(x => x.filter_name, "filter name")
                    .With(x => x.filter_type, 2).Create(),
                AutoFixture.Build<settings_product_filter>()
                    .With(x => x.filter_id, 3)
                    .With(x => x.filter_name, "filter name")
                    .With(x => x.filter_type, 3).Create()
            };
            var settingsProductFiltersFiltersDbSet = settingsProductFiltersFilters.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.settings_product_filters).Returns(settingsProductFiltersFiltersDbSet.Object);
        }

        protected void SetupProductFilters()
        {
            var productFilters = new List<product_filter>
            {
                AutoFixture.Build<product_filter>()
                    .With(x => x.Id, 1)
                    .With(x => x.product_id, 1)
                    .With(x => x.filter_id, 1).Create(),
                AutoFixture.Build<product_filter>()
                    .With(x => x.Id, 2)
                    .With(x => x.product_id, 1)
                    .With(x => x.filter_id, 2).Create(),
                AutoFixture.Build<product_filter>()
                    .With(x => x.Id, 3)
                    .With(x => x.product_id, 1)
                    .With(x => x.filter_id, 3).Create()
            };
            var productFiltersDbSet = productFilters.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.product_filters).Returns(productFiltersDbSet.Object);
        }

        protected void SetupSettingsProductFiltersCategories()
        {
            var settingsProductFiltersCategories = new List<settings_product_filters_category>
            {
                AutoFixture.Build<settings_product_filters_category>()
                    .With(x => x.id, 1)
                    .With(x => x.item_name, "Support").Create(),
                AutoFixture.Build<settings_product_filters_category>()
                    .With(x => x.id, 2)
                    .With(x => x.item_name, "Training").Create(),
                AutoFixture.Build<settings_product_filters_category>()
                    .With(x => x.id, 3)
                    .With(x => x.item_name, "Platform").Create()
            };
            var settingsProductFiltersCategoriesDbSet = settingsProductFiltersCategories.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.settings_product_filters_categories).Returns(settingsProductFiltersCategoriesDbSet.Object);
        }

        protected void SetupProductPriceBaseMetricPrice()
        {
            var productPriceBaseMetricPrices = new List<product_price_base_metric_price>
            {
                AutoFixture.Build<product_price_base_metric_price>()
                    .With(x => x.product_price_base_id, 1)
                    .With(x => x.product_price_base_description_id, 1)
                    .With(x => x.product_price_id, TestProductPriceId)
                    .Create()
            };
            var productPriceBaseMetricPriceDbSet = productPriceBaseMetricPrices.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.product_price_base_metric_prices).Returns(productPriceBaseMetricPriceDbSet.Object);
        }

        protected void SetupProductPriceBaseDescription()
        {
            var productPriceBaseDescriptions = new List<product_price_base_description>
            {
                AutoFixture.Build<product_price_base_description>()
                    .With(x => x.product_price_base_description_id, 1)
                    .Create()
            };
            var productPriceBaseDescriptionDbSet = productPriceBaseDescriptions.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.product_price_base_descriptions).Returns(productPriceBaseDescriptionDbSet.Object);
        }

        protected void SetupProductPriceSecondaryMetric()
        {
            var productPriceSecondaryMetrics = new List<product_price_secondary_metric>
            {
                AutoFixture.Build<product_price_secondary_metric>()
                    .With(x => x.product_price_sec_id, 1)
                    .With(x => x.product_price_sec_description_id, 1)
                    .With(x => x.product_price_id, TestProductPriceId)
                    .Create()
            };
            var productPriceSecondaryMetricDbSet = productPriceSecondaryMetrics.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.product_price_secondary_metrics).Returns(productPriceSecondaryMetricDbSet.Object);
        }

        protected void SetupProductPriceSecondaryDescription()
        {
            var productPriceSecondaryDescriptions = new List<product_price_secondary_description>
            {
                AutoFixture.Build<product_price_secondary_description>()
                    .With(x => x.product_price_sec_description_id, 1)
                    .Create()
            };
            var productPriceSecondaryDescriptionDbSet = productPriceSecondaryDescriptions.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.product_price_secondary_descriptions).Returns(productPriceSecondaryDescriptionDbSet.Object);
        }

        protected void SetupProductPrices()
        {
            var productPrices = new List<product_price>
            {
                AutoFixture.Build<product_price>()
                    .With(x => x.product_price_id, TestProductPriceId)
                    .With(x => x.productid, TestProductId)
                    .With(x => x.free_trial_end_action_id, 1)
                    .Create()
            };
            var productPricesDbSet = productPrices.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.product_prices).Returns(productPricesDbSet.Object);
        }

        protected void SetupEmptyProductPrices()
        {
            var productPrices = new List<product_price> { };
            var productPricesDbSet = productPrices.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.product_prices).Returns(productPricesDbSet.Object);
        }

        protected void SetupFreeTrialEndAction()
        {
            var freeTrialEndActions = new List<free_trial_end_action>
            {
                AutoFixture.Build<free_trial_end_action>()
                    .With(x => x.free_trial_end_action_id, 1)
                    .Create()
            };
            var freeTrialEndActionsDbSet = freeTrialEndActions.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.free_trial_end_actions).Returns(freeTrialEndActionsDbSet.Object);
        }

        protected void SetupUserDiscount()
        {
            var userDiscounts = new List<user_discount>
            {
                AutoFixture.Build<user_discount>()
                    .With(x => x.user_discount_id, 1)
                    .With(x => x.product_price_id, TestProductPriceId)
                    .Create()
            };
            var userDiscountsDbSet = userDiscounts.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.user_discounts).Returns(userDiscountsDbSet.Object);
        }

        protected void SetupAdditionalCost()
        {
            var additionalCosts = new List<additional_cost>
            {
                AutoFixture.Build<additional_cost>()
                    .With(x => x.additional_cost_id, 1)
                    .With(x => x.additional_cost_desc_id, 1)
                    .With(x => x.product_price_id, TestProductPriceId)
                    .Create()
            };
            var additionalCostsDbSet = additionalCosts.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.additional_costs).Returns(additionalCostsDbSet.Object);
        }

        protected void SetupAdditionalCostDescription()
        {
            var additionalCostDescriptions = new List<additional_cost_desc>
            {
                AutoFixture.Build<additional_cost_desc>()
                    .With(x => x.additional_cost_desc_id, 1)
                    .Create()
            };
            var additionalCostDescriptionsDbSet = additionalCostDescriptions.AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.additional_cost_descs).Returns(additionalCostDescriptionsDbSet.Object);
        }

        protected void SetupEmptyProductCapabilities()
        {
            var productCapabilitiesDbSet = new List<product_capability>().AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.product_capabilities).Returns(productCapabilitiesDbSet.Object);
        }

        protected void SetupEmptySettingsProductCapabilities()
        {
            var settingsProductCapabilitiesDbSet = new List<settings_product_capability>().AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.settings_product_capabilities).Returns(settingsProductCapabilitiesDbSet.Object);
        }

        protected void SetupEmptyProductFilters()
        {
            var productFiltersDbSet = new List<product_filter>().AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.product_filters).Returns(productFiltersDbSet.Object);
        }

        protected void SetupEmptySettingsProductFilters()
        {
            var settingsProductFiltersFiltersDbSet = new List<settings_product_filter>().AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.settings_product_filters).Returns(settingsProductFiltersFiltersDbSet.Object);
        }

        protected void SetupEmptySettingsProductFiltersCategories()
        {
            var settingsProductFiltersCategoriesDbSet = new List<settings_product_filters_category>().AsQueryable().BuildMockDbSet();
            MockHtgVendorSmeDbContext.Setup(context => context.settings_product_filters_categories).Returns(settingsProductFiltersCategoriesDbSet.Object);
        }
    }
}