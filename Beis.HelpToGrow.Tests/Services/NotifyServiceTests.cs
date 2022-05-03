using Beis.VendorManagement.Web.Services;
using FluentAssertions;
using Moq;
using Notify.Interfaces;
using System;
using Xunit;

namespace Beis.VendorManagement.Web.Tests.Services
{
    public class NotifyServiceTests
    {
        private NotifyService _sut;

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void NotificationServiceShouldThrowWhenApiKeyIsNullOtEmptyOrWhiteSpace(string apiKey)
        {
            // Arrange
            var options = Microsoft.Extensions.Options.Options.Create(new NotifyService.NotifyServiceOptions { NotifyEmailKey = apiKey });

            // Act
            Action result = () => new NotifyService(null, null, options);

            // Assert
            result.Should().Throw<ArgumentNullException>().WithMessage("Cannot create notification client as api key is null (Parameter 'notifyEmailKey')");
            _sut.Should().BeNull();
        }

        [Fact]
        public void NotificationServiceShouldNotThrowWhenApiKeyIsPassed()
        {
            // Arrange
            var options = Microsoft.Extensions.Options.Options.Create(new NotifyService.NotifyServiceOptions { NotifyEmailKey = "someKey" });

            // Act
            _sut = new NotifyService(null, new Mock<IAsyncNotificationClient>().Object, options);

            // Assert
            _sut.Should().NotBeNull();
        }
    }
}