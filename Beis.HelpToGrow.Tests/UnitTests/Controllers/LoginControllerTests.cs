using Beis.HelpToGrow.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Beis.HelpToGrow.Tests.UnitTests.Controllers
{
    public class LoginControllerTests
    {

        private readonly LoginController _sut;

        public LoginControllerTests()
        {
            _sut = new LoginController();
        }

        [Fact]
        public void LandingPageShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.LandingPage() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }
    }
}