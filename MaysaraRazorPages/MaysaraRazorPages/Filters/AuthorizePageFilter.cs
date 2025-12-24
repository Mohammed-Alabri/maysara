using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MaysaraRazorPages.Services;

namespace MaysaraRazorPages.Filters
{
    /// <summary>
    /// Page Filter for Authorization
    /// Ensures users are authenticated before accessing pages
    /// Excludes Login, Register, and Logout pages from authentication requirement
    /// </summary>
    public class AuthorizePageFilter : IPageFilter
    {
        private static readonly string[] ExcludedPages = new[]
        {
            "/Auth/Login",
            "/Auth/Register",
            "/Auth/Logout"
        };

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            // Not needed for this implementation
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var pagePath = context.ActionDescriptor.RelativePath;

            // Check if the current page is in the excluded list
            bool isExcludedPage = ExcludedPages.Any(excluded =>
                pagePath?.Contains(excluded, StringComparison.OrdinalIgnoreCase) == true);

            // If page is not excluded and user is not authenticated, redirect to login
            if (!isExcludedPage && !SessionManager.IsAuthenticated(httpContext))
            {
                context.Result = new RedirectToPageResult("/Auth/Login");
            }
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            // Not needed for this implementation
        }
    }
}
