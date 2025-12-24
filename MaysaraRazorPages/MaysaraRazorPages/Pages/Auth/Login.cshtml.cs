using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MaysaraRazorPages.Data;
using MaysaraRazorPages.Models.ViewModels;
using MaysaraRazorPages.Services;

namespace MaysaraRazorPages.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly MaysaraDbContext _context;

        public LoginModel(MaysaraDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; } = new LoginViewModel();

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Display login page
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Method syntax: Find user by email
                var user = _context.Users
                    .Where(u => u.Email == Input.Email)
                    .FirstOrDefault();

                // Alternative: Query syntax
                // var user = (from u in _context.Users
                //             where u.Email == Input.Email
                //             select u).FirstOrDefault();

                if (user == null)
                {
                    ErrorMessage = "Invalid email or password.";
                    return Page();
                }

                // Verify password (plain text comparison for Phase 2 - should use hashing in production)
                if (user.Password != Input.Password)
                {
                    ErrorMessage = "Invalid email or password.";
                    return Page();
                }

                // Set user session
                SessionManager.SetUserSession(HttpContext, user);

                // Redirect based on user role
                if (user.Role == Models.Enums.UserRole.Admin)
                {
                    return RedirectToPage("/Admin/Dashboard");
                }
                else if (user.Role == Models.Enums.UserRole.Vendor)
                {
                    return RedirectToPage("/Products/Create");
                }
                else
                {
                    return RedirectToPage("/Restaurants/Index");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred during login: {ex.Message}";
                return Page();
            }
        }
    }
}
