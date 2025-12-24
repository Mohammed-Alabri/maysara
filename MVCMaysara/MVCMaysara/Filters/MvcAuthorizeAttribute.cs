using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MVCMaysara.Services;
using MVCMaysara.Models.Enums;

namespace MVCMaysara.Filters
{
    public class MvcAuthorizeAttribute : ActionFilterAttribute
    {
        public UserRole[]? AllowedRoles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            if (!SessionManager.IsAuthenticated(httpContext))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            if (AllowedRoles != null && AllowedRoles.Length > 0)
            {
                var userRole = SessionManager.GetUserRole(httpContext);
                if (!AllowedRoles.Any(r => r.ToString() == userRole))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
