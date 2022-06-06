namespace Beis.VendorManagement.Web.Tests.Controllers
{
    public class HelpAndGuidanceControllerTests
    {

        private readonly HelpAndGuidanceController _sut;

        public HelpAndGuidanceControllerTests()
        {
            _sut = new HelpAndGuidanceController();
        }

        [Fact]
        public void VendorPortalActivationShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.VendorPortalActivation() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ProductInformationShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.ProductInformation() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void PricingShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.Pricing() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ProductComparisonToolShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.ProductComparisonTool() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void TechnicalSolutionsShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.TechnicalSolutions() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfiguringYourApiShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.ConfiguringYourApi() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ApiStandardsShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.ApiStandards() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ContactUsShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.ContactUs() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }
    }
}