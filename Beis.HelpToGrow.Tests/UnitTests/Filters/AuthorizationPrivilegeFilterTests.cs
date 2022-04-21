using Beis.HelpToGrow.Tests.UnitTests.Controllers;
using Beis.HelpToGrow.Web.Constants;
using Beis.HelpToGrow.Web.Filters;
using Beis.HelpToGrow.Web.Models;
using Beis.HelpToGrow.Web.Services.Interface;
using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Beis.HelpToGrow.Tests.UnitTests.Filters
{
    public class AuthorizationPrivilegeFilterTests : BaseControllerTests
    {
        private readonly AuthorizationPrivilegeFilter _sut;
        private readonly ActionExecutingContext _actionExecutingContext;
        private readonly ActionExecutionDelegate _next;

        public AuthorizationPrivilegeFilterTests()
        {
            _sut = new AuthorizationPrivilegeFilter(ServiceProvider.GetService<IManageUsersService>());
            var controller = new TestController();
            
            SetHttpContext(controller.ControllerContext);
            SetUserIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Adb2CId1) });
         
            var actionContext = new ActionContext(MockHttpContext.Object, new RouteData(), new ControllerActionDescriptor { ControllerName = "ManageUsers", ActionName = "SomeAction" });

            _actionExecutingContext = new ActionExecutingContext(
                actionContext, 
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(), controller);

            _next = () => Task.FromResult(new ActionExecutedContext(
                new ActionContext(MockHttpContext.Object, new RouteData(), new ControllerActionDescriptor { ControllerName = "TestController", ActionName = "TestAction" })
                , new List<IFilterMetadata>(), controller));
        }

        [Fact]
        public async Task OnActionExecutingShouldReturnForUnAuthenticatedUser()
        {
            //Arrange
            SetUserIdentity(Enumerable.Empty<Claim>(), false);

            //Act
            await _sut.OnActionExecutionAsync(_actionExecutingContext, _next);

            //Assert
            _actionExecutingContext.Result.Should().BeNull();
        }

        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        public async Task OnActionExecutionAsyncShouldSetResultToErrorPageWhenCompanyDoesNotMatchBasedOnUserId(long currentUserId)
        {
            // Arrange
            SetupVendorCompanyUser();

            UpdateContext("userId", currentUserId);

            // Act
            await _sut.OnActionExecutionAsync(_actionExecutingContext, _next);

            // Assert
            var result = _actionExecutingContext.Result as RedirectToRouteResult;
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.InvalidCompanyErrorGet);
            result.RouteValues.Should().BeEmpty();
        }

        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        public async Task OnActionExecutionAsyncShouldSetResultToErrorPageWhenCompanyDoesNotMatchBasedOnUser(long currentUserId)
        {
            // Arrange
            SetupVendorCompanyUser();

            UpdateContext("user", new UserViewModel { UserId = currentUserId });

            // Act
            await _sut.OnActionExecutionAsync(_actionExecutingContext, _next);

            // Assert
            var result = _actionExecutingContext.Result as RedirectToRouteResult;
            Assert.NotNull(result);
            result.RouteName.Should().Be(RouteNameConstants.InvalidCompanyErrorGet);
            result.RouteValues.Should().BeEmpty();
        }

        [Fact]
        public async Task OnActionExecutionAsyncShouldCallNextWhenCompanyMatch()
        {
            // Arrange
            SetupVendorCompanyUser();
            UpdateContext("userId", 1);

            // Act
            await _sut.OnActionExecutionAsync(_actionExecutingContext, _next);

            // Assert
            _actionExecutingContext.Result.Should().BeNull();
        }

        [Fact]
        public async Task OnActionExecutionAsyncShouldCallNextWhenUserIdIsZero()
        {
            // Arrange
            SetupVendorCompanyUser();
            UpdateContext("userId", 0);

            // Act
            await _sut.OnActionExecutionAsync(_actionExecutingContext, _next);

            // Assert
            _actionExecutingContext.Result.Should().BeNull();
        }

        private void UpdateContext(string actionArgumentName, object actionArgumentValue)
        {
            _actionExecutingContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(Enumerable.Empty<Claim>(), "basic"));
            _actionExecutingContext.ActionArguments.Add(actionArgumentName, actionArgumentValue);
        }
    }

    public class TestController : Controller
    {
    }
}