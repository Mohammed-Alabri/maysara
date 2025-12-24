-- =============================================
-- Maysara Delivery Hub - Sample Data
-- COMP4701 - Phase 2
-- =============================================

USE MaysaraDeliveryDB;
GO

-- =============================================
-- INSERT SAMPLE USERS
-- Password: All passwords are plain text for demo (Password123!)
-- In production, these should be hashed
-- =============================================

-- Admin User
INSERT INTO USERS (Name, Email, Password, Phone, Address, Role)
VALUES ('Admin User', 'admin@maysara.com', 'Admin123!', '+968-9999-9999', 'Muscat, Oman', 'Admin');

-- Vendor Users (Restaurant Owners)
INSERT INTO USERS (Name, Email, Password, Phone, Address, Role) VALUES
('Ahmed Al-Said', 'ahmed.vendor@maysara.com', 'Vendor123!', '+968-9123-4567', 'Muscat City Center, Muscat', 'Vendor'),
('Fatima Al-Balushi', 'fatima.vendor@maysara.com', 'Vendor123!', '+968-9234-5678', 'Mutrah Corniche, Muscat', 'Vendor'),
('Khalid Al-Harthi', 'khalid.vendor@maysara.com', 'Vendor123!', '+968-9345-6789', 'Al Khuwair Street, Muscat', 'Vendor'),
('Layla Al-Farsi', 'layla.vendor@maysara.com', 'Vendor123!', '+968-9456-7890', 'Ruwi High Street, Muscat', 'Vendor');

-- Customer Users
INSERT INTO USERS (Name, Email, Password, Phone, Address, Role) VALUES
('Mohammed Ali', 'mohammed@email.com', 'Customer123!', '+968-9567-8901', 'Al Qurum, Muscat', 'Customer'),
('Sara Ahmed', 'sara@email.com', 'Customer123!', '+968-9678-9012', 'Bousher, Muscat', 'Customer'),
('Ali Hassan', 'ali@email.com', 'Customer123!', '+968-9789-0123', 'Al Seeb, Muscat', 'Customer');

-- =============================================
-- INSERT SAMPLE RESTAURANTS
-- =============================================
INSERT INTO RESTAURANTS (OwnerID, Name, Address, Phone, Rating, DeliveryFee, IsActive) VALUES
(2, 'Al Angham Restaurant', 'Muscat City Center, Muscat', '+968-2444-5555', 4.5, 2.50, 1),
(3, 'Bait Al Luban', 'Mutrah Corniche, Muscat', '+968-2471-4408', 4.7, 3.00, 1),
(4, 'Turkish House Restaurant', 'Al Khuwair Street, Muscat', '+968-2448-3232', 4.3, 2.00, 1),
(5, 'India Palace', 'Ruwi High Street, Muscat', '+968-2470-2020', 4.6, 1.50, 1);

-- =============================================
-- INSERT SAMPLE PRODUCTS
-- =============================================

-- Al Angham Restaurant (RestaurantID = 1) - Traditional Omani
INSERT INTO PRODUCTS (RestaurantID, Name, Description, Price, Category, Stock, IsAvailable) VALUES
(1, 'Shuwa', 'Traditional slow-cooked lamb marinated in Omani spices and wrapped in banana leaves', 12.50, 'Main Course', 20, 1),
(1, 'Majboos', 'Spiced rice with tender chicken or lamb, served with traditional Omani salad', 8.00, 'Main Course', 30, 1),
(1, 'Halwa', 'Traditional Omani sweet dessert made with rosewater, saffron, and nuts', 3.50, 'Dessert', 50, 1),
(1, 'Omani Kahwa', 'Traditional Omani coffee flavored with cardamom, served with dates', 2.00, 'Beverage', 100, 1);

-- Bait Al Luban (RestaurantID = 2) - Omani Seafood
INSERT INTO PRODUCTS (RestaurantID, Name, Description, Price, Category, Stock, IsAvailable) VALUES
(2, 'Grilled Kingfish', 'Fresh kingfish grilled with authentic Omani spices and lemon', 15.00, 'Seafood', 15, 1),
(2, 'Lobster Masala', 'Fresh lobster cooked in traditional Omani masala sauce', 22.00, 'Seafood', 10, 1),
(2, 'Shrimp Biryani', 'Aromatic basmati rice with jumbo shrimp and traditional spices', 11.00, 'Main Course', 25, 1),
(2, 'Fish Tikka', 'Marinated fish pieces grilled to perfection', 9.50, 'Appetizer', 20, 1);

-- Turkish House Restaurant (RestaurantID = 3) - Turkish Cuisine
INSERT INTO PRODUCTS (RestaurantID, Name, Description, Price, Category, Stock, IsAvailable) VALUES
(3, 'Lamb Kebab', 'Grilled lamb kebab served with grilled vegetables and rice', 9.50, 'Main Course', 20, 1),
(3, 'Turkish Pide', 'Traditional Turkish flatbread topped with minced meat and cheese', 7.00, 'Main Course', 30, 1),
(3, 'Baklava', 'Sweet pastry made with layers of filo filled with chopped nuts and honey', 4.00, 'Dessert', 40, 1),
(3, 'Turkish Tea', 'Authentic Turkish black tea served in traditional glass', 1.50, 'Beverage', 100, 1);

-- India Palace (RestaurantID = 4) - Indian Cuisine
INSERT INTO PRODUCTS (RestaurantID, Name, Description, Price, Category, Stock, IsAvailable) VALUES
(4, 'Chicken Tikka Masala', 'Tender chicken pieces in creamy tomato-based curry sauce', 10.00, 'Main Course', 25, 1),
(4, 'Butter Naan', 'Soft Indian bread brushed with butter, baked in tandoor', 2.50, 'Bread', 50, 1),
(4, 'Gulab Jamun', 'Sweet milk-solid balls soaked in aromatic sugar syrup', 3.00, 'Dessert', 35, 1),
(4, 'Biryani Special', 'Fragrant basmati rice with your choice of meat and aromatic spices', 12.00, 'Main Course', 20, 1);

-- =============================================
-- INSERT SAMPLE ORDERS
-- =============================================
INSERT INTO ORDERS (UserID, RestaurantID, TotalAmount, DeliveryAddress, PaymentMethod, Status, OrderDate) VALUES
-- Completed order from 5 days ago
(6, 1, 26.50, 'Al Qurum, House No. 45, Muscat', 'CreditCard', 'Delivered', DATEADD(day, -5, GETDATE())),
-- Completed order from 3 days ago
(7, 2, 48.00, 'Bousher, Villa 12, Muscat', 'Cash', 'Delivered', DATEADD(day, -3, GETDATE())),
-- Order in preparation from 2 hours ago
(8, 3, 20.50, 'Al Seeb, Apartment 5B, Muscat', 'DebitCard', 'Preparing', DATEADD(hour, -2, GETDATE())),
-- Pending order
(6, 4, 17.50, 'Al Qurum, House No. 45, Muscat', 'CreditCard', 'Pending', GETDATE()),
-- Out for delivery
(7, 1, 15.50, 'Bousher, Villa 12, Muscat', 'Cash', 'OutForDelivery', DATEADD(minute, -30, GETDATE()));

-- =============================================
-- INSERT ORDER ITEMS
-- =============================================
-- Order 1: Al Angham (OrderID = 1)
INSERT INTO ORDER_ITEMS (OrderID, ProductID, ProductName, UnitPrice, Quantity) VALUES
(1, 1, 'Shuwa', 12.50, 1),
(1, 2, 'Majboos', 8.00, 1),
(1, 3, 'Halwa', 3.50, 1),
(1, 4, 'Omani Kahwa', 2.00, 1);

-- Order 2: Bait Al Luban (OrderID = 2)
INSERT INTO ORDER_ITEMS (OrderID, ProductID, ProductName, UnitPrice, Quantity) VALUES
(2, 5, 'Grilled Kingfish', 15.00, 2),
(2, 7, 'Shrimp Biryani', 11.00, 1),
(2, 6, 'Lobster Masala', 22.00, 1);

-- Order 3: Turkish House (OrderID = 3)
INSERT INTO ORDER_ITEMS (OrderID, ProductID, ProductName, UnitPrice, Quantity) VALUES
(3, 9, 'Lamb Kebab', 9.50, 2),
(3, 11, 'Baklava', 4.00, 1),
(3, 12, 'Turkish Tea', 1.50, 2);

-- Order 4: India Palace (OrderID = 4)
INSERT INTO ORDER_ITEMS (OrderID, ProductID, ProductName, UnitPrice, Quantity) VALUES
(4, 13, 'Chicken Tikka Masala', 10.00, 1),
(4, 14, 'Butter Naan', 2.50, 2),
(4, 15, 'Gulab Jamun', 3.00, 1);

-- Order 5: Al Angham (OrderID = 5)
INSERT INTO ORDER_ITEMS (OrderID, ProductID, ProductName, UnitPrice, Quantity) VALUES
(5, 2, 'Majboos', 8.00, 1),
(5, 3, 'Halwa', 3.50, 2);

GO