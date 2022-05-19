using Beis.VendorManagement.Web.Constants;
using Beis.VendorManagement.Web.Handlers.ManageUsers;
using Beis.VendorManagement.Web.Models;
using Beis.VendorManagement.Web.Models.Enums;
using Beis.VendorManagement.Web.Services.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Web.Controllers
{
    [Authorize]
    public class ManageUsersController : Controller
    {
        private readonly IManageUsersService _manageUsersService;
        private readonly ILogger<ManageUsersController> _logger;
        private readonly IMediator _mediator;

        public ManageUsersController(IMediator mediator, IManageUsersService manageUsersService, ILogger<ManageUsersController> logger)
        {
            _mediator = mediator;
            _manageUsersService = manageUsersService;
            _logger = logger;
        }

        [HttpGet("ManageUsers/ManageUsersHome", Name = RouteNameConstants.ManageUsersHomeGet)]
        public async Task<ActionResult> ManageUsersHomeAsync()
        {
            var result = await _mediator.Send(new ManageUsersHomeGetHandler.Context { Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value });
            return View(result.Value);
        }

        [HttpGet("ManageUsers/User", Name = RouteNameConstants.UserGet)]
        public async Task<ActionResult> UserAsync(long userId, BackPagesEnum pagesEnum)
        {
            if (userId <= 0)
            {
                var loggedInUser = await _manageUsersService.GetUser(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return View(new UserViewModel
                {
                    BackPage = pagesEnum,
                    CompanyId = loggedInUser.CompanyId,
                    ContentKey = AnalyticConstants.ManageUsersAdd
                });
            }
            
            var existingUser = await _manageUsersService.GetUser(userId);
            if (existingUser != null)
            {
                existingUser.ContentKey = AnalyticConstants.ManageUsersEdit;
                return View(existingUser);
            }

            _logger.LogError("Invalid User");
            return RedirectToRoute(RouteNameConstants.LoggedUserErrorGet);
        }

        [HttpPost("ManageUsers/User", Name = RouteNameConstants.UserPost)]
        public async Task<IActionResult> UserAsync(UserViewModel user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ShowFullNameError = ModelState[nameof(user.FullName)]?.ValidationState == ModelValidationState.Invalid;
                ViewBag.ShowEmailError = ModelState[nameof(user.Email)]?.ValidationState == ModelValidationState.Invalid;
                return View(user);
            }

            var result = await _mediator.Send(new UserPostHandler.Context 
                { UserId = user.UserId, CompanyId = user.CompanyId, Email = user.Email, FullName = user.FullName });
            if (result)
            {
                return RedirectToRoute(RouteNameConstants.ManageUsersHomeGet);
            }

            ModelState.AddModelError(nameof(user.Email), $"The user with email {user.Email.Trim()} already exists");
            ViewBag.ShowEmailError = true;
            return View(user);
        }

        [HttpGet("ManageUsers/RemoveUser", Name = RouteNameConstants.RemoveUserGet)]
        public async Task<ActionResult> RemoveUser(long userId)
        {
            var existingUser = await _manageUsersService.GetUser(userId);
            if (existingUser != null)
            {
                existingUser.ContentKey = AnalyticConstants.ManageUsersRemove;
                return View(existingUser);
            }

            _logger.LogError("Invalid User");
            return RedirectToRoute(RouteNameConstants.LoggedUserErrorGet);
        }

        [HttpPost("ManageUsers/RemoveUser", Name = RouteNameConstants.RemoveUserPost)]
        public async Task<ActionResult> RemoveUser(long userId, bool hasToBeRemoved)
        {
            if (hasToBeRemoved)
            {
                await _mediator.Send(new RemoveUserPostHandler.Context { CurrentUserId = userId });
            }

            return RedirectToRoute(RouteNameConstants.ManageUsersHomeGet);
        }

        [HttpPost("ManageUsers/ConfirmPrimaryUserChange", Name = RouteNameConstants.ConfirmPrimaryUserChangePost)]
        public async Task<ActionResult> ConfirmPrimaryUserChange(UserViewModel userViewModel)
        {
            var result = await _mediator.Send(new ConfirmPrimaryUserChangePostHandler.Context 
                { Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value, ChangeUserId = userViewModel.UserId });

            return result.PrimaryContactSameAsChangeUserId ? RedirectToRoute(RouteNameConstants.HomeIndexGet) 
                : RedirectToRoute(RouteNameConstants.PrimaryUserChangeGet, new { userId = userViewModel.UserId });
        }

        [HttpGet("ManageUsers/PrimaryUserChange", Name = RouteNameConstants.PrimaryUserChangeGet)]
        public async Task<ActionResult> PrimaryUserChange(long userId)
        {
            var existingUser = await _manageUsersService.GetUser(userId);
            if (existingUser != null)
            {
                existingUser.ContentKey = "ManageUsers-PrimaryUserChange";
                return View(existingUser);
            }

            _logger.LogError("Invalid User");
            return RedirectToRoute(RouteNameConstants.LoggedUserErrorGet);
        }

        [HttpPost("ManageUsers/PrimaryUserChange", Name = RouteNameConstants.PrimaryUserChangePost)]
        public async Task<ActionResult> PrimaryUserChange(long userId, bool? hasToBePrimaryContact)
        {
            var existingUser = await _manageUsersService.GetUser(userId);
            
            if (hasToBePrimaryContact == null)
            {
                existingUser.ShowErrorMessage = true;
                return View(existingUser);
            }

            if (!Convert.ToBoolean(hasToBePrimaryContact))
                return RedirectToRoute(RouteNameConstants.ManageUsersHomeGet);

            await _mediator.Send(new PrimaryUserChangePostHandler.Context
            { Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value, UserId = userId, CompanyId = existingUser.CompanyId });

            return RedirectToRoute(RouteNameConstants.HomeIndexGet);
        }
    }
}