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
    [MvcAuthorize(AllowedRoles = new[] { UserRole.Customer })]
    public class CustomerController : Controller
    {
        private readonly MaysaraDbContext _context;

        public CustomerController(MaysaraDbContext context)
        {
            _context = context;
        }

        // GET: Customer/Index - Browse/Search Restaurants [SEARCH Operation]
        public async Task<IActionResult> Index(string searchTerm, decimal? minRating)
        {
            try
            {
                // LINQ Method Syntax with multiple filters
                var query = _context.Restaurants
                    .Include(r => r.Products)
                    .Where(r => r.IsActive);

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(r => r.Name.Contains(searchTerm));
                }

                // Apply rating filter
                if (minRating.HasValue)
                {
                    query = query.Where(r => r.Rating >= minRating.Value);
                }

                // Execute query with projection to ViewModel
                var restaurants = await query
                    .OrderByDescending(r => r.Rating)
                    .ThenBy(r => r.Name)
                    .Select(r => new RestaurantListViewModel
                    {
                        RestaurantID = r.RestaurantID,
                        Name = r.Name,
                        Address = r.Address,
                        Phone = r.Phone,
                        Rating = r.Rating,
                        DeliveryFee = r.DeliveryFee,
                        IsActive = r.IsActive,
                        ProductCount = r.Products!.Count(p => p.IsAvailable)
                    })
                    .ToListAsync();

                ViewData["SearchTerm"] = searchTerm;
                ViewData["MinRating"] = minRating;

                return View(restaurants);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading restaurants: {ex.Message}";
                return View(new List<RestaurantListViewModel>());
            }
        }

        // GET: Customer/RestaurantDetails/5 - View Menu [READ Operation]
        public async Task<IActionResult> RestaurantDetails(int id)
        {
            try
            {
                // LINQ with Include for eager loading
                var restaurant = await _context.Restaurants
                    .Include(r => r.Products)
                    .Where(r => r.RestaurantID == id && r.IsActive)
                    .FirstOrDefaultAsync();

                if (restaurant == null)
                {
                    TempData["Error"] = "Restaurant not found or inactive";
                    return RedirectToAction("Index");
                }

                var viewModel = new RestaurantDetailsViewModel
                {
                    RestaurantID = restaurant.RestaurantID,
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    Phone = restaurant.Phone,
                    Rating = restaurant.Rating,
                    DeliveryFee = restaurant.DeliveryFee,
                    Products = restaurant.Products?
                        .Where(p => p.IsAvailable)
                        .OrderBy(p => p.Category)
                        .ThenBy(p => p.Name)
                        .ToList() ?? new List<Product>()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading restaurant: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: Customer/AddToCart - Add Product to Cart [CREATE Operation - Session]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                // LINQ to load product with restaurant
                var product = await _context.Products
                    .Include(p => p.Restaurant)
                    .Where(p => p.ProductID == productId && p.IsAvailable)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    TempData["Error"] = "Product not found or unavailable";
                    return RedirectToAction("Index");
                }

                // Add to session cart
                var success = SessionManager.AddToCart(
                    HttpContext,
                    product.ProductID,
                    product.Name,
                    product.Price,
                    quantity,
                    product.RestaurantID,
                    product.Restaurant!.Name
                );

                if (!success)
                {
                    TempData["Error"] = "Cannot add items from different restaurants. Please clear your cart first.";
                }
                else
                {
                    TempData["Success"] = $"{product.Name} added to cart!";
                }

                return RedirectToAction("RestaurantDetails", new { id = product.RestaurantID });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error adding to cart: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // GET: Customer/Cart - View Shopping Cart
        public IActionResult Cart()
        {
            var cart = SessionManager.GetCart(HttpContext);
            return View(cart);
        }

        // POST: Customer/UpdateCartItem - Update Quantity [UPDATE Operation - Session]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCartItem(int productId, int quantity)
        {
            try
            {
                SessionManager.UpdateCartItem(HttpContext, productId, quantity);
                TempData["Success"] = "Cart updated successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating cart: {ex.Message}";
            }

            return RedirectToAction("Cart");
        }

        // GET: Customer/Checkout - Display Checkout Form
        public async Task<IActionResult> Checkout()
        {
            var cart = SessionManager.GetCart(HttpContext);

            if (cart == null || cart.Items.Count == 0)
            {
                TempData["Error"] = "Your cart is empty";
                return RedirectToAction("Cart");
            }

            try
            {
                // Get restaurant delivery fee
                var restaurant = await _context.Restaurants
                    .Where(r => r.RestaurantID == cart.RestaurantID)
                    .FirstOrDefaultAsync();

                if (restaurant == null)
                {
                    TempData["Error"] = "Restaurant not found";
                    return RedirectToAction("Cart");
                }

                // Get user's saved address
                var userId = SessionManager.GetUserId(HttpContext).Value;
                var user = await _context.Users
                    .Where(u => u.UserID == userId)
                    .FirstOrDefaultAsync();

                var viewModel = new CheckoutViewModel
                {
                    Cart = cart,
                    DeliveryFee = restaurant.DeliveryFee,
                    DeliveryAddress = user?.Address ?? string.Empty
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading checkout: {ex.Message}";
                return RedirectToAction("Cart");
            }
        }

        // POST: Customer/PlaceOrder - Submit Order [CREATE Operation - Database with Transaction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var cart = SessionManager.GetCart(HttpContext);
                if (cart != null)
                {
                    var restaurant = await _context.Restaurants.FindAsync(cart.RestaurantID);
                    model.Cart = cart;
                    model.DeliveryFee = restaurant?.DeliveryFee ?? 0;
                }
                return View("Checkout", model);
            }

            var orderCart = SessionManager.GetCart(HttpContext);
            if (orderCart == null || orderCart.Items.Count == 0)
            {
                TempData["Error"] = "Your cart is empty";
                return RedirectToAction("Cart");
            }

            var userId = SessionManager.GetUserId(HttpContext).Value;

            // BEGIN TRANSACTION
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get restaurant delivery fee
                var restaurant = await _context.Restaurants
                    .Where(r => r.RestaurantID == orderCart.RestaurantID)
                    .FirstOrDefaultAsync();

                if (restaurant == null)
                {
                    TempData["Error"] = "Restaurant not found";
                    return RedirectToAction("Cart");
                }

                // Create order
                var order = new Order
                {
                    UserID = userId,
                    RestaurantID = orderCart.RestaurantID,
                    TotalAmount = orderCart.TotalAmount + restaurant.DeliveryFee,
                    DeliveryAddress = model.DeliveryAddress,
                    PaymentMethod = model.PaymentMethod,
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Save to get OrderID

                // Create order items using LINQ Select
                var orderItems = orderCart.Items.Select(item => new OrderItem
                {
                    OrderID = order.OrderID,
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    UnitPrice = item.Price,
                    Quantity = item.Quantity
                }).ToList();

                _context.OrderItems.AddRange(orderItems);
                await _context.SaveChangesAsync();

                // COMMIT TRANSACTION
                await transaction.CommitAsync();

                // Clear cart from session
                SessionManager.ClearCart(HttpContext);

                TempData["Success"] = $"Order #{order.OrderID} placed successfully!";
                return RedirectToAction("OrderDetails", new { id = order.OrderID });
            }
            catch (Exception ex)
            {
                // ROLLBACK TRANSACTION
                await transaction.RollbackAsync();

                TempData["Error"] = $"Failed to place order: {ex.Message}";
                return RedirectToAction("Checkout");
            }
        }

        // GET: Customer/OrderHistory - View Past Orders [READ with JOIN - Query Syntax]
        public async Task<IActionResult> OrderHistory()
        {
            var userId = SessionManager.GetUserId(HttpContext).Value;

            try
            {
                // LINQ Query Syntax with JOIN
                var orders = await (
                    from o in _context.Orders
                    join r in _context.Restaurants on o.RestaurantID equals r.RestaurantID
                    where o.UserID == userId
                    orderby o.OrderDate descending
                    select new OrderHistoryViewModel
                    {
                        OrderID = o.OrderID,
                        RestaurantName = r.Name,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status,
                        OrderDate = o.OrderDate,
                        ItemCount = o.OrderItems!.Count()
                    }
                ).ToListAsync();

                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading order history: {ex.Message}";
                return View(new List<OrderHistoryViewModel>());
            }
        }

        // GET: Customer/OrderDetails/5 - View Single Order [READ with Multiple Includes]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = SessionManager.GetUserId(HttpContext).Value;

            try
            {
                // LINQ with ThenInclude for nested relationships
                var order = await _context.Orders
                    .Include(o => o.Restaurant)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Where(o => o.OrderID == id && o.UserID == userId)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    TempData["Error"] = "Order not found";
                    return RedirectToAction("OrderHistory");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading order details: {ex.Message}";
                return RedirectToAction("OrderHistory");
            }
        }
    }
}
