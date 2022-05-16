using Beis.Htg.VendorSme.Database.Models;
using Beis.VendorManagement.Web.Constants;
using Beis.VendorManagement.Web.Controllers;
using Beis.VendorManagement.Web.Models;
using Beis.VendorManagement.Web.Services.Interface;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Beis.VendorManagement.Web.Tests.Controllers
{
    public class HomeControllerTests : BaseControllerTests
    {

        private readonly HomeController _sut;
        private readonly Mock<ITempDataDictionary> _mockTempData;

        public HomeControllerTests()
        {
            _sut = new HomeController(
                ServiceProvider.GetService<IAccountHomeService>(),
                ServiceProvider.GetService<IMediator>());

            SetHttpContext(_sut.ControllerContext);
            _mockTempData = new Mock<ITempDataDictionary>();
            _sut.TempData = _mockTempData.Object;
        }

        [Fact]
        public async Task IndexAsyncShouldRedirectToViewWithoutDataWhenUserIsNotAuthenticated()
        {
            // Arrange
            SetUserIdentity(Enumerable.Empty<Claim>(), false);

            // Act
            var result = await _sut.IndexAsync() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as AccountHomeViewModel;
            Assert.Null(model);
        }

        [Fact]
        public async Task IndexAsyncShouldLogAndRedirectToErrorWhenAdb2CUserIdIsNull()
        {
            // Arrange
            SetUserIdentity(Enumerable.Empty<Claim>());

            // Act
            var result = await _sut.IndexAsync() as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductErrorGet);
            VerifyLogger("The Microsoft ADB2C user doesn't have id. NameIdentifier is missing");
        }

        [Fact]
        public async Task IndexAsyncShouldLogAndRedirectToErrorWhenAdb2CUserEmailIsNull()
        {
            // Arrange
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, TestUserId.ToString()) });

            // Act
            var result = await _sut.IndexAsync() as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductErrorGet);
            VerifyLogger("The Microsoft ADB2C user doesn't have email. Email is missing");
        }

        [Fact]
        public async Task IndexAsyncShouldLogAndRedirectToErrorWhenUserIsNull()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetUserIdentity(new List<Claim> { { new(ClaimTypes.NameIdentifier, TestUserId.ToString()) }, { new("emails", TestUserEmailAddress) } });
            _sut.TempData = _mockTempData.Object;

            // Act
            var result = await _sut.IndexAsync() as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ProductErrorGet);
            VerifyLogger($"There is not an user in the database for the logged Microsoft ADB2C account id: {TestUserId}");
        }

        [Fact]
        public async Task IndexAsyncShouldUpdateVendorCompanyUserAndSecretAndRedirectToViewWithDataWhenAbd2CUserIdExist()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupVendorCompanies();
            SetupProducts();
            SetupSettingsProductTypes();
            MockHtgVendorSmeDbContext.Object.vendor_companies.First().access_secret = string.Empty;
            SetUserIdentity(new List<Claim> { { new(ClaimTypes.NameIdentifier, MockHtgVendorSmeDbContext.Object.vendor_company_users.First().adb2c) }, { new("emails", TestUserEmailAddress) } });
            _mockTempData.Setup(r => r["ACCESS_LINK_GUID"]).Returns(MockHtgVendorSmeDbContext.Object.vendor_company_users.First().access_link);
            _sut.TempData = _mockTempData.Object;

            // Act
            var result = await _sut.IndexAsync() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as AccountHomeViewModel;
            Assert.NotNull(model);
            model.RegistrationNumber.Should().NotBeNullOrWhiteSpace();
            model.CompanyId.Should().BeGreaterThan(0);
            model.CompanyName.Should().NotBeNullOrWhiteSpace();
            model.ApiKey.Should().NotBeNullOrWhiteSpace();
            model.Products.Any().Should().BeTrue();
            model.Products.Any(r => !string.IsNullOrWhiteSpace(r.Product.ProductName) && r.Product.ProductId > 0).Should().BeTrue();
            model.Products.Any(r => !string.IsNullOrWhiteSpace(r.TypeName)).Should().BeTrue();
            MockHtgVendorSmeDbContext.Verify(r => r.vendor_companies.Update(It.IsAny<vendor_company>()), Times.Once);
            MockHtgVendorSmeDbContext.Verify(r => r.SaveChangesAsync(default), Times.Exactly(2));
            model.ContentKey.Should().Be("Home-Index");
        }

        [Fact]
        public async Task IndexAsyncShouldUpdateCompanySecretAndRedirectToViewWithDataWhenAbd2CUserIdDoesNotExist()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupVendorCompanies();
            SetupProducts();
            SetupSettingsProductTypes();
            MockHtgVendorSmeDbContext.Object.vendor_companies.First().access_secret = string.Empty;
            SetUserIdentity(new List<Claim> { { new(ClaimTypes.NameIdentifier, MockHtgVendorSmeDbContext.Object.vendor_company_users.First().adb2c) }, { new("emails", TestUserEmailAddress) } });
            _sut.TempData = _mockTempData.Object;

            // Act
            var result = await _sut.IndexAsync() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as AccountHomeViewModel;
            Assert.NotNull(model);
            MockHtgVendorSmeDbContext.Verify(r => r.vendor_companies.Update(It.IsAny<vendor_company>()), Times.Once);
            MockHtgVendorSmeDbContext.Verify(r => r.SaveChangesAsync(default), Times.Once);
            model.ContentKey.Should().Be("Home-Index");
        }

        [Theory]
        [InlineData(ClaimTypes.NameIdentifier, false)]
        [InlineData(ClaimTypes.Email, true)]
        public async Task RangesShouldRedirectToViewWithData(string claimType, bool expected)
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupVendorCompanies();
            SetUserIdentity(new List<Claim> { new (claimType, Adb2CId1) });

            // Act
            var result = await _sut.Ranges() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as RangesViewModel;
            Assert.NotNull(model);
            string.IsNullOrWhiteSpace(model.IpAddresses).Should().Be(expected);
            model.ContentKey.Should().Be("Home-Ranges");
        }

        [Fact]
        public async Task SaveRangesShouldRedirectBackToViewWhenModelStateIsInvalid()
        {
            // Arrange
            _sut.ModelState.AddModelError("ipaddresses", string.Empty);

            // Act
            var result = await _sut.Ranges(new RangesViewModel()) as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(12345)]
        [InlineData(123456)]
        public async Task SaveRangesShouldRedirectBackToViewWhenIpAddressesIsNotSaved(long companyId)
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupVendorCompanies();
            MockHtgVendorSmeDbContext.Object.vendor_companies.First().vendorid = companyId;

            // Act
            var result = await _sut.Ranges(new RangesViewModel { IpAddresses = TestIpAddresses }) as ViewResult;

            // Assert
            Assert.NotNull(result);
            _sut.ModelState.Values
                .All(r => r.Errors.First().ErrorMessage == "Unable to save ip addresses. Please try again later.")
                .Should().BeTrue();
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("192.168.1.100")]
        public async Task SaveRangesShouldRedirectHomeIndexOnSuccess(string ipAddresses)
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupVendorCompanies();
            MockHtgVendorSmeDbContext.Object.vendor_companies.First().ipaddresses = ipAddresses;

            // Act
            var result = await _sut.Ranges(new RangesViewModel { Adb2CId = Adb2CId2, IpAddresses = TestIpAddresses }) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.HomeIndexGet);
            MockHtgVendorSmeDbContext.Verify(r => r.vendor_companies.Update(It.IsAny<vendor_company>()), Times.Once);
            MockHtgVendorSmeDbContext.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public void AccessibilityStatementShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.AccessibilityStatement() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as string;
            Assert.NotNull(model);
        }

        private void VerifyLogger(string message)
        {
            MockIndexGetLogger.Verify(x =>
                x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((m, c) => m.ToString() == message), null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }
    }
}