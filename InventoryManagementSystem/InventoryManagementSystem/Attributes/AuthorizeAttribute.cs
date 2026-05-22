using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventoryManagementSystem.Attributes
{
    /// <summary>
    /// Custom authorization attribute to check if user is logged in
    /// </summary>
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                // Not logged in, redirect to login
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            // Check role if specified
            if (_roles.Length > 0)
            {
                var userRole = context.HttpContext.Session.GetString("UserRole");

                if (!_roles.Contains(userRole))
                {
                    // User doesn't have required role
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}
