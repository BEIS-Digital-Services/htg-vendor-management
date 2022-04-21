using Beis.HelpToGrow.Web.Models;
using Beis.HelpToGrow.Web.Validators;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Beis.HelpToGrow.Tests.UnitTests.Validators
{
    public class IpAddressValidatorTests
    {
        private readonly IpAddressValidator _sut;

        public IpAddressValidatorTests()
        {
            _sut = new IpAddressValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidIpAddressesShouldAddErrorMessageToResult(string ipAddresses)
        {
            // Arrange

            // Act
            var result = _sut.Validate(new RangesViewModel { IpAddresses = ipAddresses });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("Please enter valid Ip Addresses");
        }

        [Fact]
        public void CheckMaxLengthForIpAddressesShouldAddErrorMessageToResult()
        {
            // Arrange
            var ipAddresses = new string('1', 501);

            // Act
            var result = _sut.Validate(new RangesViewModel { IpAddresses = ipAddresses });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("Ip Addresses can have a maximum of 500 characters");
        }

        [Fact]
        public void CheckDuplicateIpAddressesShouldAddErrorMessageToResult()
        {
            // Arrange

            // Act
            var result = _sut.Validate(new RangesViewModel { IpAddresses = "192.168.1.1;192.168.1.1" });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("Please enter different Ip Addresses");
        }

        [Theory]
        [InlineData("192.168.1/34")]
        [InlineData("192.168.1.1;192.168.1.2;192.168.1")]
        [InlineData("300.400.500.600")]
        public void CheckIpAddressesFormatShouldAddErrorMessageToResult(string ipAddresses)
        {
            // Arrange

            // Act
            var result = _sut.Validate(new RangesViewModel { IpAddresses = ipAddresses });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("Please enter valid Ip Addresses");
        }

        [Fact]
        public void CheckIpAddressesFormatShouldNotAddErrorMessageToResult()
        {
            // Arrange

            // Act
            var result = _sut.Validate(new RangesViewModel { IpAddresses = "192.168.1.1;192.168.1.10" });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.Errors.Count.Should().Be(0);
        }
    }
}
