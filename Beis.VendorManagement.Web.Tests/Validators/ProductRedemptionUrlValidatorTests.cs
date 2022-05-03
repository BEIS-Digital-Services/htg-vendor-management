using Beis.VendorManagement.Web.Models;
using Beis.VendorManagement.Web.Validators;
using FluentAssertions;
using System.Linq;
using Xunit;

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
            result.Errors.First().ErrorMessage.Should().Be("Enter a URL, like www.example.com/redeem123");
        }

        [Theory]
        [InlineData("image1.bmp")]
        [InlineData("image1")]
        [InlineData("www.image1.html")]
        [InlineData("http://www.abc.jepg")]
        public void ShouldAddErrorMessageToResultWhenRedemptionUrlIsInValid(string redemptionUrl)
        {
            // Arrange

            // Act
            var result = _sut.Validate(new RedemptionUrlViewModel { RedemptionUrl = redemptionUrl });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("Enter a URL in the correct format, like www.example.com/redeem123");
        }
    }
}