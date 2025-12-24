using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCMaysara.Data;
using MVCMaysara.Models;
using MVCMaysara.Models.Enums;
using MVCMaysara.ViewModels;
using MVCMaysara.Services;
using MVCMaysara.Filters;

namespace MVCMaysara.Controllers
{
    [MvcAuthorize(AllowedRoles = new[] { UserRole.Vendor })]
    public class VendorController : Controller
    {
        private readonly MaysaraDbContext _context;

        public VendorController(MaysaraDbContext context)
        {
            _context = context;
        }

        // GET: Vendor/Index - Vendor Dashboard [SEARCH with Aggregates]
        public async Task<IActionResult> Index()
        {
            var vendorId = SessionManager.GetUserId(HttpContext).Value;

            try
            {
                // LINQ Aggregation queries
                var dashboard = new VendorDashboardViewModel
                {
                    TotalRestaurants = await _context.Restaurants
                        .Where(r => r.OwnerID == vendorId)
                        .CountAsync(),

                    TotalProducts = await _context.Products
                        .Where(p => p.Restaurant!.OwnerID == vendorId)
                        .CountAsync(),

                    TotalOrders = await _context.Orders
                        .Where(o => o.Restaurant!.OwnerID == vendorId)
                        .CountAsync(),

                    TotalRevenue = await _context.Orders
                        .Where(o => o.Restaurant!.OwnerID == vendorId && o.Status == OrderStatus.Delivered)
                        .SumAsync(o => (decimal?)o.TotalAmount) ?? 0,

                    // GroupBy for restaurant statistics
                    RestaurantStats = await _context.Orders
                        .Include(o => o.Restaurant)
                        .Where(o => o.Restaurant!.OwnerID == vendorId)
                        .GroupBy(o => new { o.RestaurantID, o.Restaurant!.Name })
                        .Select(g => new RestaurantStatViewModel
                        {
                            RestaurantName = g.Key.Name,
                            OrderCount = g.Count(),
                            Revenue = g.Where(o => o.Status == OrderStatus.Delivered)
                                       .Sum(o => o.TotalAmount)
                        })
                        .OrderByDescending(s => s.Revenue)
                        .ToListAsync()
                };

                return View(dashboard);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading dashboard: {ex.Message}";
                return View(new VendorDashboardViewModel());
            }
        }

        // GET: Vendor/RestaurantList - List Vendor's Restaurants [SEARCH]
        public async Task<IActionResult> RestaurantList()
        {
            var vendorId = SessionManager.GetUserId(HttpContext).Value;

            try
            {
                var restaurants = await _context.Restaurants
                    .Where(r => r.OwnerID == vendorId)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new RestaurantListViewModel
                    {
                        RestaurantID = r.RestaurantID,
                        Name = r.Name,
                        Address = r.Address,
                        Phone = r.Phone,
                        Rating = r.Rating,
                        DeliveryFee = r.DeliveryFee,
                        IsActive = r.IsActive,
                        ProductCount = r.Products!.Count()
                    })
                    .ToListAsync();

                return View(restaurants);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading restaurants: {ex.Message}";
                return View(new List<RestaurantListViewModel>());
            }
        }

        // GET: Vendor/RestaurantCreate - Display Create Form
        public IActionResult RestaurantCreate()
        {
            return View();
        }

        // POST: Vendor/RestaurantCreate - Create Restaurant [CREATE Operation]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestaurantCreate(Restaurant model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var vendorId = SessionManager.GetUserId(HttpContext).Value;

                model.OwnerID = vendorId;
                model.CreatedAt = DateTime.Now;
                model.Rating = 0m;
                model.IsActive = true;

                _context.Restaurants.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Restaurant created successfully!";
                return RedirectToAction("RestaurantList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating restaurant: {ex.Message}");
                return View(model);
            }
        }

        // GET: Vendor/RestaurantEdit/5 - Display Edit Form [READ]
        public async Task<IActionResult> RestaurantEdit(int id)
        {
            var vendorId = SessionManager.GetUserId(HttpContext).Value;

            try
            {
                var restaurant = await _context.Restaurants
                    .Where(r => r.RestaurantID == id && r.OwnerID == vendorId)
                    .FirstOrDefaultAsync();

                if (restaurant == null)
                {
                    TempData["Error"] = "Restaurant not found or you don't have permission";
                    return RedirectToAction("RestaurantList");
                }

                return View(restaurant);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading restaurant: {ex.Message}";
                return RedirectToAction("RestaurantList");
            }
        }

        // POST: Vendor/RestaurantEdit/5 - Update Restaurant [UPDATE Operation]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestaurantEdit(int id, Restaurant model)
        {
            if (id != model.RestaurantID)
            {
                TempData["Error"] = "Invalid restaurant ID";
                return RedirectToAction("RestaurantList");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var vendorId = SessionManager.GetUserId(HttpContext).Value;

                var restaurant = await _context.Restaurants
                    .Where(r => r.RestaurantID == id && r.OwnerID == vendorId)
                    .FirstOrDefaultAsync();

                if (restaurant == null)
                {
                    TempData["Error"] = "Restaurant not found or you don't have permission";
                    return RedirectToAction("RestaurantList");
                }

                // Update properties
                restaurant.Name = model.Name;
                restaurant.Address = model.Address;
                restaurant.Phone = model.Phone;
                restaurant.DeliveryFee = model.DeliveryFee;
                restaurant.IsActive = model.IsActive;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Restaurant updated successfully!";
                return RedirectToAction("RestaurantList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating restaurant: {ex.Message}");
                return View(model);
            }
        }

        // POST: Vendor/RestaurantDelete/5 - Delete Restaurant [DELETE Operation]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestaurantDelete(int id)
        {
            try
            {
                var vendorId = SessionManager.GetUserId(HttpContext).Value;

                var restaurant = await _context.Restaurants
                    .Include(r => r.Products)
                    .Where(r => r.RestaurantID == id && r.OwnerID == vendorId)
                    .FirstOrDefaultAsync();

                if (restaurant == null)
                {
                    TempData["Error"] = "Restaurant not found or you don't have permission";
                    return RedirectToAction("RestaurantList");
                }

                // Check if restaurant has orders
                var hasOrders = await _context.Orders
                    .AnyAsync(o => o.RestaurantID == id);

                if (hasOrders)
                {
                    // Soft delete - just mark as inactive
                    restaurant.IsActive = false;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Restaurant deactivated (has existing orders)";
                }
                else
                {
                    // Hard delete if no orders
                    _context.Restaurants.Remove(restaurant);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Restaurant deleted successfully!";
                }

                return RedirectToAction("RestaurantList");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting restaurant: {ex.Message}";
                return RedirectToAction("RestaurantList");
            }
        }

        // GET: Vendor/ProductList/5 - List Products for Restaurant [SEARCH]
        public async Task<IActionResult> ProductList(int restaurantId)
        {
            var vendorId = SessionManager.GetUserId(HttpContext).Value;

            try
            {
                // Verify ownership
                var restaurant = await _context.Restaurants
                    .Where(r => r.RestaurantID == restaurantId && r.OwnerID == vendorId)
                    .FirstOrDefaultAsync();

                if (restaurant == null)
                {
                    TempData["Error"] = "Restaurant not found or you don't have permission";
                    return RedirectToAction("RestaurantList");
                }

                var products = await _context.Products
                    .Where(p => p.RestaurantID == restaurantId)
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.Name)
                    .ToListAsync();

                ViewBag.Restaurant = restaurant;
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading products: {ex.Message}";
                return RedirectToAction("RestaurantList");
            }
        }

        // GET: Vendor/ProductCreate/5 - Display Create Form
        public async Task<IActionResult> ProductCreate(int restaurantId)
        {
            var vendorId = SessionManager.GetUserId(HttpContext).Value;

            try
            {
                // Verify ownership
                var restaurant = await _context.Restaurants
                    .Where(r => r.RestaurantID == restaurantId && r.OwnerID == vendorId)
                    .FirstOrDefaultAsync();

                if (restaurant == null)
                {
                    TempData["Error"] = "Restaurant not found or you don't have permission";
                    return RedirectToAction("RestaurantList");
                }

                ViewBag.Restaurant = restaurant;
                return View(new Product { RestaurantID = restaurantId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("RestaurantList");
            }
        }

        // POST: Vendor/ProductCreate - Create Product [CREATE Operation]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate(Product model)
        {
            if (!ModelState.IsValid)
            {
                var restaurant = await _context.Restaurants.FindAsync(model.RestaurantID);
                ViewBag.Restaurant = restaurant;
                return View(model);
            }

            try
            {
                var vendorId = SessionManager.GetUserId(HttpContext).Value;

                // Verify ownership
                var ownsRestaurant = await _context.Restaurants
                    .AnyAsync(r => r.RestaurantID == model.RestaurantID && r.OwnerID == vendorId);

                if (!ownsRestaurant)
                {
                    TempData["Error"] = "You don't have permission to add products to this restaurant";
                    return RedirectToAction("RestaurantList");
                }

                model.CreatedAt = DateTime.Now;
                model.IsAvailable = true;

                _context.Products.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Product created successfully!";
                return RedirectToAction("ProductList", new { restaurantId = model.RestaurantID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating product: {ex.Message}");
                var restaurant = await _context.Restaurants.FindAsync(model.RestaurantID);
                ViewBag.Restaurant = restaurant;
                return View(model);
            }
        }

        // GET: Vendor/ProductEdit/5 - Display Edit Form [READ]
        public async Task<IActionResult> ProductEdit(int id)
        {
            var vendorId = SessionManager.GetUserId(HttpContext).Value;

            try
            {
                var product = await _context.Products
                    .Include(p => p.Restaurant)
                    .Where(p => p.ProductID == id && p.Restaurant!.OwnerID == vendorId)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    TempData["Error"] = "Product not found or you don't have permission";
                    return RedirectToAction("RestaurantList");
                }

                ViewBag.Restaurant = product.Restaurant;
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading product: {ex.Message}";
                return RedirectToAction("RestaurantList");
            }
        }

        // POST: Vendor/ProductEdit/5 - Update Product [UPDATE Operation]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(int id, Product model)
        {
            if (id != model.ProductID)
            {
                TempData["Error"] = "Invalid product ID";
                return RedirectToAction("RestaurantList");
            }

            if (!ModelState.IsValid)
            {
                var restaurant = await _context.Restaurants.FindAsync(model.RestaurantID);
                ViewBag.Restaurant = restaurant;
                return View(model);
            }

            try
            {
                var vendorId = SessionManager.GetUserId(HttpContext).Value;

                var product = await _context.Products
                    .Include(p => p.Restaurant)
                    .Where(p => p.ProductID == id && p.Restaurant!.OwnerID == vendorId)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    TempData["Error"] = "Product not found or you don't have permission";
                    return RedirectToAction("RestaurantList");
                }

                // Update properties
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Category = model.Category;
                product.Stock = model.Stock;
                product.IsAvailable = model.IsAvailable;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction("ProductList", new { restaurantId = product.RestaurantID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating product: {ex.Message}");
                var restaurant = await _context.Restaurants.FindAsync(model.RestaurantID);
                ViewBag.Restaurant = restaurant;
                return View(model);
            }
        }

        // POST: Vendor/ProductDelete/5 - Delete Product [DELETE Operation]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(int id)
        {
            try
            {
                var vendorId = SessionManager.GetUserId(HttpContext).Value;

                var product = await _context.Products
                    .Include(p => p.Restaurant)
                    .Where(p => p.ProductID == id && p.Restaurant!.OwnerID == vendorId)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    TempData["Error"] = "Product not found or you don't have permission";
                    return RedirectToAction("RestaurantList");
                }

                var restaurantId = product.RestaurantID;

                // Check if product is in any orders
                var inOrders = await _context.OrderItems
                    .AnyAsync(oi => oi.ProductID == id);

                if (inOrders)
                {
                    // Soft delete
                    product.IsAvailable = false;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Product marked as unavailable (exists in orders)";
                }
                else
                {
                    // Hard delete
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Product deleted successfully!";
                }

                return RedirectToAction("ProductList", new { restaurantId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting product: {ex.Message}";
                return RedirectToAction("RestaurantList");
            }
        }
    }
}
