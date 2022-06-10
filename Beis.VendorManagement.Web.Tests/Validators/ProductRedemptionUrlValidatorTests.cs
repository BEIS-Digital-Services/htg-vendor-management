namespace Beis.VendorManagement.Web.Tests.Validators
{
    public class ProductRedemptionUrlValidatorTests
    {
        private readonly ProductRedemptionUrlValidator _sut;

        public ProductRedemptionUrlValidatorTests()
        {
            _sut = new ProductRedemptionUrlValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldAddErrorMessageToResultWhenRedemptionUrlIsNullOrEmptyOrWhiteSpace(string redemptionUrl)
        {
            // Arrange

            // Act
            var result = _sut.Validate(new RedemptionUrlViewModel { RedemptionUrl = redemptionUrl });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("Enter a URL, like example.com/redeem123");
        }

        [Theory]
        [InlineData("image1.bmp")]
        [InlineData("image1")]
        [InlineData("www.image1.html")]
        [InlineData("https://select*fromTable")]
        public void ShouldAddErrorMessageToResultWhenRedemptionUrlIsInValid(string redemptionUrl)
        {
            // Arrange

            // Act
            var result = _sut.Validate(new RedemptionUrlViewModel { RedemptionUrl = redemptionUrl });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("Enter a URL in the correct format, like example.com/redeem123");
        }

        [Theory]
        [InlineData("http://abc.co.uk")]
        [InlineData("http://abc.com")]
        [InlineData("https://abc.io")]
        [InlineData("https://www.abc.org")]
        public void ShouldNotAddErrorMessageToResultWhenRedemptionUrlIsInCorrectFormat(string redemptionUrl)
        {
            // Arrange

            // Act
            var result = _sut.Validate(new RedemptionUrlViewModel { RedemptionUrl = redemptionUrl });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.Errors.Count.Should().Be(0);
        }
    }
}