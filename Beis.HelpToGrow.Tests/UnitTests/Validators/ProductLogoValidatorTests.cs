using Beis.HelpToGrow.Web.Models;
using Beis.HelpToGrow.Web.Validators;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Linq;
using Xunit;

namespace Beis.HelpToGrow.Tests.UnitTests.Validators
{
    public class ProductLogoValidatorTests
    {
        private readonly ProductLogoValidator _sut;

        public ProductLogoValidatorTests()
        {
            _sut = new ProductLogoValidator();
        }

        [Fact]
        public void ShouldAddErrorMessageToResultWhenFileIsNull()
        {
            // Arrange

            // Act
            var result = _sut.Validate(new ProductLogoViewModel());

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("Please choose an appropriate file");
        }

        [Theory]
        [InlineData("testfile.png", "image/png")]
        [InlineData("testfile.html", "text/html")]
        [InlineData("testfile", "image/bmp")]
        [InlineData("testfile.bmp", "image")]
        public void ShouldCheckForValidFileTypeAndAddErrorMessageToResult(string fileNameWithExtension, string fileContentType)
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(r => r.FileName).Returns(fileNameWithExtension);
            mockFile.Setup(r => r.ContentType).Returns(fileContentType);

            // Act
            var result = _sut.Validate(new ProductLogoViewModel { File = mockFile.Object });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("The selected file must be a JPEG, GIF or BMP");
        }

        [Fact]
        public void ShouldCheckForFileEmptyAndAddErrorMessageToResult()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(r => r.FileName).Returns("TestFile.jpeg");
            mockFile.Setup(r => r.ContentType).Returns("image/jpeg");

            // Act
            var result = _sut.Validate(new ProductLogoViewModel { File = mockFile.Object });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("The selected file is empty");
        }

        [Fact]
        public void ShouldCheckForFileSizeLimitAndAddErrorMessageToResult()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(r => r.FileName).Returns("TestFile.jpeg");
            mockFile.Setup(r => r.ContentType).Returns("image/jpeg");
            mockFile.Setup(r => r.Length).Returns(256001);

            // Act
            var result = _sut.Validate(new ProductLogoViewModel { File = mockFile.Object });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("The selected file must be smaller than 250Kb");
        }
    }
}