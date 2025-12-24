using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MaysaraRazorPages.Data;
using MaysaraRazorPages.Models;
using MaysaraRazorPages.Models.ViewModels;

namespace MaysaraRazorPages.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly MaysaraDbContext _context;

        public RegisterModel(MaysaraDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegisterViewModel Input { get; set; } = new RegisterViewModel();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
            // Display registration page
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Check if email already exists using LINQ Any()
                bool emailExists = _context.Users.Any(u => u.Email == Input.Email);

                if (emailExists)
                {
                    ErrorMessage = "Email already registered. Please use a different email.";
                    return Page();
                }

                // Create new user
                var newUser = new User
                {
                    Name = Input.Name,
                    Email = Input.Email,
                    Password = Input.Password,
                    Phone = Input.Phone,
                    Address = Input.Address,
                    Role = Input.Role,
                    CreatedAt = DateTime.Now
                };

                // Add user to database (EF CRUD - Create)
                _context.Users.Add(newUser);
                _context.SaveChanges();

                SuccessMessage = "Registration successful! Please login.";

                // Redirect to login page after successful registration
                return RedirectToPage("/Auth/Login");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred during registration: {ex.Message}";
                return Page();
            }
        }
    }
}
