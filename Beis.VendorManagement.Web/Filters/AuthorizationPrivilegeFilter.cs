using Microsoft.AspNetCore.Mvc.Controllers;

namespace Beis.VendorManagement.Web.Filters
{
    public class AuthorizationPrivilegeFilter : IAsyncActionFilter
    {
        private readonly IManageUsersService _service;

        private static IEnumerable<string> NoCheckRequiredActions => new List<string>
        {
            ApplicationConstants.ManageUsersHome,
            ApplicationConstants.ConfirmPrimaryUserChange
        };

        public AuthorizationPrivilegeFilter(IManageUsersService service)
        {
            _service = service;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            switch (context.HttpContext.User.Identity.IsAuthenticated 
                    && actionDescriptor.ControllerName.Equals(ApplicationConstants.ManageUsers, StringComparison.OrdinalIgnoreCase)
                    && !NoCheckRequiredActions.Contains(actionDescriptor.ActionName))
            {
                case true:
                    if (!await MatchCompanyIdByAdb2CId(context))
                    {
                        context.Result = new RedirectToRouteResult(RouteNameConstants.InvalidCompanyErrorGet, new RouteValueDictionary());
                        return;
                    }

                    break;
            }

            await next();
        }

        private async Task<bool> MatchCompanyIdByAdb2CId(ActionExecutingContext filterContext)
        {
            long currentUserId = 0;
            if (filterContext.ActionArguments.ContainsKey(ApplicationConstants.UserId))
            {
                currentUserId = ParseToLong(filterContext.ActionArguments[ApplicationConstants.UserId].ToString());
            }
            else if (filterContext.ActionArguments.ContainsKey(ApplicationConstants.User))
            {
                currentUserId = ((UserViewModel)filterContext.ActionArguments[ApplicationConstants.User]).UserId;
            }

            if (currentUserId == 0) //user id will be 0 for add user
            {
                return true;
            }

            var currentUser = await _service.GetUser(currentUserId);
            if (currentUser == null) return false;

            var loggedInUser = await _service.GetUser(filterContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return currentUser?.CompanyId == loggedInUser?.CompanyId;
        }

        private static long ParseToLong(string value)
        {
            long.TryParse(value, out var convertedValue);
            return convertedValue;
        }
    }
}