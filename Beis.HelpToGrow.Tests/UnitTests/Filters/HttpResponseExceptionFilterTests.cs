using Beis.HelpToGrow.Web.Filters;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using Xunit;

namespace Beis.HelpToGrow.Tests.UnitTests.Filters
{
    public class HttpResponseExceptionFilterTests
    {
        private readonly HttpResponseExceptionFilter _sut;

        public HttpResponseExceptionFilterTests()
        {
            _sut = new HttpResponseExceptionFilter();
        }

        [Fact]
        public void ShouldGetTheFilterOrder()
        {
            // Arrange

            // Act

            //Assert
            _sut.Order.Should().Be(int.MaxValue - 10);
        }

        [Fact]
        public void OnActionExecutingShouldDoNothing()
        {
            // Arrange
            var context = new ActionExecutingContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>(),
                new Dictionary<string, object> { { "controller", "action" } }, null);

            // Act
            _sut.OnActionExecuting(context);

            //Assert
            context.Result.Should().BeNull();
        }

        [Fact]
        public void OnActionExecutedShouldCheckForHttpResponseException()
        {
            // Arrange
            var context = new ActionExecutedContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>(),
                null)
            {
                Exception = new HttpResponseException
                {
                    Value = "value",
                    Status = 500
                }
            };

            // Act
            _sut.OnActionExecuted(context);

            //Assert
            context.ExceptionHandled.Should().BeTrue();
            var result = context.Result as ObjectResult;
            Assert.NotNull(result);
            result.StatusCode.Should().Be(500);
        }
    }
}