namespace Beis.VendorManagement.Web.Tests.Controllers
{
    public class MicrosoftIdentityControllerTests
    {

        private readonly H2GAccountController _sut;

        public MicrosoftIdentityControllerTests()
        {
            _sut = new H2GAccountController();
            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(r => r.Content(It.IsAny<string>())).Returns("/");
            _sut.Url = mockUrl.Object;
        }

        [Fact]
        public void SignInShouldSetPolicyAndChallenge()
        {
            // Arrange

            // Act
            var result = _sut.SignIn(null) as ChallengeResult;

            // Assert
            AssertChallenge(result, "B2C_1_h2g_signin");
        }

        [Fact]
        public void SignUpShouldSetPolicyAndChallenge()
        {
            // Arrange

            // Act
            var result = _sut.SignUp(null) as ChallengeResult;

            // Assert
            AssertChallenge(result, "B2C_1_h2g_signup");
        }

        private static void AssertChallenge(IActionResult actionResult, string policyName)
        {
            var result = actionResult as ChallengeResult;
            Assert.NotNull(result);
            result.AuthenticationSchemes.Count.Should().Be(1);
            result.AuthenticationSchemes.First().Should().Be(OpenIdConnectDefaults.AuthenticationScheme);
            result.Properties.RedirectUri.Should().NotBeNullOrWhiteSpace();
            result.Properties.Items.Count.Should().Be(2);
            result.Properties.Items.First().Key.Should().Be(".redirect");
            result.Properties.Items.First().Value.Should().NotBeNullOrWhiteSpace();
            result.Properties.Items.Last().Key.Should().Be("policy");
            result.Properties.Items.Last().Value.Should().Be(policyName);
        }
    }
}