using Beis.VendorManagement.Web.Constants;
using Beis.VendorManagement.Web.Controllers;
using Beis.VendorManagement.Web.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Beis.VendorManagement.Web.Tests.Controllers
{
    public class ErrorControllerTests : BaseControllerTests
    {

        private readonly ErrorController _sut;

        public ErrorControllerTests()
        {
            _sut = new ErrorController();
        }

        [Fact]
        public void ErrorShouldRedirectToViewWithData()
        {
            // Arrange
            SetHttpContext(_sut.ControllerContext);
            MockHttpContext.Setup(r => r.TraceIdentifier).Returns("TraceIdentifier");
            // Act
            var result = _sut.Error() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as ErrorViewModel;
            Assert.NotNull(model);
            model.RequestId.Should().NotBeNullOrWhiteSpace();
            model.ShowRequestId.Should().BeTrue();
            model.ContentKey.Should().Be(AnalyticConstants.Error);
        }

        [Theory]
        [InlineData("Staging")]
        [InlineData("Production")]
        public void ErrorLocalDevelopmentShouldThrowWhenEnvironmentIsNotDevelopment(string environment)
        {
            // Arrange
            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.Setup(r => r.EnvironmentName).Returns(environment);

            // Act
            Action result = () => _sut.ErrorLocalDevelopment(webHostEnvironment.Object);

            // Assert
            result.Should().Throw<InvalidOperationException>().WithMessage("This shouldn't be invoked in non-development environments.");
        }

        [Fact]
        public void ErrorLocalDevelopmentShouldReturnProblemWhenEnvironmentIsDevelopment()
        {
            // Arrange
            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.Setup(r => r.EnvironmentName).Returns(Environments.Development);
            var mockExceptionHandlerFeature = new Mock<IExceptionHandlerFeature>();
            mockExceptionHandlerFeature.Setup(r => r.Error).Returns(new Exception("error message"));
            var mockFeatureCollection = new Mock<IFeatureCollection>();
            mockFeatureCollection.Setup(r => r.Get<IExceptionHandlerFeature>()).Returns(mockExceptionHandlerFeature.Object);
            _sut.ControllerContext.HttpContext = new DefaultHttpContext(mockFeatureCollection.Object);
            _sut.ProblemDetailsFactory = new MockProblemDetailsFactory();

            // Act
            var result = _sut.ErrorLocalDevelopment(webHostEnvironment.Object) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            var problemDetails = result.Value as ProblemDetails;
            Assert.NotNull(problemDetails);
            problemDetails.Title.Should().Be("error message");
        }

        [Fact]
        public void LoggedUserShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.LoggedUser() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void InvalidCompanyShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.InvalidCompany() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ActivatedUserShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.ActivatedUser() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ProductShouldRedirectToView()
        {
            // Arrange

            // Act
            var result = _sut.Product() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        public class MockProblemDetailsFactory : ProblemDetailsFactory
        {
            public override ProblemDetails CreateProblemDetails(HttpContext httpContext,
                int? statusCode = default, string title = default,
                string type = default, string detail = default, string instance = default)
            {
                return new ProblemDetails
                {
                    Detail = detail,
                    Instance = instance,
                    Status = statusCode,
                    Title = title,
                    Type = type,
                };
            }

            public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext,
                ModelStateDictionary modelStateDictionary, int? statusCode = default,
                string title = default, string type = default, string detail = default,
                string instance = default)
            {
                return new ValidationProblemDetails(new Dictionary<string, string[]>())
                {
                    Detail = detail,
                    Instance = instance,
                    Status = statusCode,
                    Title = title,
                    Type = type,
                };
            }
        }
    }
}