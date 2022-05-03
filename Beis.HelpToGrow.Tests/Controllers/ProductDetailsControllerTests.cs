using Beis.Htg.VendorSme.Database.Models;
using Beis.VendorManagement.Web.Constants;
using Beis.VendorManagement.Web.Controllers;
using Beis.VendorManagement.Web.Models;
using Beis.VendorManagement.Web.Models.Enums;
using Beis.VendorManagement.Web.Services.Interface;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Beis.VendorManagement.Web.Tests.Controllers
{
    public class ProductDetailsControllerTests : BaseControllerTests
    {

        private readonly ProductDetailsController _sut;

        public ProductDetailsControllerTests()
        {
            _sut = new ProductDetailsController(
                ServiceProvider.GetService<IMediator>(),
                ServiceProvider.GetService<IProductService>());

            SetHttpContext(_sut.ControllerContext);
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId1) });

        }

        [Theory]
        [InlineData(1, "adb2c4")] // With incorrect adb2c id
        [InlineData(2, Adb2CId2)] //With incorrect product id
        public async Task ProductDetailsShouldRedirectToErrorWhenProductIsNull(int productId, string adb2c)
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, adb2c) });

            // Act
            var result = await _sut.ProductCapabilities(productId) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductErrorGet);

            // Act
            result = await _sut.ProductSupport(productId) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductErrorGet);

            // Act
            result = await _sut.PlatformDetails(productId) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductErrorGet);
        }

        [Fact]
        public async Task ShouldGetProductSummary()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();

            // Act
            var result = await _sut.ProductSummary(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as SummaryViewModel;
            Assert.NotNull(model);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.DraftProductDescription.Should().NotBeNullOrWhiteSpace();
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductStatus.Should().BeOneOf(ProductStatus.Approved, ProductStatus.InReview,
                ProductStatus.Incomplete, ProductStatus.NotInScheme);
        }

        [Fact]
        public async Task ShouldNotAddProductProductSummaryWhenModelStateIsInvalid()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            _sut.ModelState.AddModelError("DraftProductDescription", "");

            // Act
            var result = await _sut.ProductSummary(new SummaryViewModel { ProductId = 1, ProductName = "product1" }) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as SummaryViewModel;
            Assert.NotNull(model);
            model.DraftProductDescription.Should().BeNull();
        }

        [Fact]
        public async Task ShouldAddProductSummary()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();

            // Act
            var result = await _sut.ProductSummary(new SummaryViewModel { ProductId = 1, ProductName = "product1", DraftProductDescription = "draft1", Adb2CId = Adb2CId1 }) as RedirectToRouteResult;

            // Assert
            AssertForDbUpdates(result);
        }

        [Fact]
        public async Task ShouldGetProductCapabilities()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();

            // Act
            var result = await _sut.ProductCapabilities(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as ProductCapabilitiesViewModel;
            Assert.NotNull(model);
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductTypeId.Should().BeGreaterThan(0);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            //model.UserId.Should().BeGreaterThan(0);
            model.Status.Should().BeOneOf(ProductStatus.Approved, ProductStatus.InReview,
                ProductStatus.Incomplete, ProductStatus.NotInScheme);
            model.SettingsProductCapabilitiesList.Count.Should().BeGreaterThan(0);
            model.SettingsProductCapabilitiesList.Any(r => r.Selected).Should().BeTrue();
            model.SettingsProductCapabilitiesList.All(r => r.Text.Length > 0).Should().BeTrue();
            model.SettingsProductCapabilitiesList.All(r => r.Value.Length > 0).Should().BeTrue();
        }

        [Fact]
        public async Task ShouldAddProductCapabilities()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();
            var input = new ProductCapabilitiesViewModel
            {
                SettingsProductCapabilitiesList = new List<SelectListItem> { new SelectListItem("test1", "1", true) },
                DraftAdditionalCapabilities = "DraftAdditionalCapabilities"
            };
            var prodCapabilities = MockHtgVendorSmeDbContext.Object.product_capabilities.Where(r => r.product_id == 1 && r.capability_id == 1);

            // Act
            var result = await _sut.ProductCapabilities(input, 1, 1) as RedirectToRouteResult;

            // Assert
            AssertForDbUpdates(result, 1, 3);
            MockHtgVendorSmeDbContext.Verify(r => r.product_capabilities.RemoveRange(prodCapabilities), Times.Once);
            MockHtgVendorSmeDbContext.Verify(r => r.product_capabilities.AddAsync(It.IsAny<product_capability>(), default), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("DraftAdditionalCapabilities")]
        public async Task ShouldAddProductCapabilitiesWithoutUpdatingDraftCapabilities(string draftCapabilities)
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();
            MockHtgVendorSmeDbContext.Object.products.Single().draft_other_capabilities = draftCapabilities;
            var input = new ProductCapabilitiesViewModel
            {
                SettingsProductCapabilitiesList = new List<SelectListItem> { new SelectListItem("test1", "1", true) },
                DraftAdditionalCapabilities = draftCapabilities
            };
            var prodCapabilities = MockHtgVendorSmeDbContext.Object.product_capabilities.Where(r => r.product_id == 1 && r.capability_id == 1);

            // Act
            var result = await _sut.ProductCapabilities(input, 1, 1) as RedirectToRouteResult;

            // Assert
            AssertForDbUpdates(result, numberOfProductUpdate: 0, 2);
            MockHtgVendorSmeDbContext.Verify(r => r.product_capabilities.RemoveRange(prodCapabilities), Times.Once);
            MockHtgVendorSmeDbContext.Verify(r => r.product_capabilities.AddAsync(It.IsAny<product_capability>(), default), Times.Once);
        }

        [Fact]
        public async Task ShouldAddProductCapabilitiesWithoutSettingsProductCapabilities()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();
            var input = new ProductCapabilitiesViewModel
            {
                SettingsProductCapabilitiesList = new List<SelectListItem>(),
                DraftAdditionalCapabilities = "DraftAdditionalCapabilities"
            };
            var prodCapabilities = MockHtgVendorSmeDbContext.Object.product_capabilities.Where(r => r.product_id == 1 && r.capability_id == 1);

            // Act
            var result = await _sut.ProductCapabilities(input, 1, 1) as RedirectToRouteResult;

            // Assert
            AssertForDbUpdates(result, 1, 2);
            MockHtgVendorSmeDbContext.Verify(r => r.product_capabilities.RemoveRange(prodCapabilities), Times.Once);
            MockHtgVendorSmeDbContext.Verify(r => r.product_capabilities.AddAsync(It.IsAny<product_capability>(), default), Times.Never);
        }

        [Fact]
        public async Task ShouldAddProductCapabilitiesWithADifferentProductFilter()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();
            var input = new ProductCapabilitiesViewModel
            {
                SettingsProductCapabilitiesList = new List<SelectListItem> { new SelectListItem("test1", "2", true) },
                DraftAdditionalCapabilities = "DraftAdditionalCapabilities"
            };
            var prodCapabilities = MockHtgVendorSmeDbContext.Object.product_capabilities.Where(r => r.product_id == 1 && r.capability_id == 1);

            // Act
            var result = await _sut.ProductCapabilities(input, 1, 1) as RedirectToRouteResult;

            // Assert
            AssertForDbUpdates(result, 1, 3);
            MockHtgVendorSmeDbContext.Verify(r => r.product_capabilities.RemoveRange(prodCapabilities), Times.Once);
            MockHtgVendorSmeDbContext.Verify(r => r.product_capabilities.AddAsync(It.IsAny<product_capability>(), default), Times.Once);
        }

        [Fact]
        public async Task ShouldAddProductCapabilitiesWithoutProductCapabilities()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductCapabilities();
            SetupEmptyProductCapabilities();
            var input = new ProductCapabilitiesViewModel
            {
                SettingsProductCapabilitiesList = new List<SelectListItem> { new("test1", "2", true) },
                DraftAdditionalCapabilities = "DraftAdditionalCapabilities"
            };
            var prodCapabilities = MockHtgVendorSmeDbContext.Object.product_capabilities.Where(r => r.product_id == 1 && r.capability_id == 1);

            // Act
            var result = await _sut.ProductCapabilities(input, 1, 1) as RedirectToRouteResult;

            // Assert
            AssertForDbUpdates(result, 1, 3);
            MockHtgVendorSmeDbContext.Verify(r => r.product_capabilities.RemoveRange(prodCapabilities), Times.Once);
            MockHtgVendorSmeDbContext.Verify(r => r.product_capabilities.AddAsync(It.IsAny<product_capability>(), default), Times.Once);
        }

        [Fact]
        public async Task ShouldGetProductSupport()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductFiltersCategories();
            SetupSettingsProductFilters();
            SetupProductFilters();

            // Act
            var result = await _sut.ProductSupport(1) as ViewResult;

            // Assert
            var model = AssertGetProductSupport(result);
            model.SettingsProductFiltersCategories.Any().Should().BeTrue();
            model.SettingsProductFiltersCategories.All(r => r.SettingsProductFilters.Any()).Should().BeTrue();
            model.SettingsProductFiltersCategories.All(r => !string.IsNullOrWhiteSpace(r.ItemName)).Should().BeTrue();
            model.ProductId.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ShouldGetProductSupportWithoutSettingsProductFiltersCategories()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupEmptySettingsProductFiltersCategories();
            SetupSettingsProductFilters();
            SetupProductFilters();

            // Act
            var result = await _sut.ProductSupport(1) as ViewResult;

            // Assert
            AssertGetProductSupport(result, false);
        }

        [Fact]
        public async Task ShouldGetProductSupportWithoutProductFilters()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductFiltersCategories();
            SetupSettingsProductFilters();
            SetupEmptyProductFilters();

            // Act
            var result = await _sut.ProductSupport(1) as ViewResult;

            // Assert
            var model = AssertGetProductSupport(result);
            model.SettingsProductFiltersCategories.Any().Should().BeTrue();
            model.SettingsProductFiltersCategories.All(r => r.SettingsProductFilters.Any()).Should().BeTrue();
        }

        [Fact]
        public async Task ShouldGetProductSupportWithoutSettingsProductFilters()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductFiltersCategories();
            SetupEmptySettingsProductFilters();
            SetupProductFilters();

            // Act
            var result = await _sut.ProductSupport(1) as ViewResult;

            // Assert
            var model = AssertGetProductSupport(result);
            model.SettingsProductFiltersCategories.Any().Should().BeTrue();
            model.SettingsProductFiltersCategories.All(r => r.SettingsProductFilters.Count == 0).Should().BeTrue();
        }

        [Fact]
        public async Task ShouldAddProductSupport()
        {
            // Arrange
            SetupSettingsProductFilters();
            SetupProductFilters();
            var input = new ProductSupportViewModel
            {
                SettingsProductFiltersCategories = new List<SettingsProductFiltersCategory> { 
                    new() { SettingsProductFilters = new List<SelectListItem> { new ("test1", "1", true) } }
                },
                Adb2CId = Adb2CId2
            };
            var settingsProdFilters = MockHtgVendorSmeDbContext.Object.product_filters.Where(r => r.product_id == 1 && new List<long> { { 1 }, { 2 } }.Contains(r.filter_id));

            // Act
            var result = await _sut.ProductSupport(input, 1) as RedirectToRouteResult;

            // Assert
            AssertForDbUpdates(result, 0, 2);
            MockHtgVendorSmeDbContext.Verify(r => r.product_filters.RemoveRange(settingsProdFilters), Times.Once);
            MockHtgVendorSmeDbContext.Verify(r => r.product_filters.AddAsync(It.IsAny<product_filter>(), default), Times.Once);
        }

        [Fact]
        public async Task ShouldGetPlatformDetails()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductTypes();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();
            SetupSettingsProductFilters();
            SetupProductFilters();
            SetupSettingsProductFiltersCategories();

            // Act
            var result = await _sut.PlatformDetails(1) as ViewResult;

            // Assert
            var model = AssertGetPlatformDetails(result);
            model.SettingsProductFiltersCategory.Should().NotBeNull();
            model.SettingsProductFiltersCategory.ItemName.Should().NotBeNullOrWhiteSpace();
            model.SettingsProductFiltersCategory.SettingsProductFilters.Any().Should().BeTrue();
        }

        [Fact]
        public async Task ShouldGetPlatformDetailsWithoutSettingsProductFiltersCategories()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductTypes();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();
            SetupSettingsProductFilters();
            SetupProductFilters();
            SetupEmptySettingsProductFiltersCategories();

            // Act
            var result = await _sut.PlatformDetails(1) as ViewResult;

            // Assert
            var model = AssertGetPlatformDetails(result, false);
            model.SettingsProductFiltersCategory.Should().BeNull();
        }

        [Fact]
        public async Task ShouldGetPlatformDetailsWithoutProductFilters()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductTypes();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();
            SetupSettingsProductFilters();
            SetupEmptyProductFilters();
            SetupSettingsProductFiltersCategories();

            // Act
            var result = await _sut.PlatformDetails(1) as ViewResult;

            // Assert
            var model = AssertGetPlatformDetails(result);
            model.SettingsProductFiltersCategory.Should().NotBeNull();
            model.SettingsProductFiltersCategory.SettingsProductFilters.Any().Should().BeTrue();
        }

        [Fact]
        public async Task ShouldGetPlatformDetailsWithoutSettingsProductFilters()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductTypes();
            SetupSettingsProductCapabilities();
            SetupProductCapabilities();
            SetupEmptySettingsProductFilters();
            SetupProductFilters();
            SetupSettingsProductFiltersCategories();

            // Act
            var result = await _sut.PlatformDetails(1) as ViewResult;

            // Assert
            var model = AssertGetPlatformDetails(result);
            model.SettingsProductFiltersCategory.Should().NotBeNull();
            model.SettingsProductFiltersCategory.SettingsProductFilters.Any().Should().BeFalse();
        }

        [Fact]
        public async Task ShouldAddPlatformDetails()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupProducts();
            SetupSettingsProductFilters();
            SetupProductFilters();
            var input = new PlatformDetailsViewModel
            {
                SettingsProductFiltersCategory = new SettingsProductFiltersCategory
                {
                    SettingsProductFilters = new List<SelectListItem> { new("test1", "1", true) }
                }
            };
            var settingsProdFilters = MockHtgVendorSmeDbContext.Object.product_filters.Where(r => r.product_id == 1 && new List<long> { 3 }.Contains(r.filter_id));

            // Act
            var result = await _sut.PlatformDetails(input, 1) as RedirectToRouteResult;

            // Assert
            AssertForDbUpdates(result, 1, 3);
            MockHtgVendorSmeDbContext.Verify(r => r.product_filters.RemoveRange(settingsProdFilters), Times.Once);
            MockHtgVendorSmeDbContext.Verify(r => r.product_filters.AddAsync(It.IsAny<product_filter>(), default), Times.Once);
        }

        private static ProductSupportViewModel AssertGetProductSupport(ViewResult result, bool expectedSettingProductFiltersCategory = true)
        {
            Assert.NotNull(result);
            var model = result.Model as ProductSupportViewModel;
            Assert.NotNull(model);
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.ProductStatus.Should().BeOneOf(ProductStatus.Approved, ProductStatus.InReview, ProductStatus.Incomplete, ProductStatus.NotInScheme);
            model.SettingsProductFiltersCategories.Any().Should().Be(expectedSettingProductFiltersCategory);

            return model;
        }

        private static PlatformDetailsViewModel AssertGetPlatformDetails(ViewResult result, bool expectedSettingProductFiltersCategory = true)
        {
            Assert.NotNull(result);
            var model = result.Model as PlatformDetailsViewModel;
            Assert.NotNull(model);
            model.ProductId.Should().BeGreaterThan(0);
            model.ProductName.Should().NotBeNullOrWhiteSpace();
            model.ProductStatus.Should().BeOneOf(ProductStatus.Approved, ProductStatus.InReview, ProductStatus.Incomplete, ProductStatus.NotInScheme);
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