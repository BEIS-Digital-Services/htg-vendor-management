using AutoMapper.Internal;
using Beis.HelpToGrow.Web.Constants;
using Beis.HelpToGrow.Web.Controllers;
using Beis.HelpToGrow.Web.Models;
using Beis.HelpToGrow.Web.Services.Interface;
using Beis.Htg.VendorSme.Database.Models;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Beis.HelpToGrow.Tests.UnitTests.Controllers
{
    public class ProductControllerTests : BaseControllerTests
    {
        private readonly ProductController _sut;

        public ProductControllerTests()
        {
            _sut = new ProductController(
                ServiceProvider.GetService<IMediator>(),
                ServiceProvider.GetService<IProductService>());

            SetHttpContext(_sut.ControllerContext);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ProductShouldRedirectToErrorWhenAdb2CIdIsInvalid(string adb2CId)
        {
            // Arrange
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, adb2CId) });

            // Act
            var result = await _sut.SoftwareHome(It.IsAny<int>()) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductErrorGet);
        }

        [Theory]
        [InlineData(1, "adb2c3")] // With incorrect adb2cid id
        [InlineData(2, Adb2CId1)] //With incorrect product id
        public async Task ProductShouldRedirectToErrorWhenProductIsNull(int productId, string adb2C)
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, adb2C) });

            // Act
            var result = await _sut.SoftwareHome(productId) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductErrorGet);

            // Act
            result = await _sut.RedemptionUrl(productId) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductErrorGet);

            // Act
            result = await _sut.Sku(productId) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductErrorGet);

            // Act
            result = await _sut.ProductLogo(productId) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductErrorGet);
        }

        [Fact]
        public async Task ShouldGetProductWithSubmitForReview()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductTypes();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();
            SetupSettingsProductFilters();
            SetupProductFilters();
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId2) });

            // Act
            var result = await _sut.SoftwareHome(1) as ViewResult;

            // Assert
            AssertGetProductWithoutSubmitForReview(result, true, true, true, true);
        }

        [Fact]
        public async Task ShouldGetProductWithoutSubmitForReviewBasedWithoutOtherCapabilities()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductTypes();
            SetupSettingsProductCapabilities();
            SetupEmptyProductCapabilities();
            SetupSettingsProductFilters();
            SetupProductFilters();
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId1) });

            // Act
            var result = await _sut.SoftwareHome(1) as ViewResult;

            // Assert
            AssertGetProductWithoutSubmitForReview(result, true, true, true, true);
        }

        [Fact]
        public async Task ShouldGetProductWithoutSubmitForReviewWithoutExistingAndOtherCapabilities()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts(otherCapabilities: null);
            SetupSettingsProductTypes();
            SetupSettingsProductCapabilities();
            SetupEmptyProductCapabilities();
            SetupSettingsProductFilters();
            SetupProductFilters();
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId1) });

            // Act
            var result = await _sut.SoftwareHome(1) as ViewResult;

            // Assert
            AssertGetProductWithoutSubmitForReview(result, false, true, true, false);
        }

        [Fact]
        public async Task ShouldGetProductWithoutSubmitForReviewOnWithoutProductFilters()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductTypes();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();
            SetupSettingsProductFilters();
            SetupEmptyProductFilters();
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId2) });

            // Act
            var result = await _sut.SoftwareHome(1) as ViewResult;

            // Assert
            AssertGetProductWithoutSubmitForReview(result);
        }

        [Fact]
        public async Task ShouldGetProductWithoutSubmitForReviewOnWithoutSettingsProductFilters()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductTypes();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();
            SetupEmptySettingsProductFilters();
            SetupProductFilters();
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId1) });

            // Act
            var result = await _sut.SoftwareHome(1) as ViewResult;

            // Assert
            AssertGetProductWithoutSubmitForReview(result);
        }

        [Fact]
        public async Task ShouldGetProductWithoutSubmitForReviewWhenFilterIdDoNotMatch()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductTypes();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();
            SetupSettingsProductFilters();
            SetupProductFilters();
            MockHtgVendorSmeDbContext.Object.product_filters.ForAll(r => r.filter_id = 4);
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId2) });

            // Act
            var result = await _sut.SoftwareHome(1) as ViewResult;

            // Assert
            AssertGetProductWithoutSubmitForReview(result);
        }

        [Fact]
        public async Task ShouldGetRedemptionUrl()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId1) });

            // Act
            var result = await _sut.RedemptionUrl(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as RedemptionUrlViewModel;
            Assert.NotNull(model);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.RedemptionUrl.Should().NotBeNullOrWhiteSpace();
            model.ProductId.Should().BeGreaterThan(0);
            //model.UserId.Should().BeGreaterThan(0);
            model.ShowValidationError.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldNotAddRedemptionUrlWhenModelStateIsInvalid()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            _sut.ModelState.AddModelError("redemption_url", "");
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId2) });

            // Act
            var result = await _sut.RedemptionUrl(new RedemptionUrlViewModel { ProductId = 1 }) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as RedemptionUrlViewModel;
            Assert.NotNull(model);
            //model.UserId.Should().BeGreaterThan(0);
            model.ShowValidationError.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldAddRedemptionUrl()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId1) });

            // Act
            var result = await _sut.RedemptionUrl(new RedemptionUrlViewModel { ProductId = 1, RedemptionUrl = "url" }) as RedirectToRouteResult;

            // Assert
            AssertForDbUpdates(result);
        }

        [Fact]
        public void ShouldGetTechnicalSolutions()
        {
            // Arrange

            // Act
            var result = _sut.TechnicalSolutions() as RedirectToRouteResult;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(RouteNameConstants.TechnicalSolutionsGet, result.RouteName);
        }

        [Fact]
        public async Task ShouldGetSku()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId2) });

            // Act
            var result = await _sut.Sku(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as SkuViewModel;
            Assert.NotNull(model);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.ProductSku.Should().NotBeNullOrWhiteSpace();
            model.ProductId.Should().BeGreaterThan(0);
            //model.UserId.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ShouldAddSku()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId1) });

            // Act
            var result = await _sut.Sku(new SkuViewModel { ProductId = 1, ProductSku = "product_sku" }) as RedirectToRouteResult;

            // Assert
            AssertForDbUpdates(result);
        }

        [Fact]
        public async Task ShouldGetProductLogo()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId2) });

            // Act
            var result = await _sut.ProductLogo(1) as ViewResult;

            // Assert
            var model = AssertGetProductLogo(result);
            model.ProductLogo.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        public async Task ShouldGetProductLogoPageWithoutLogo(string logoName)
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts(logoName);
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId1) });

            // Act
            var result = await _sut.ProductLogo(1) as ViewResult;

            // Assert
            var model = AssertGetProductLogo(result);
            model.ProductLogo.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ShouldNotAddProductLogoWhenModelStateIsInvalid()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            _sut.ModelState.AddModelError("file", "");

            // Act
            var result = await _sut.ProductLogo(new ProductLogoViewModel { ProductId = 1, ProductName = "product1", ProductLogo = "productlogo1" }) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as ProductLogoViewModel;
            Assert.NotNull(model);
            model.File.Should().BeNull();
        }

        [Fact]
        public async Task ShouldAddProductLogo()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(r => r.FileName).Returns("filename.jpg");
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads"));
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId2) });

            // Act
            var result = await _sut.ProductLogo(new ProductLogoViewModel { ProductId = 1, ProductName = "product1", ProductLogo = "productlogo1", File = mockFile.Object }) as RedirectToRouteResult;

            // Assert
            AssertForDbUpdates(result);
        }

        private static void AssertGetProductWithoutSubmitForReview(ViewResult result, bool hasCapabilities = true, bool hasProductSupport = false, bool hasPlatformDetails = false, bool canSubmitForReview = false)
        {
            Assert.NotNull(result);
            var model = result.Model as SoftwareHomeViewModel;
            Assert.NotNull(model);
            model.ProductId.Should().BeGreaterThan(0);
            //model.UserId.Should().BeGreaterThan(0);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.ProductStatus.Should().BeOneOf(ProductStatus.Approved, ProductStatus.InReview, ProductStatus.Incomplete, ProductStatus.NotInScheme);
            model.ProductTypeName.Should().NotBeNullOrWhiteSpace();
            model.DraftProductDescription.Should().NotBeNullOrWhiteSpace();

            model.HasCapabilities.Should().Be(hasCapabilities);
            model.HasProductSupport.Should().Be(hasProductSupport);
            model.HasPlatformDetails.Should().Be(hasPlatformDetails);
            model.CanSubmitForReview.Should().Be(canSubmitForReview);
        }

        private static ProductLogoViewModel AssertGetProductLogo(ViewResult result)
        {
            Assert.NotNull(result);
            var model = result.Model as ProductLogoViewModel;
            Assert.NotNull(model);
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            //model.UserId.Should().BeGreaterThan(0);
            return model;
        }

        private void AssertForDbUpdates(RedirectToRouteResult result, int numberOfProductUpdate = 1, int numberOfSave = 1)
        {
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.SoftwareHomeGet);
            result.RouteValues.Count.Should().Be(1);
            result.RouteValues.ContainsKey("id").Should().BeTrue();
            MockHtgVendorSmeDbContext.Verify(r => r.products.Update(It.Is<product>(p => p.product_id == 1)), Times.Exactly(numberOfProductUpdate));
            MockHtgVendorSmeDbContext.Verify(r => r.SaveChangesAsync(default), Times.Exactly(numberOfSave));
        }
    }
}