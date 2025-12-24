-- =============================================
-- Maysara Delivery Hub - Database Schema
-- COMP4701 - Phase 2
-- =============================================

-- Create Database
CREATE DATABASE MaysaraDeliveryDB;
GO

USE MaysaraDeliveryDB;
GO

-- =============================================
-- TABLE 1: USERS
-- Stores all users (Customers, Vendors, Admin)
-- =============================================
CREATE TABLE USERS (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Address NVARCHAR(300) NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'Customer',
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    -- Constraints
    CONSTRAINT CHK_User_Role CHECK (Role IN ('Customer', 'Vendor', 'Admin')),
    CONSTRAINT CHK_User_Email CHECK (Email LIKE '%@%')
);

-- =============================================
-- TABLE 2: RESTAURANTS
-- Stores restaurant information
-- =============================================
CREATE TABLE RESTAURANTS (
    RestaurantID INT IDENTITY(1,1) PRIMARY KEY,
    OwnerID INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Address NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Rating DECIMAL(3,2) DEFAULT 0.00,
    DeliveryFee DECIMAL(10,2) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    -- Foreign Key
    CONSTRAINT FK_Restaurant_Owner FOREIGN KEY (OwnerID) REFERENCES USERS(UserID),

    -- Constraints
    CONSTRAINT CHK_Restaurant_Rating CHECK (Rating >= 0 AND Rating <= 5),
    CONSTRAINT CHK_Restaurant_DeliveryFee CHECK (DeliveryFee >= 0)
);

-- =============================================
-- TABLE 3: PRODUCTS
-- Stores menu items/products from restaurants
-- =============================================
CREATE TABLE PRODUCTS (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    RestaurantID INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    Price DECIMAL(10,2) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    Stock INT DEFAULT 999,
    IsAvailable BIT DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    -- Foreign Key with Cascade Delete
    CONSTRAINT FK_Product_Restaurant FOREIGN KEY (RestaurantID)
        REFERENCES RESTAURANTS(RestaurantID) ON DELETE CASCADE,

    -- Constraints
    CONSTRAINT CHK_Product_Price CHECK (Price > 0),
    CONSTRAINT CHK_Product_Stock CHECK (Stock >= 0)
);

-- =============================================
-- TABLE 4: ORDERS
-- Stores customer orders
-- =============================================
CREATE TABLE ORDERS (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    RestaurantID INT NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    DeliveryAddress NVARCHAR(300) NOT NULL,
    PaymentMethod NVARCHAR(20) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),

    -- Foreign Keys
    CONSTRAINT FK_Order_User FOREIGN KEY (UserID) REFERENCES USERS(UserID),
    CONSTRAINT FK_Order_Restaurant FOREIGN KEY (RestaurantID) REFERENCES RESTAURANTS(RestaurantID),

    -- Constraints
    CONSTRAINT CHK_Order_TotalAmount CHECK (TotalAmount > 0),
    CONSTRAINT CHK_Order_PaymentMethod CHECK (PaymentMethod IN ('Cash', 'CreditCard', 'DebitCard')),
    CONSTRAINT CHK_Order_Status CHECK (Status IN ('Pending', 'Confirmed', 'Preparing', 'OutForDelivery', 'Delivered', 'Cancelled'))
);

-- =============================================
-- TABLE 5: ORDER_ITEMS
-- Stores individual items within each order
-- =============================================
CREATE TABLE ORDER_ITEMS (
    OrderItemID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    ProductName NVARCHAR(100) NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    Quantity INT NOT NULL,
    Subtotal AS (UnitPrice * Quantity) PERSISTED,

    -- Foreign Keys with Cascade Delete
    CONSTRAINT FK_OrderItem_Order FOREIGN KEY (OrderID)
        REFERENCES ORDERS(OrderID) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItem_Product FOREIGN KEY (ProductID)
        REFERENCES PRODUCTS(ProductID),

    -- Constraints
    CONSTRAINT CHK_OrderItem_Quantity CHECK (Quantity > 0),
    CONSTRAINT CHK_OrderItem_UnitPrice CHECK (UnitPrice > 0)
);

-- =============================================
-- VIEW: Order Summary with Restaurant and User Details
-- Joins ORDERS, USERS, and RESTAURANTS for reporting
-- =============================================
GO
CREATE VIEW VW_OrderSummary AS
SELECT
    o.OrderID,
    o.OrderDate,
    o.Status,
    o.TotalAmount,
    o.PaymentMethod,
    o.DeliveryAddress,
    u.UserID,
    u.Name AS CustomerName,
    u.Email AS CustomerEmail,
    u.Phone AS CustomerPhone,
    r.RestaurantID,
    r.Name AS RestaurantName,
    r.Phone AS RestaurantPhone,
    r.DeliveryFee
FROM ORDERS o
INNER JOIN USERS u ON o.UserID = u.UserID
INNER JOIN RESTAURANTS r ON o.RestaurantID = r.RestaurantID;
GO

-- =============================================
-- INDEXES for Performance
-- =============================================
CREATE INDEX IX_Products_Restaurant ON PRODUCTS(RestaurantID);
CREATE INDEX IX_Orders_User ON ORDERS(UserID);
CREATE INDEX IX_Orders_Restaurant ON ORDERS(RestaurantID);
CREATE INDEX IX_Orders_Date ON ORDERS(OrderDate DESC);
CREATE INDEX IX_OrderItems_Order ON ORDER_ITEMS(OrderID);

GO

