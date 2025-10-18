using Microsoft.AspNetCore.Mvc;
using Maysara.Server.Models;
using Maysara.Server.Services;

namespace Maysara.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly DataService _dataService;

        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;
            _dataService = DataService.Instance;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        public IActionResult GetProducts()
        {
            try
            {
                var products = _dataService.GetAllProducts();
                _logger.LogInformation("Retrieved {Count} products", products.Count);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return StatusCode(500, new { error = "An error occurred while retrieving products. Please try again later." });
            }
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { error = "Invalid product ID. ID must be a positive number." });
                }

                var product = _dataService.GetProductById(id);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", id);
                    return NotFound(new { error = $"Product with ID {id} not found." });
                }

                _logger.LogInformation("Retrieved product {ProductId}", id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product {ProductId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the product. Please try again later." });
            }
        }

        /// <summary>
        /// Get products by restaurant
        /// </summary>
        [HttpGet("restaurant/{restaurantId}")]
        public IActionResult GetProductsByRestaurant(int restaurantId)
        {
            try
            {
                if (restaurantId <= 0)
                {
                    return BadRequest(new { error = "Invalid restaurant ID. ID must be a positive number." });
                }

                var restaurant = _dataService.GetRestaurantById(restaurantId);
                if (restaurant == null)
                {
                    return NotFound(new { error = $"Restaurant with ID {restaurantId} not found." });
                }

                var products = _dataService.GetProductsByRestaurant(restaurantId);
                _logger.LogInformation("Retrieved {Count} products for restaurant {RestaurantId}", products.Count, restaurantId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products for restaurant {RestaurantId}", restaurantId);
                return StatusCode(500, new { error = "An error occurred while retrieving products. Please try again later." });
            }
        }

        /// <summary>
        /// Get products by category
        /// </summary>
        [HttpGet("category/{category}")]
        public IActionResult GetProductsByCategory(string category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category))
                {
                    return BadRequest(new { error = "Category cannot be empty." });
                }

                var products = _dataService.GetProductsByCategory(category);
                _logger.LogInformation("Retrieved {Count} products for category {Category}", products.Count, category);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products for category {Category}", category);
                return StatusCode(500, new { error = "An error occurred while retrieving products. Please try again later." });
            }
        }
    }
}
