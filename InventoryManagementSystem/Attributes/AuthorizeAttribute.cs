using InventoryManagementSystem.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventoryManagementSystem.Attributes
{
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var userId = session.GetString(SessionKeys.UserId);
            var isApi = context.HttpContext.Request.Path.StartsWithSegments("/api");

            if (string.IsNullOrEmpty(userId))
            {
                context.Result = isApi
                    ? new UnauthorizedResult()
                    : new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            if (_roles.Length > 0)
            {
                var userRole = session.GetString(SessionKeys.UserRole);
                if (!_roles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
                {
                    context.Result = isApi ? new ForbidResult() : new RedirectToActionResult("Index", "Home", null);
                }
            }
        }
    }
}
