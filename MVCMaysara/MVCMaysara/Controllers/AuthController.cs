using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCMaysara.Data;
using MVCMaysara.Models;
using MVCMaysara.ViewModels;
using MVCMaysara.Services;

namespace MVCMaysara.Controllers
{
    public class AuthController : Controller
    {
        private readonly MaysaraDbContext _context;

        public AuthController(MaysaraDbContext context)
        {
            _context = context;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            // If already logged in, redirect to appropriate page
            if (SessionManager.IsAuthenticated(HttpContext))
            {
                var role = SessionManager.GetUserRole(HttpContext);
                if (role == "Customer")
                    return RedirectToAction("Index", "Customer");
                else if (role == "Vendor")
                    return RedirectToAction("Index", "Vendor");
            }

            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // LINQ query to find user by email
                var user = await _context.Users
                    .Where(u => u.Email == model.Email)
                    .FirstOrDefaultAsync();

                if (user == null || user.Password != model.Password)
                {
                    ModelState.AddModelError("", "Invalid email or password");
                    return View(model);
                }

                // Set user session
                SessionManager.SetUserSession(HttpContext, user);

                TempData["Success"] = $"Welcome back, {user.Name}!";

                // Redirect based on role
                if (user.Role == Models.Enums.UserRole.Customer)
                    return RedirectToAction("Index", "Customer");
                else if (user.Role == Models.Enums.UserRole.Vendor)
                    return RedirectToAction("Index", "Vendor");
                else
                    return RedirectToAction("Index", "Customer");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(model);
            }
        }

        // GET: Auth/Register
        public IActionResult Register()
        {
            // If already logged in, redirect
            if (SessionManager.IsAuthenticated(HttpContext))
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // LINQ query to check if email already exists
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == model.Email);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email address is already registered");
                    return View(model);
                }

                // Create new user
                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password, // In production, hash this!
                    Phone = model.Phone,
                    Address = model.Address,
                    Role = model.Role,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Registration failed: {ex.Message}");
                return View(model);
            }
        }

        // POST: Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            SessionManager.ClearSession(HttpContext);
            TempData["Success"] = "You have been logged out successfully";
            return RedirectToAction("Login");
        }
    }
}
