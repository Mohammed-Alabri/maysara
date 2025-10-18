using Microsoft.AspNetCore.Mvc;
using Maysara.Server.Models;
using Maysara.Server.Services;

namespace Maysara.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly DataService _dataService;

        public OrdersController(ILogger<OrdersController> logger)
        {
            _logger = logger;
            _dataService = DataService.Instance;
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        [HttpGet]
        public IActionResult GetOrders()
        {
            try
            {
                var orders = _dataService.GetAllOrders();
                _logger.LogInformation("Retrieved {Count} orders", orders.Count);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, new { error = "An error occurred while retrieving orders. Please try again later." });
            }
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { error = "Invalid order ID. ID must be a positive number." });
                }

                var order = _dataService.GetOrderById(id);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found", id);
                    return NotFound(new { error = $"Order with ID {id} not found." });
                }

                _logger.LogInformation("Retrieved order {OrderId}", id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the order. Please try again later." });
            }
        }

        /// <summary>
        /// Get orders by user
        /// </summary>
        [HttpGet("user/{userId}")]
        public IActionResult GetOrdersByUser(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new { error = "User ID cannot be empty." });
                }

                var orders = _dataService.GetOrdersByUser(userId);
                _logger.LogInformation("Retrieved {Count} orders for user {UserId}", orders.Count, userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for user {UserId}", userId);
                return StatusCode(500, new { error = "An error occurred while retrieving orders. Please try again later." });
            }
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        [HttpPost]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            try
            {
                if (order == null)
                {
                    return BadRequest(new { error = "Order data is required." });
                }

                // Validate order
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new { error = "Validation failed", details = errors });
                }

                // Validate restaurant exists
                var restaurant = _dataService.GetRestaurantById(order.RestaurantID);
                if (restaurant == null)
                {
                    return BadRequest(new { error = $"Restaurant with ID {order.RestaurantID} not found." });
                }

                // Validate items exist and are available
                if (order.Items == null || !order.Items.Any())
                {
                    return BadRequest(new { error = "Order must contain at least one item." });
                }

                foreach (var item in order.Items)
                {
                    var product = _dataService.GetProductById(item.ProductID);
                    if (product == null)
                    {
                        return BadRequest(new { error = $"Product with ID {item.ProductID} not found." });
                    }

                    if (!product.CanOrder(item.Quantity))
                    {
                        return BadRequest(new { error = $"Product '{product.Name}' is not available or insufficient stock." });
                    }
                }

                var createdOrder = _dataService.CreateOrder(order);
                _logger.LogInformation("Created order {OrderId} for user {UserId}", createdOrder.OrderID, createdOrder.UserID);

                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.OrderID }, createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, new { error = "An error occurred while creating the order. Please try again later." });
            }
        }

        /// <summary>
        /// Update order status
        /// </summary>
        [HttpPut("{id}/status")]
        public IActionResult UpdateOrderStatus(int id, [FromBody] OrderStatusUpdate statusUpdate)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { error = "Invalid order ID." });
                }

                if (statusUpdate == null)
                {
                    return BadRequest(new { error = "Status update data is required." });
                }

                var order = _dataService.GetOrderById(id);
                if (order == null)
                {
                    return NotFound(new { error = $"Order with ID {id} not found." });
                }

                var success = _dataService.UpdateOrderStatus(id, statusUpdate.Status);
                if (!success)
                {
                    return BadRequest(new { error = "Failed to update order status." });
                }

                _logger.LogInformation("Updated order {OrderId} status to {Status}", id, statusUpdate.Status);

                var updatedOrder = _dataService.GetOrderById(id);
                return Ok(updatedOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} status", id);
                return StatusCode(500, new { error = "An error occurred while updating the order status. Please try again later." });
            }
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        [HttpPost("{id}/cancel")]
        public IActionResult CancelOrder(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { error = "Invalid order ID." });
                }

                var order = _dataService.GetOrderById(id);
                if (order == null)
                {
                    return NotFound(new { error = $"Order with ID {id} not found." });
                }

                var success = _dataService.CancelOrder(id);
                if (!success)
                {
                    return BadRequest(new { error = "Failed to cancel order." });
                }

                _logger.LogInformation("Cancelled order {OrderId}", id);

                var cancelledOrder = _dataService.GetOrderById(id);
                return Ok(cancelledOrder);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot cancel order {OrderId}", id);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", id);
                return StatusCode(500, new { error = "An error occurred while cancelling the order. Please try again later." });
            }
        }
    }

    /// <summary>
    /// DTO for updating order status
    /// </summary>
    public class OrderStatusUpdate
    {
        public OrderStatus Status { get; set; }
    }
}
