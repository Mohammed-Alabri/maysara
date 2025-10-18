using Microsoft.AspNetCore.Mvc;
using Maysara.Server.Models;
using Maysara.Server.Services;

namespace Maysara.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantsController : ControllerBase
    {
        private readonly ILogger<RestaurantsController> _logger;
        private readonly DataService _dataService;

        public RestaurantsController(ILogger<RestaurantsController> logger)
        {
            _logger = logger;
            _dataService = DataService.Instance;
        }

        /// <summary>
        /// Get all active restaurants
        /// </summary>
        [HttpGet]
        public IActionResult GetRestaurants()
        {
            try
            {
                var restaurants = _dataService.GetActiveRestaurants();
                _logger.LogInformation("Retrieved {Count} restaurants", restaurants.Count);
                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving restaurants");
                return StatusCode(500, new { error = "An error occurred while retrieving restaurants. Please try again later." });
            }
        }

        /// <summary>
        /// Get restaurant by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetRestaurant(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { error = "Invalid restaurant ID. ID must be a positive number." });
                }

                var restaurant = _dataService.GetRestaurantById(id);

                if (restaurant == null)
                {
                    _logger.LogWarning("Restaurant with ID {RestaurantId} not found", id);
                    return NotFound(new { error = $"Restaurant with ID {id} not found." });
                }

                _logger.LogInformation("Retrieved restaurant {RestaurantId}", id);
                return Ok(restaurant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving restaurant {RestaurantId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the restaurant. Please try again later." });
            }
        }

        /// <summary>
        /// Get all restaurants (including inactive)
        /// </summary>
        [HttpGet("all")]
        public IActionResult GetAllRestaurants()
        {
            try
            {
                var restaurants = _dataService.GetAllRestaurants();
                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all restaurants");
                return StatusCode(500, new { error = "An error occurred while retrieving restaurants." });
            }
        }
    }
}
