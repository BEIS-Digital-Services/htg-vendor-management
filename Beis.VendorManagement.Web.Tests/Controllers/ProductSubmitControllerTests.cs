using Beis.Htg.VendorSme.Database.Models;
using Beis.VendorManagement.Web.Constants;
using Beis.VendorManagement.Web.Controllers;
using Beis.VendorManagement.Web.Models;
using Beis.VendorManagement.Web.Services.Interface;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Beis.VendorManagement.Web.Tests.Controllers
{
    public class ProductSubmitControllerTests : BaseControllerTests
    {

        private readonly ProductSubmitController _sut;

        public ProductSubmitControllerTests()
        {
            _sut = new ProductSubmitController(
                ServiceProvider.GetService<IMediator>(),
                ServiceProvider.GetService<IProductService>());

            SetHttpContext(_sut.ControllerContext);
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId2) });

        }

        [Theory]
        [InlineData(1, "adb2c5")] // With incorrect adb2c id
        [InlineData(2, Adb2CId1)] //With incorrect product id
        public async Task ProductSubmitShouldRedirectToErrorWhenProductIsNull(int productId, string adb2CId)
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, adb2CId) });

            // Act
            var result = await _sut.ProductSubmitReviewDetails(productId) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductErrorGet);

            // Act
            result = await _sut.ProductSubmitConfirmation(productId, TestUserEmailAddress) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductErrorGet);
        }

        [Fact]
        public async Task ShouldGetProductSubmitReviewDetails()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();
            SetupSettingsProductFiltersCategories();
            SetupSettingsProductFilters();
            SetupProductFilters();

            // Act
            var result = await _sut.ProductSubmitReviewDetails(1) as ViewResult;

            // Assert
            AssertGetProductSubmitReviewDetails(result);
        }

        [Fact]
        public async Task ShouldGetProductSubmitReviewDetailsWithoutProductCapabilities()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductCapabilities();
            SetupEmptyProductCapabilities();
            SetupSettingsProductFiltersCategories();
            SetupSettingsProductFilters();
            SetupProductFilters();

            // Act
            var result = await _sut.ProductSubmitReviewDetails(1) as ViewResult;

            // Assert
            AssertGetProductSubmitReviewDetails(result);
        }

        [Fact]
        public async Task ShouldGetProductSubmitReviewDetailsWithoutSettingsProductCapabilities()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupEmptySettingsProductCapabilities();
            SetupProductCapabilities();
            SetupSettingsProductFiltersCategories();
            SetupSettingsProductFilters();
            SetupProductFilters();

            // Act
            var result = await _sut.ProductSubmitReviewDetails(1) as ViewResult;

            // Assert
            AssertGetProductSubmitReviewDetails(result);
        }

        [Fact]
        public async Task ShouldPostProductSubmitReviewDetails()
        {
            // Arrange
            SetupProducts();
            SetupVendorCompanyUser();
            SetupVendorCompanies();

            // Act
            var result = await _sut.ProductSubmitReviewDetails(new ProductSubmitReviewDetailsViewModel { ProductId = 1 }) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductSubmitConfirmationGet);
            result.RouteValues.Count.Should().Be(2);
            result.RouteValues.ContainsKey("id").Should().BeTrue();
            result.RouteValues.ContainsKey("email").Should().BeTrue();
            MockHtgVendorSmeDbContext.Verify(r => r.products.Update(It.Is<product>(p => p.product_id == 1)), Times.Once);
            MockHtgVendorSmeDbContext.Verify(r => r.SaveChangesAsync(default), Times.Once);
            MockNotificationClient.Verify(r => r.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), default, default), Times.Exactly(2));
        }

        [Fact]
        public async Task ShouldGetProductSubmitConfirmation()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();

            // Act
            var result = await _sut.ProductSubmitConfirmation(1, "email@test.com") as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as ProductSubmitConfirmationViewModel;
            Assert.NotNull(model);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.Email.Should().NotBeNullOrWhiteSpace();
            model.ProductId.Should().BeGreaterThan(0);
            model.ContentKey.Should().Contain(AnalyticConstants.ProductSubmitConfirmation);
        }

        private static void AssertGetProductSubmitReviewDetails(ViewResult result)
        {
            Assert.NotNull(result);
            var model = result.Model as ProductSubmitReviewDetailsViewModel;
            Assert.NotNull(model);
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.DraftProductDescription.Should().NotBeNullOrWhiteSpace();
            model.RedemptionUrl.Should().NotBeNullOrWhiteSpace();
            model.DraftReviewUrl.Should().NotBeNullOrWhiteSpace();
            model.ProductCapabilities.Any().Should().BeTrue();
            model.ProductLogo.Should().NotBeNullOrWhiteSpace();
            model.DraftOtherCompatibility.Should().NotBeNullOrWhiteSpace();
            model.ContentKey.Should().Contain(AnalyticConstants.ProductSubmitReviewDetails);
        }
    }
}