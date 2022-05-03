using Beis.VendorManagement.Web.Models;
using Beis.VendorManagement.Web.Validators;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Beis.VendorManagement.Web.Tests.Validators
{
    public class UserValidatorTests
    {
        private const string ErrorMessage = "Enter an email address in the correct format, like name@example.com";

        private readonly UserValidator _sut;

        public UserValidatorTests()
        {
            _sut = new UserValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldCheckForNullOrEmptyOrWhiteSpace(string input)
        {
            // Arrange

            // Act
            var result = _sut.Validate(new UserViewModel { FullName = input, Email = input });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().BeGreaterThan(1);
            result.Errors.Any(r => r.ErrorMessage == "Enter your full name").Should().BeTrue();
            result.Errors.Any(r => r.ErrorMessage == "Enter an email address, like name@example.com").Should().BeTrue();
        }

        [Fact]
        public void ShouldCheckForNameMaxLengthAndAddErrorMessageToResult()
        {
            // Arrange

            // Act
            var result = _sut.Validate(new UserViewModel { Email = "test@test.com", FullName = "fullName. fullName, fullName fullName' -n" });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("Name must be 40 characters or fewer");
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("\"`¬!£$%^&*()_=+[{]};:@#~\\|<.>?/")]
        [InlineData("12345\"`¬!£$%^&*()_=+[{]};:@#~\\|<.>?/")]
        [InlineData("<script>alert('Inject')</script>")]
        public void ShouldCheckForNameFormatAndAddErrorMessageToResult(string fullName)
        {
            // Arrange

            // Act
            var result = _sut.Validate(new UserViewModel { FullName = fullName, Email = "test@test.com" });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("Name can only include letters and the characters space, full stop, comma, apostrophe, space or dash");
        }

        [Theory]
        [InlineData("abc@efg.com", true, 0)]
        [InlineData("123abc@efg.com", true, 0)]
        [InlineData("abc123@efg.com", true, 0)]
        [InlineData("123@123.com", true, 0)]
        [InlineData("abc@efg.co.uk", true, 0)]
        [InlineData("abc@d@efh.com", false, 1, ErrorMessage)]
        [InlineData("abc@efg", false, 1, ErrorMessage)]
        [InlineData("abc@.com", false, 1, ErrorMessage)]
        [InlineData("abc@.", false, 1, ErrorMessage)]
        [InlineData("abc-efg.com", false, 1, ErrorMessage)]
        public void ShouldCheckForEmailAddressFormatAddErrorMessageToResult(string emailAddress, bool expected, int expectedErrorCount, string expectedErrorMessage = null)
        {
            // Arrange

            // Act
            var result = _sut.Validate(new UserViewModel { FullName = "fullName", Email = emailAddress });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().Be(expected);
            result.Errors.Count.Should().Be(expectedErrorCount);
            result.Errors.FirstOrDefault()?.ErrorMessage.Should().Be(expectedErrorMessage);
        }
    }
}