using Beis.VendorManagement.Web.Constants;
using Beis.VendorManagement.Web.Controllers;
using Beis.VendorManagement.Web.Models;
using Beis.VendorManagement.Web.Services.Interface;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Beis.VendorManagement.Web.Tests.Controllers
{
    public class ActivateAccountControllerTests : BaseControllerTests
    {

        private readonly ActivateAccount _sut;
        private readonly Mock<ILogger<ActivateAccount>> _logger;

        public ActivateAccountControllerTests()
        {
            _logger = new Mock<ILogger<ActivateAccount>>();
            _sut = new ActivateAccount(ServiceProvider.GetService<IActivateAccountService>(), _logger.Object);

            SetHttpContext(_sut.ControllerContext);
        }

        [Fact]
        public async Task CheckCompanyDetailsAsyncShouldRedirectToSignOutForAuthenticatedUser()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetUserIdentity(Enumerable.Empty<Claim>());

            // Act
            var result = await _sut.CheckCompanyDetailsAsync("access_link") as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            result.ActionName.Should().Be("SignOut");
            result.ControllerName.Should().Be("Account");
            result.RouteValues.Count.Should().Be(1);
            result.RouteValues.ContainsKey("area").Should().BeTrue();
            result.RouteValues.Values.First().Should().Be("MicrosoftIdentity");
        }

        [Fact]
        public async Task CheckCompanyDetailsAsyncShouldLogErrorAndRedirectToActivatedUser()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetUserIdentity(Enumerable.Empty<Claim>(), false);

            // Act
            var result = await _sut.CheckCompanyDetailsAsync("access_link") as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ActivatedUserErrorGet);
            _logger.Verify(x =>
                x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((m, c) => m.ToString() == "There is not an user with that guid: access_link"), null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task CheckCompanyDetailsAsyncShouldRedirectToViewWithData()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupVendorCompanies();
            SetUserIdentity(Enumerable.Empty<Claim>(), false);
            var tempData = new Mock<ITempDataDictionary>();
            _sut.TempData = tempData.Object;

            // Act
            var result = await _sut.CheckCompanyDetailsAsync(MockHtgVendorSmeDbContext.Object.vendor_company_users.First().access_link) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as VendorCompanyViewModel;
            Assert.NotNull(model);
            model.VendorCompanyName.Should().NotBeNullOrWhiteSpace();
            model.RegistrationId.Should().NotBeNullOrWhiteSpace();
            model.VendorCompanyAddress1.Should().NotBeNullOrWhiteSpace();
            model.VendorCompanyCity.Should().NotBeNullOrWhiteSpace();
            model.VendorCompanyPostcode.Should().NotBeNullOrWhiteSpace();
            model.PrimaryUserId.Should().BeGreaterThan(0);
            model.ShowErrorMessage.Should().BeFalse();
            model.ContentKey.Should().Be("ActivateAccount-CheckCompanyDetails");
        }

        [Fact]
        public async Task CheckCompanyDetailsAsyncPostShouldRedirectBackToViewWhenUserHasNotAuthorized()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupVendorCompanies();

            // Act
            var result = await _sut.CheckCompanyDetailsAsync(TestUserId, false) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as VendorCompanyViewModel;
            Assert.NotNull(model);
            model.VendorCompanyName.Should().NotBeNullOrWhiteSpace();
            model.RegistrationId.Should().NotBeNullOrWhiteSpace();
            model.VendorCompanyAddress1.Should().NotBeNullOrWhiteSpace();
            model.VendorCompanyCity.Should().NotBeNullOrWhiteSpace();
            model.VendorCompanyPostcode.Should().NotBeNullOrWhiteSpace();
            model.PrimaryUserId.Should().BeGreaterThan(0);
            model.HasUserAuthorised.Should().BeFalse();
            model.ShowErrorMessage.Should().BeTrue();
        }

        [Fact]
        public async Task CheckCompanyDetailsAsyncPostShouldRedirectToTermsAndConditions()
        {
            // Arrange

            // Act
            var result = await _sut.CheckCompanyDetailsAsync(TestUserId, true) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.TermsAndConditionsGet);
            result.RouteValues.Count.Should().Be(1);
            result.RouteValues.ContainsKey("id").Should().BeTrue();
            result.RouteValues.Values.First().Should().Be(TestUserId);
        }

        [Fact]
        public async Task TermsAndConditionsAsyncShouldRedirectToViewWithData()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.TermsAndConditionsAsync(TestUserId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as VendorCompanyUserViewModel;
            Assert.NotNull(model);
            model.ContentKey.Should().Be("ActivateAccount-TermsAndConditions");
        }

        [Fact]
        public async Task TermsAndConditionsAsyncPostShouldRedirectToSignUpH2GAccount()
        {
            // Arrange

            // Act
            var result = await _sut.TermsAndConditionsAsync(TestUserId, true, true) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            result.ActionName.Should().Be("SignUp");
            result.ControllerName.Should().Be("H2GAccount");
            result.RouteValues.Count.Should().Be(1);
            result.RouteValues.ContainsKey("area").Should().BeTrue();
            result.RouteValues.Values.First().Should().Be("MicrosoftIdentity");
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task TermsAndConditionsAsyncPostShouldRedirectBackToViewWhenConditionsNotMet(bool hasTermsChecked, bool hasPrivacyPolicyChecked)
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.TermsAndConditionsAsync(TestUserId, hasTermsChecked, hasPrivacyPolicyChecked) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as VendorCompanyUserViewModel;
            Assert.NotNull(model);
            model.ShowErrorMessage.Should().BeTrue();
            model.HasTermsChecked.Should().Be(hasTermsChecked);
            model.HasPrivacyPolicyChecked.Should().Be(hasPrivacyPolicyChecked);
        }


        [Fact]
        public void TermsOfUseShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.TermsOfUse() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void PrivacyPolicyShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.PrivacyPolicy() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }
    }
}