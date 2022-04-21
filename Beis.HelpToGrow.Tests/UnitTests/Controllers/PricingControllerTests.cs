using Beis.HelpToGrow.Web.Controllers;
using Beis.HelpToGrow.Web.Models;
using Beis.HelpToGrow.Web.Models.Pricing;
using Beis.HelpToGrow.Web.Services.Interface;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Beis.HelpToGrow.Tests.UnitTests.Controllers
{
    public class PricingControllerTests : BaseControllerTests
    {

        private readonly PricingController _sut;

        public PricingControllerTests()
        {
            _sut = new PricingController(
                ServiceProvider.GetService<IProductService>(),
                ServiceProvider.GetService<IPricingService>());

            SetHttpContext(_sut.ControllerContext);
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId1) });
        }

        [Fact]
        public async Task HomeShouldRedirectToView()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();

            // Act
            var result = await _sut.Home((int)TestProductId, TestProductPriceId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as PricingHomeViewModel;
            Assert.NotNull(model);
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductPriceId.Should().Be(TestProductPriceId);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task MetricShouldRedirectToView()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupProductPriceBaseMetricPrice();
            SetupProductPriceSecondaryMetric();
            SetupProductPriceBaseDescription();
            SetupProductPriceSecondaryDescription();

            // Act
            var result = await _sut.Metric((int)TestProductId, TestProductPriceId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as MetricDetailsViewModel;
            Assert.NotNull(model);
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductPriceId.Should().Be(TestProductPriceId);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.PrimaryMetricDetails.Any().Should().BeTrue();
            model.PrimaryMetricDetails.All(r => r.Amount > 0).Should().BeTrue();
            model.PrimaryMetricDetails.All(r => r.NumberOfUsers > 0).Should().BeTrue();
            model.PrimaryMetricDetails.All(r => r.PricePercentage > 0).Should().BeTrue();
            model.PrimaryMetricDetails.All(r => !string.IsNullOrWhiteSpace(r.Description)).Should().BeTrue();
            model.SecondaryMetricDetails.Any().Should().BeTrue();
            model.SecondaryMetricDetails.All(r => !string.IsNullOrWhiteSpace(r.Description)).Should().BeTrue();
            model.SecondaryMetricDetails.All(r => r.MetricNumber > 0).Should().BeTrue();
            model.SecondaryMetricDetails.All(r => !string.IsNullOrWhiteSpace(r.MetricUnit)).Should().BeTrue();
        }

        [Fact]
        public async Task MinimumCommitmentShouldRedirectToView()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupProductPrices();
            SetupFreeTrialEndAction();

            // Act
            var result = await _sut.MinimumCommitment((int)TestProductId, TestProductPriceId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as MinimumCommitmentViewModel;
            Assert.NotNull(model);
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductPriceId.Should().Be((int)TestProductPriceId);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.CommitmentFlag.Should().As<bool>();
            model.CommitmentUnit.Should().NotBeNullOrWhiteSpace();
            model.CommitmentNo.Should().BeGreaterThan(0);
            model.MinNoUsers.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task FreeTrialShouldRedirectToView()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupProductPrices();
            SetupFreeTrialEndAction();

            // Act
            var result = await _sut.FreeTrial((int)TestProductId, TestProductPriceId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as FreeTrialViewModel;
            Assert.NotNull(model);
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductPriceId.Should().Be((int)TestProductPriceId);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.FreeTrialFlag.Should().As<bool>();
            model.FreeTrialTermNo.Should().BeGreaterThan(0);
            model.FreeTrialTermUnit.Should().NotBeNullOrWhiteSpace();
            model.FreeTrialPaymentUpfront.Should().As<bool>();
            model.FreeTrialEndActionDescription.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task FreeTrialShouldRedirectToViewWithEmptyProductPricesData()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupEmptyProductPrices();

            // Act
            var result = await _sut.FreeTrial((int)TestProductId, TestProductPriceId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as FreeTrialViewModel;
            Assert.NotNull(model);
            model.ProductPriceId.Should().BeGreaterThan(0);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.FreeTrialTermNo.Should().Be(0);
            model.FreeTrialTermUnit.Should().BeNull();
        }

        [Fact]
        public async Task DiscountPeriodShouldRedirectToView()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupProductPrices();
            SetupFreeTrialEndAction();

            // Act
            var result = await _sut.DiscountPeriod((int)TestProductId, TestProductPriceId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as DiscountPeriodViewModel;
            Assert.NotNull(model);
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductPriceId.Should().Be((int)TestProductPriceId);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.DiscountFlag.Should().As<bool>();
            model.DiscountTermUnit.Should().NotBeNullOrWhiteSpace();
            model.DiscountTermNo.Should().BeGreaterThan(0);
            model.DiscountPrice.Should().BeGreaterThan(0);
            model.DiscountPercentage.Should().BeGreaterThan(0);
            model.DiscountApplicationDescription.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task AdditionalDiscountsShouldRedirectToView()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupProductPrices();
            SetupFreeTrialEndAction();
            SetupUserDiscount();

            // Act
            var result = await _sut.AdditionalDiscounts((int)TestProductId, TestProductPriceId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as AdditionalDiscountsViewModel;
            Assert.NotNull(model);
            model.Should().NotBeNull();
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductPriceId.Should().Be((int)TestProductPriceId);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.HasPricing.Should().BeTrue();
            model.ContractDurationDiscountFlag.Should().As<bool>();
            model.ContractDurationDiscountUnit.Should().NotBeNullOrWhiteSpace();
            model.ContractDurationDiscount.Should().BeGreaterThan(0);
            model.ContractDurationDiscountPercentage.Should().BeGreaterThan(0);
            model.ContractDurationDiscountDescription.Should().NotBeNullOrWhiteSpace();
            model.PaymentTermsDiscountFlag.Should().As<bool>();
            model.PaymentTermsDiscountUnit.Should().NotBeNullOrWhiteSpace();
            model.PaymentTermsDiscount.Should().BeGreaterThan(0);
            model.PaymentTermsDiscountPercentage.Should().BeGreaterThan(0);
            model.PaymentTermsDiscountDescription.Should().NotBeNullOrWhiteSpace();
            model.UserDiscounts.Any().Should().BeTrue();
            model.UserDiscounts.All(r => r.product_price_id > 0).Should().BeTrue();
            model.UserDiscounts.All(r => r.min_licenses > 0).Should().BeTrue();
            model.UserDiscounts.All(r => r.max_licenses > 0).Should().BeTrue();
            model.UserDiscounts.All(r => r.discount_price > 0).Should().BeTrue();
            model.UserDiscounts.All(r => r.discount_percentage > 0).Should().BeTrue();
            model.UserDiscounts.All(r => !string.IsNullOrWhiteSpace(r.discount_sku)).Should().BeTrue();
        }

        [Fact]
        public async Task AdditionalCostsShouldRedirectToView()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupAdditionalCost();
            SetupAdditionalCostDescription();

            // Act
            var result = await _sut.AdditionalCosts((int)TestProductId, TestProductPriceId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as AdditionalCostsViewModel;
            Assert.NotNull(model);
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductPriceId.Should().Be((int)TestProductPriceId);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.AdditionalCosts.Any().Should().BeTrue();
            model.AdditionalCosts.All(r => !string.IsNullOrWhiteSpace(r.Type)).Should().BeTrue();
            model.AdditionalCosts.All(r => !string.IsNullOrWhiteSpace(r.CostAndFrequency)).Should().BeTrue();
            Assert.IsType<bool>(model.AdditionalCosts.All(r => r.IsMandatory));
        }

        [Fact]
        public async Task ProductPricingShouldRedirectToView()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupProductPrices();

            // Act
            var result = await _sut.ProductPricing((int)TestProductId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as ProductPriceDetailsViewModel;
            Assert.NotNull(model);
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.ProductPrices.Any().Should().BeTrue();
            model.ProductPrices.All(r => !string.IsNullOrWhiteSpace(r.ProductPriceTitle)).Should().BeTrue();
            model.ProductPrices.All(r => !string.IsNullOrWhiteSpace(r.VoucherUrl)).Should().BeTrue();
            model.ProductPrices.All(r => !string.IsNullOrWhiteSpace(r.ProductPriceSku)).Should().BeTrue();
            model.ProductPrices.All(r => r.ProductPriceId > 0).Should().BeTrue();
        }
    }
}