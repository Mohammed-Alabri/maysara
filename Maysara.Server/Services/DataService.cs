using Maysara.Server.Models;

namespace Maysara.Server.Services
{
    /// <summary>
    /// In-memory data service for managing restaurants, products, and orders
    /// </summary>
    public class DataService
    {
        private static readonly object _lock = new object();
        private static DataService? _instance;

        // In-memory collections
        private readonly List<Restaurant> _restaurants;
        private readonly List<Product> _products;
        private readonly List<Order> _orders;

        // ID generators
        private int _nextRestaurantId = 1;
        private int _nextProductId = 1;
        private int _nextOrderId = 1;

        // Singleton pattern
        public static DataService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DataService();
                        }
                    }
                }
                return _instance;
            }
        }

        private DataService()
        {
            _restaurants = new List<Restaurant>();
            _products = new List<Product>();
            _orders = new List<Order>();
            InitializeSampleData();
        }

        // Initialize with sample data
        private void InitializeSampleData()
        {
            // Sample Restaurants
            var restaurant1 = new Restaurant
            {
                RestaurantID = _nextRestaurantId++,
                Name = "Al Angham Restaurant",
                Address = "Muscat, Oman",
                Phone = "+968-2444-5555",
                Rating = 4.5,
                DeliveryFee = 2.50m,
                IsActive = true,
                Cuisine = "Traditional Omani",
                ImageUrl = "/images/restaurants/alangham.jpg"
            };

            var restaurant2 = new Restaurant
            {
                RestaurantID = _nextRestaurantId++,
                Name = "Bait Al Luban",
                Address = "Mutrah, Muscat",
                Phone = "+968-2471-4408",
                Rating = 4.7,
                DeliveryFee = 3.00m,
                IsActive = true,
                Cuisine = "Omani Seafood",
                ImageUrl = "/images/restaurants/baitalluban.jpg"
            };

            var restaurant3 = new Restaurant
            {
                RestaurantID = _nextRestaurantId++,
                Name = "Turkish House Restaurant",
                Address = "Al Khuwair, Muscat",
                Phone = "+968-2448-3232",
                Rating = 4.3,
                DeliveryFee = 2.00m,
                IsActive = true,
                Cuisine = "Turkish",
                ImageUrl = "/images/restaurants/turkish.jpg"
            };

            var restaurant4 = new Restaurant
            {
                RestaurantID = _nextRestaurantId++,
                Name = "India Palace",
                Address = "Ruwi, Muscat",
                Phone = "+968-2470-2020",
                Rating = 4.6,
                DeliveryFee = 1.50m,
                IsActive = true,
                Cuisine = "Indian",
                ImageUrl = "/images/restaurants/indiapalace.jpg"
            };

            _restaurants.AddRange(new[] { restaurant1, restaurant2, restaurant3, restaurant4 });

            // Sample Products for Al Angham Restaurant
            _products.Add(new Product
            {
                ProductID = _nextProductId++,
                RestaurantID = 1,
                Name = "Shuwa",
                Description = "Traditional slow-cooked lamb marinated in Omani spices",
                Price = 12.50m,
                Category = "Main Course",
                Stock = 20,
                IsAvailable = true,
                ImageUrl = "/images/products/shuwa.jpg"
            });

            _products.Add(new Product
            {
                ProductID = _nextProductId++,
                RestaurantID = 1,
                Name = "Majboos",
                Description = "Spiced rice with chicken or lamb",
                Price = 8.00m,
                Category = "Main Course",
                Stock = 30,
                IsAvailable = true,
                ImageUrl = "/images/products/majboos.jpg"
            });

            _products.Add(new Product
            {
                ProductID = _nextProductId++,
                RestaurantID = 1,
                Name = "Halwa",
                Description = "Traditional Omani sweet dessert",
                Price = 3.50m,
                Category = "Dessert",
                Stock = 50,
                IsAvailable = true,
                ImageUrl = "/images/products/halwa.jpg"
            });

            // Sample Products for Bait Al Luban
            _products.Add(new Product
            {
                ProductID = _nextProductId++,
                RestaurantID = 2,
                Name = "Grilled Kingfish",
                Description = "Fresh kingfish grilled with Omani spices",
                Price = 15.00m,
                Category = "Seafood",
                Stock = 15,
                IsAvailable = true,
                ImageUrl = "/images/products/kingfish.jpg"
            });

            _products.Add(new Product
            {
                ProductID = _nextProductId++,
                RestaurantID = 2,
                Name = "Lobster Masala",
                Description = "Lobster cooked in traditional Omani masala",
                Price = 22.00m,
                Category = "Seafood",
                Stock = 10,
                IsAvailable = true,
                ImageUrl = "/images/products/lobster.jpg"
            });

            _products.Add(new Product
            {
                ProductID = _nextProductId++,
                RestaurantID = 2,
                Name = "Shrimp Biryani",
                Description = "Aromatic rice with shrimp and spices",
                Price = 11.00m,
                Category = "Main Course",
                Stock = 25,
                IsAvailable = true,
                ImageUrl = "/images/products/biryani.jpg"
            });

            // Sample Products for Turkish House
            _products.Add(new Product
            {
                ProductID = _nextProductId++,
                RestaurantID = 3,
                Name = "Lamb Kebab",
                Description = "Grilled lamb kebab with vegetables",
                Price = 9.50m,
                Category = "Main Course",
                Stock = 20,
                IsAvailable = true,
                ImageUrl = "/images/products/kebab.jpg"
            });

            _products.Add(new Product
            {
                ProductID = _nextProductId++,
                RestaurantID = 3,
                Name = "Turkish Pide",
                Description = "Traditional Turkish flatbread with toppings",
                Price = 7.00m,
                Category = "Main Course",
                Stock = 30,
                IsAvailable = true,
                ImageUrl = "/images/products/pide.jpg"
            });

            _products.Add(new Product
            {
                ProductID = _nextProductId++,
                RestaurantID = 3,
                Name = "Baklava",
                Description = "Sweet pastry with nuts and honey",
                Price = 4.00m,
                Category = "Dessert",
                Stock = 40,
                IsAvailable = true,
                ImageUrl = "/images/products/baklava.jpg"
            });

            // Sample Products for India Palace
            _products.Add(new Product
            {
                ProductID = _nextProductId++,
                RestaurantID = 4,
                Name = "Chicken Tikka Masala",
                Description = "Tender chicken in creamy tomato sauce",
                Price = 10.00m,
                Category = "Main Course",
                Stock = 25,
                IsAvailable = true,
                ImageUrl = "/images/products/tikka.jpg"
            });

            _products.Add(new Product
            {
                ProductID = _nextProductId++,
                RestaurantID = 4,
                Name = "Butter Naan",
                Description = "Soft Indian bread with butter",
                Price = 2.50m,
                Category = "Bread",
                Stock = 50,
                IsAvailable = true,
                ImageUrl = "/images/products/naan.jpg"
            });

            _products.Add(new Product
            {
                ProductID = _nextProductId++,
                RestaurantID = 4,
                Name = "Gulab Jamun",
                Description = "Sweet milk balls in sugar syrup",
                Price = 3.00m,
                Category = "Dessert",
                Stock = 35,
                IsAvailable = true,
                ImageUrl = "/images/products/gulab.jpg"
            });
        }

        // Restaurant methods
        public List<Restaurant> GetAllRestaurants()
        {
            lock (_lock)
            {
                return new List<Restaurant>(_restaurants);
            }
        }

        public Restaurant? GetRestaurantById(int id)
        {
            lock (_lock)
            {
                return _restaurants.FirstOrDefault(r => r.RestaurantID == id);
            }
        }

        public List<Restaurant> GetActiveRestaurants()
        {
            lock (_lock)
            {
                return _restaurants.Where(r => r.IsActive).ToList();
            }
        }

        // Product methods
        public List<Product> GetAllProducts()
        {
            lock (_lock)
            {
                return new List<Product>(_products);
            }
        }

        public Product? GetProductById(int id)
        {
            lock (_lock)
            {
                return _products.FirstOrDefault(p => p.ProductID == id);
            }
        }

        public List<Product> GetProductsByRestaurant(int restaurantId)
        {
            lock (_lock)
            {
                return _products.Where(p => p.RestaurantID == restaurantId).ToList();
            }
        }

        public List<Product> GetProductsByCategory(string category)
        {
            lock (_lock)
            {
                return _products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        // Order methods
        public List<Order> GetAllOrders()
        {
            lock (_lock)
            {
                return new List<Order>(_orders);
            }
        }

        public Order? GetOrderById(int id)
        {
            lock (_lock)
            {
                return _orders.FirstOrDefault(o => o.OrderID == id);
            }
        }

        public List<Order> GetOrdersByUser(string userId)
        {
            lock (_lock)
            {
                return _orders.Where(o => o.UserID == userId).OrderByDescending(o => o.OrderDate).ToList();
            }
        }

        public Order CreateOrder(Order order)
        {
            lock (_lock)
            {
                order.OrderID = _nextOrderId++;
                order.OrderDate = DateTime.Now;
                order.Status = OrderStatus.Pending;
                _orders.Add(order);
                return order;
            }
        }

        public bool UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            lock (_lock)
            {
                var order = _orders.FirstOrDefault(o => o.OrderID == orderId);
                if (order == null) return false;

                order.UpdateStatus(newStatus);
                return true;
            }
        }

        public bool CancelOrder(int orderId)
        {
            lock (_lock)
            {
                var order = _orders.FirstOrDefault(o => o.OrderID == orderId);
                if (order == null) return false;

                if (!order.CanCancel())
                    throw new InvalidOperationException("Order cannot be cancelled at this stage.");

                order.Cancel();
                return true;
            }
        }
    }
}
