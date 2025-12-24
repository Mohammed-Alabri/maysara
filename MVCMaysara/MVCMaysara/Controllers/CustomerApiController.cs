using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCMaysara.Data;
using MVCMaysara.Models;
using MVCMaysara.Models.DTOs;
using MVCMaysara.Models.Enums;
using MVCMaysara.Services;

namespace MVCMaysara.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private readonly MaysaraDbContext _context;

        public CustomerApiController(MaysaraDbContext context)
        {
            _context = context;
        }

        // GET: api/customer/restaurants
        [HttpGet("restaurants")]
        public async Task<ActionResult<object>> GetRestaurants([FromQuery] string? search, [FromQuery] decimal? minRating)
        {
            try
            {
                var query = _context.Restaurants
                    .Where(r => r.IsActive);

                if (!string.IsNullOrEmpty(search))
                    query = query.Where(r => r.Name.Contains(search));

                if (minRating.HasValue)
                    query = query.Where(r => r.Rating >= minRating.Value);

                var restaurants = await query
                    .OrderByDescending(r => r.Rating)
                    .ThenBy(r => r.Name)
                    .Select(r => new
                    {
                        r.RestaurantID,
                        r.Name,
                        r.Address,
                        r.Phone,
                        r.Rating,
                        r.DeliveryFee,
                        ProductCount = r.Products!.Count(p => p.IsAvailable)
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = restaurants });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/customer/restaurants/5
        [HttpGet("restaurants/{id}")]
        public async Task<ActionResult<object>> GetRestaurantDetails(int id)
        {
            try
            {
                var restaurant = await _context.Restaurants
                    .Include(r => r.Products)
                    .Where(r => r.RestaurantID == id && r.IsActive)
                    .Select(r => new
                    {
                        r.RestaurantID,
                        r.Name,
                        r.Address,
                        r.Phone,
                        r.Rating,
                        r.DeliveryFee,
                        Products = r.Products!
                            .Where(p => p.IsAvailable)
                            .OrderBy(p => p.Category)
                            .ThenBy(p => p.Name)
                            .Select(p => new
                            {
                                p.ProductID,
                                p.Name,
                                p.Description,
                                p.Price,
                                p.Category,
                                p.Stock
                            })
                    })
                    .FirstOrDefaultAsync();

                if (restaurant == null)
                    return NotFound(new { success = false, message = "Restaurant not found" });

                return Ok(new { success = true, data = restaurant });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/customer/cart
        [HttpGet("cart")]
        public ActionResult<object> GetCart()
        {
            try
            {
                var cart = SessionManager.GetCart(HttpContext);
                return Ok(new { success = true, data = cart });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/customer/cart/add
        [HttpPost("cart/add")]
        public async Task<ActionResult<object>> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Restaurant)
                    .Where(p => p.ProductID == request.ProductId && p.IsAvailable)
                    .FirstOrDefaultAsync();

                if (product == null)
                    return NotFound(new { success = false, message = "Product not found" });

                var success = SessionManager.AddToCart(
                    HttpContext,
                    product.ProductID,
                    product.Name,
                    product.Price,
                    request.Quantity,
                    product.RestaurantID,
                    product.Restaurant!.Name
                );

                if (!success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Cannot add items from different restaurant"
                    });
                }

                var cart = SessionManager.GetCart(HttpContext);
                return Ok(new { success = true, data = cart, message = "Product added to cart" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // PUT: api/customer/cart/update
        [HttpPut("cart/update")]
        public ActionResult<object> UpdateCart([FromBody] UpdateCartRequest request)
        {
            try
            {
                SessionManager.UpdateCartItem(HttpContext, request.ProductId, request.Quantity);
                var cart = SessionManager.GetCart(HttpContext);
                return Ok(new { success = true, data = cart, message = "Cart updated" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // DELETE: api/customer/cart/remove/5
        [HttpDelete("cart/remove/{productId}")]
        public ActionResult<object> RemoveFromCart(int productId)
        {
            try
            {
                SessionManager.UpdateCartItem(HttpContext, productId, 0);
                var cart = SessionManager.GetCart(HttpContext);
                return Ok(new { success = true, data = cart, message = "Item removed from cart" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/customer/orders
        [HttpGet("orders")]
        public async Task<ActionResult<object>> GetOrderHistory()
        {
            try
            {
                var userId = SessionManager.GetUserId(HttpContext);
                if (!userId.HasValue)
                    return Unauthorized(new { success = false, message = "Not authenticated" });

                var orders = await (
                    from o in _context.Orders
                    join r in _context.Restaurants on o.RestaurantID equals r.RestaurantID
                    where o.UserID == userId.Value
                    orderby o.OrderDate descending
                    select new
                    {
                        o.OrderID,
                        RestaurantName = r.Name,
                        o.TotalAmount,
                        o.Status,
                        o.OrderDate,
                        ItemCount = o.OrderItems!.Count()
                    }
                ).ToListAsync();

                return Ok(new { success = true, data = orders });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/customer/orders/5
        [HttpGet("orders/{id}")]
        public async Task<ActionResult<object>> GetOrderDetails(int id)
        {
            try
            {
                var userId = SessionManager.GetUserId(HttpContext);
                if (!userId.HasValue)
                    return Unauthorized(new { success = false, message = "Not authenticated" });

                var order = await _context.Orders
                    .Include(o => o.Restaurant)
                    .Include(o => o.OrderItems)
                    .Where(o => o.OrderID == id && o.UserID == userId.Value)
                    .Select(o => new
                    {
                        o.OrderID,
                        RestaurantName = o.Restaurant!.Name,
                        o.TotalAmount,
                        o.DeliveryAddress,
                        o.PaymentMethod,
                        o.Status,
                        o.OrderDate,
                        Items = o.OrderItems!.Select(oi => new
                        {
                            oi.ProductName,
                            oi.UnitPrice,
                            oi.Quantity,
                            oi.Subtotal
                        })
                    })
                    .FirstOrDefaultAsync();

                if (order == null)
                    return NotFound(new { success = false, message = "Order not found" });

                return Ok(new { success = true, data = order });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/customer/orders
        [HttpPost("orders")]
        public async Task<ActionResult<object>> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                var userId = SessionManager.GetUserId(HttpContext);
                if (!userId.HasValue)
                    return Unauthorized(new { success = false, message = "Not authenticated" });

                var cart = SessionManager.GetCart(HttpContext);
                if (cart == null || cart.Items.Count == 0)
                    return BadRequest(new { success = false, message = "Cart is empty" });

                if (string.IsNullOrWhiteSpace(request.DeliveryAddress))
                    return BadRequest(new { success = false, message = "Delivery address is required" });

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var restaurant = await _context.Restaurants
                        .Where(r => r.RestaurantID == cart.RestaurantID)
                        .FirstOrDefaultAsync();

                    if (restaurant == null)
                        return NotFound(new { success = false, message = "Restaurant not found" });

                    var order = new Order
                    {
                        UserID = userId.Value,
                        RestaurantID = cart.RestaurantID,
                        TotalAmount = cart.TotalAmount + restaurant.DeliveryFee,
                        DeliveryAddress = request.DeliveryAddress,
                        PaymentMethod = request.PaymentMethod,
                        Status = OrderStatus.Pending,
                        OrderDate = DateTime.Now
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    var orderItems = cart.Items.Select(item => new OrderItem
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        ProductName = item.ProductName,
                        UnitPrice = item.Price,
                        Quantity = item.Quantity
                    }).ToList();

                    _context.OrderItems.AddRange(orderItems);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    SessionManager.ClearCart(HttpContext);

                    return StatusCode(201, new
                    {
                        success = true,
                        data = new { OrderID = order.OrderID },
                        message = "Order placed successfully"
                    });
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
