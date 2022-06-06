namespace Beis.VendorManagement.Web.Tests.Controllers
{
    public class ManageUsersControllerTests : BaseControllerTests
    {
        private readonly ManageUsersController _sut;
        private readonly Mock<ILogger<ManageUsersController>> _logger;

        public ManageUsersControllerTests()
        {
            _logger = new Mock<ILogger<ManageUsersController>>();
            _sut = new ManageUsersController(
                ServiceProvider.GetService<IMediator>(),
                ServiceProvider.GetService<IManageUsersService>(),
                _logger.Object);

            SetHttpContext(_sut.ControllerContext);
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId1) });
        }

        [Fact]
        public async Task PrimaryUserChangeShouldLogAndRedirectToErrorWhenUserIsNull()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.PrimaryUserChange(TestNonExistingUserId) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.LoggedUserErrorGet);
            VerifyLogger();
        }

        [Fact]
        public async Task PrimaryUserChangeShouldRedirectToView()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.PrimaryUserChange(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as VendorCompanyUserViewModel;
            Assert.NotNull(model);
            model.FullName.Should().NotBeNullOrWhiteSpace();
            model.UserId.Should().BeGreaterThan(0);
            model.ContentKey.Should().Be("ManageUsers-PrimaryUserChange");
        }

        [Fact]
        public async Task ConfirmPrimaryUserChangeShouldNotSendEmailWhenUserIdIsAMismatch()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.ConfirmPrimaryUserChange(new UserViewModel { UserId = TestUserId }) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.PrimaryUserChangeGet);
            result.RouteValues.Count.Should().Be(1);
            result.RouteValues.ContainsKey(ApplicationConstants.UserId).Should().BeTrue();
            MockNotificationClient.Verify(r => r.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), default, default), Times.Never);
        }

        [Fact]
        public async Task ConfirmPrimaryUserChangeShouldSendEmail()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.ConfirmPrimaryUserChange(new UserViewModel { UserId = TestPrimaryUserId }) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.HomeIndexGet);
            MockNotificationClient.Verify(r => r.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), default, default), Times.Exactly(2));
        }

        [Fact]
        public async Task PrimaryUserChangePostShouldNotUpdatePrimaryContactAndSendEmail()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.PrimaryUserChange(TestUserId, false) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ManageUsersHomeGet);
            result.RouteValues.Should().BeNull();
        }

        [Fact]
        public async Task PrimaryUserChangePostShouldThrowErrorAndNotUpdatePrimaryContactAndSendEmail()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.PrimaryUserChange(TestUserId, null) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as VendorCompanyUserViewModel;
            Assert.NotNull(model);
            model.ShowErrorMessage.Should().BeTrue();
        }

        [Fact]
        public async Task PrimaryUserChangePostShouldUpdatePrimaryContactAndSendEmail()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.PrimaryUserChange(TestUserId, true) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.HomeIndexGet);
            MockHtgVendorSmeDbContext.Verify(r => r.SaveChangesAsync(default), Times.Once);
            MockHtgVendorSmeDbContext.Object.vendor_company_users.First(r => r.primary_contact).userid.Should().Be(TestUserId);
            MockNotificationClient.Verify(r => r.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), default, default), Times.Exactly(2));
        }

        [Fact]
        public async Task ManageUsersHomeShouldRedirectToCorrectViewWithData()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.ManageUsersHomeAsync() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as ManageUsersHomeViewModel;
            Assert.NotNull(model);
            var users = model.Users.ToList();
            Assert.NotNull(users);
            users.Any().Should().BeTrue();
            users.Single(r => r.PrimaryContact).PrimaryContact.Should().BeTrue();
            model.ContentKey.Should().Be(AnalyticConstants.ManageUsersManageUsersHome);
        }

        [Fact]
        public async Task RemoveUserShouldRedirectToCorrectView()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.RemoveUser(TestUserId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as UserViewModel;
            Assert.NotNull(model);
            model.UserId.Should().BeGreaterThan(0);
            model.FullName.Should().NotBeNullOrWhiteSpace();
            model.HasToBeRemoved.Should().BeTrue();
            model.ContentKey.Should().Be(AnalyticConstants.ManageUsersRemove);
        }

        [Fact]
        public async Task RemoveUserShouldRedirectToError()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.RemoveUser(TestNonExistingUserId) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.LoggedUserErrorGet);
            VerifyLogger();
        }

        [Fact]
        public async Task RemoveUserShouldSendEmailAndRemoveUser()
        {
            // Arrange
            SetupVendorCompanyUser();
            SetupVendorCompanies();

            // Act
            var result = await _sut.RemoveUser(TestUserId, true) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ManageUsersHomeGet);
            result.RouteValues.Should().BeNull();
            MockHtgVendorSmeDbContext.Verify(r => r.SaveChangesAsync(default), Times.Once);
            MockNotificationClient.Verify(r => r.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), default, default), Times.Once);
        }

        [Fact]
        public async Task RemoveUserShouldNotSendEmailAndRemoveUser()
        {
            // Arrange

            // Act
            var result = await _sut.RemoveUser(TestUserId, false) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ManageUsersHomeGet);
            result.RouteValues.Should().BeNull();
            MockHtgVendorSmeDbContext.Verify(r => r.SaveChangesAsync(default), Times.Never);
            MockNotificationClient.Verify(r => r.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), default, default), Times.Never);
        }

        [Fact]
        public async Task UserAsyncShouldRedirectToViewWhenUserIdIsZero() //Add user scenario
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.UserAsync(0, BackPagesEnum.ManageUsers) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as UserViewModel;
            Assert.NotNull(model);
            model.HasToBePrimaryContact.Should().BeNull();
            model.UserId.Should().Be(0);
            model.FullName.Should().BeNullOrWhiteSpace();
            model.CompanyId.Should().BeGreaterThan(0);
            model.BackPage.Should().Be(BackPagesEnum.ManageUsers);
            model.ContentKey.Should().Be(AnalyticConstants.ManageUsersAdd);
        }

        [Fact]
        public async Task UserAsyncShouldRedirectToViewWithData() //Edit user scenario
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.UserAsync(TestUserId, BackPagesEnum.ManageUsers) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as UserViewModel;
            Assert.NotNull(model);
            model.UserId.Should().BeGreaterThan(0);
            model.FullName.Should().NotBeNullOrWhiteSpace();
            model.CompanyId.Should().BeGreaterThan(0);
            model.ContentKey.Should().Be(AnalyticConstants.ManageUsersEdit);
        }

        [Fact]
        public async Task UserAsyncShouldLogErrorAndRedirectToErrorPage()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.UserAsync(TestNonExistingUserId, BackPagesEnum.ManageUsers) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.LoggedUserErrorGet);
            VerifyLogger();
        }

        [Fact]
        public async Task UserAsyncPostShouldRedirectBackToViewWhenModelStateIsInvalid()
        {
            // Arrange
            _sut.ModelState.AddModelError("FullName", string.Empty);
            _sut.ModelState.AddModelError("Email", string.Empty);

            // Act
            var result = await _sut.UserAsync(new UserViewModel()) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as UserViewModel;
            Assert.NotNull(model);
            Assert.True(_sut.ViewBag.ShowFullNameError);
            Assert.True(_sut.ViewBag.ShowEmailError);
        }

        [Fact]
        public async Task UserAsyncPostShouldRedirectBackToViewWhenModelStateIsInvalidForEdit()
        {
            // Arrange
            _sut.ModelState.AddModelError("FullName", string.Empty);

            // Act
            var result = await _sut.UserAsync(new UserViewModel()) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as UserViewModel;
            Assert.NotNull(model);
            Assert.True(_sut.ViewBag.ShowFullNameError);
            Assert.False(_sut.ViewBag.ShowEmailError);
        }

        [Fact]
        public async Task UserAsyncPostShouldNotAddAnExistingUser()
        {
            // Arrange
            SetupVendorCompanyUser();
            MockHtgVendorSmeDbContext.Object.vendor_company_users.First().email = TestUserEmailAddress;

            // Act
            var result = await _sut.UserAsync(new UserViewModel { Email = TestUserEmailAddress, CompanyId = TestCompanyId, FullName = "fullname" }) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as UserViewModel;
            Assert.NotNull(model);
            Assert.True(_sut.ViewBag.ShowEmailError);
            Assert.Null(_sut.ViewBag.ShowFullNameError);
        }

        [Fact]
        public async Task UserAsyncPostShouldAddUserWhenUserIdIsZero()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.UserAsync(new UserViewModel { Email = "testnewuser@test.com" }) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ManageUsersHomeGet);
            result.RouteValues.Should().BeNull();
            MockHtgVendorSmeDbContext.Verify(r => r.vendor_company_users.AddAsync(It.IsAny<vendor_company_user>(), default), Times.Once);
            MockHtgVendorSmeDbContext.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UserAsyncPostShouldUpdateUserWhenUserIdIsNotZero()
        {
            // Arrange
            SetupVendorCompanyUser();

            // Act
            var result = await _sut.UserAsync(
                    new UserViewModel { Email = TestUserEmailAddress, FullName = "test_fullname", UserId = TestUserId, CompanyId = TestCompanyId }) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.ManageUsersHomeGet);
            result.RouteValues.Should().BeNull();
            MockHtgVendorSmeDbContext.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        private void VerifyLogger()
        {
            _logger.Verify(x =>
                x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((m, c) => m.ToString() == "Invalid User"), null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }
    }
}