namespace Beis.VendorManagement.Web.Tests.Validators
{
    public class ProductSummaryValidatorTests
    {
        private readonly ProductSummaryValidator _sut;

        public ProductSummaryValidatorTests()
        {
            _sut = new ProductSummaryValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldCheckForNullOrEmptyOrWhiteSpace(string draftProductDescription)
        {
            // Arrange

            // Act
            var result = _sut.Validate(new SummaryViewModel { DraftProductDescription = draftProductDescription });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("Enter product summary");
        }

        [Fact]
        public void ShouldCheckForNameMaxLengthAndAddErrorMessageToResult()
        {
            // Arrange
            var draftProductDescription = new string('A', 5001);

            // Act
            var result = _sut.Validate(new SummaryViewModel { DraftProductDescription = draftProductDescription });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("Summary must be 5000 characters or fewer");
        }

        [Theory]
        [InlineData("http://www.abc.com")]
        [InlineData("image1.bmp")]
        [InlineData("image1.html")]
        public void ShouldCheckForFormatAndAddErrorMessageToResult(string draftProductDescription)
        {
            // Arrange

            // Act
            var result = _sut.Validate(new SummaryViewModel { DraftProductDescription = draftProductDescription });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("Summary can not contains links to web pages, downloads or images");
        }
    }
}