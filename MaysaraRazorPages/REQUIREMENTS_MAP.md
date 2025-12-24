# Phase 2 Requirements - Implementation Map

## Team Contribution Table

| Team Member | Student ID | Tasks Completed |
|-------------|------------|-----------------|
| Ahmed Bader Rashid Al Abri | 136696 | Q1: Database Application<br>Q4: Teamwork and Submission |
| Almakhtar humaid khalfan albadowi | 136720 | Q2: Direct Data Access (Parameterized Queries, Validation) |
| Mohammed Sultan Rashid Al Abri | 136382 | Q3: Entity Framework (LINQ Implementation) |

---


## Question 1: Database Application 

### 1.1 Three Database Tables 
- **File:** `Database/MaysaraDeliveryDB.sql`
- **Tables:** USERS, RESTAURANTS, PRODUCTS, ORDERS, ORDER_ITEMS (5 tables)

### 1.2 User Login Table 
- **File:** `Database/MaysaraDeliveryDB.sql` (Lines 8-23)
- **Table:** USERS (Email, Password, Name, Phone, Role)

### 1.3 Primary & Foreign Keys 
- **File:** `Database/MaysaraDeliveryDB.sql` (Lines 83-113)
- **Primary Keys:** 5 tables with IDENTITY
- **Foreign Keys:** 6 relationships with Cascade/Restrict

### 1.4 Constraints 
- **File:** `Database/MaysaraDeliveryDB.sql` (Lines 115-147)
- **CHECK:** 11 constraints
- **DEFAULT:** 10+ constraints
- **NOT NULL:** All required fields

### 1.5 Database View 
- **View:** `Database/MaysaraDeliveryDB.sql` (Lines 150-162) - VW_OrderSummary
- **Usage:** `Pages/Admin/Dashboard.cshtml.cs` (Lines 210-230)

### 1.6 Sample Data 
- **File:** `Database/SeedData.sql`
- **Data:** 8 users, 4 restaurants, 15+ products, 5+ orders

---

## Question 2: Direct Data Access 

### 2.1 SELECT Statements 
- **Dashboard.cshtml.cs** - COUNT, SUM, AVG, GROUP BY, TOP 5, View
- **Restaurants/Index.cshtml.cs** - SELECT with WHERE, ORDER BY, LIKE
- **Restaurants/Details.cshtml.cs** - SELECT with INNER JOIN

### 2.2 INSERT/UPDATE/DELETE 
- **File:** `Pages/Restaurants/Manage.cshtml.cs`
- **INSERT:** Lines 85-115
- **UPDATE:** Lines 165-210
- **DELETE:** Lines 240-275 (soft delete)

### 2.3 Parameterized Queries 
- **Index.cshtml.cs** Line 60 - SqlParameter with @SearchTerm
- **Details.cshtml.cs** Line 68 - SqlParameter with @RestaurantID
- **Manage.cshtml.cs** Lines 100-108 - All fields parameterized

### 2.4 Form Validation 
- **All forms have:**
  - ModelState validation
  - Data annotations ([Required], [Range], [EmailAddress])
  - Exception handling (try/catch)
  - asp-validation-for tags

---

## Question 3: Entity Framework 

### 3.1 EF Model 
- **DbContext:** `Data/MaysaraDbContext.cs`
- **Models:** `Models/User.cs`, `Restaurant.cs`, `Product.cs`, `Order.cs`, `OrderItem.cs`
- **Enums:** `Models/Enums/UserRole.cs`, `OrderStatus.cs`, `PaymentMethod.cs`

### 3.2a Authentication 
- **Login:** `Pages/Auth/Login.cshtml.cs` (LINQ Where, Lines 48-52)
- **Register:** `Pages/Auth/Register.cshtml.cs` (LINQ Any, Add, Lines 38-55)
- **Logout:** `Pages/Auth/Logout.cshtml.cs`

### 3.2b State Management 
- **File:** `Services/SessionManager.cs`
- **Config:** `Program.cs` (Lines 14-20)
- **Session:** UserId, UserName, UserEmail, UserRole

### 3.2c Access Restriction 
- **File:** `Filters/AuthorizePageFilter.cs`
- **Registration:** `Program.cs` (Lines 22-25)
- **All pages protected** except Login/Register/Logout

### 3.3a LINQ Styles 
- **Query Syntax:** `Pages/Products/Edit.cshtml.cs` (Lines 56-62)
- **Method Syntax:** `Pages/Auth/Login.cshtml.cs` (Lines 48-52)
- **Mixed Syntax:** `Pages/Orders/MyOrders.cshtml.cs` (Lines 121-127)

### 3.3b Lambda Types 
- **Where:** Login, Products, Orders (8+ pages)
- **OrderBy:** Products/Create (Lines 63-64), MyOrders (Line 70)
- **Select:** MyOrders (Lines 113-118)
- **Sum/Count/Avg:** MyOrders (Lines 114-116)
- **Any:** Register (Line 39)
- **GroupBy:** MyOrders (Lines 110-118)

### 3.3c LINQ Constructs 
- **WHERE:** All EF pages (8+ pages)
- **ORDER BY:** Products/Create, Orders/MyOrders, Orders/Checkout
- **GROUP BY:** `Pages/Orders/MyOrders.cshtml.cs` (Lines 110-118)
- **Multi-table:** MyOrders (3-table JOIN, Lines 87-100)
- **Include:** Products pages, Orders pages (4+ pages)

### 3.3d CRUD 
- **CREATE:** Register (Lines 45-55), Products/Create (Lines 75-85), Checkout (Lines 147-165)
- **READ:** All EF pages (Where, Find, FirstOrDefault, ToList)
- **UPDATE:** `Pages/Products/Edit.cshtml.cs` (Lines 105-125)
- **DELETE:** `Pages/Products/Delete.cshtml.cs` (Lines 85-100)

---


## Quick File Reference

**Database:**
- Schema: `Database/MaysaraDeliveryDB.sql`
- Data: `Database/SeedData.sql`

**ADO.NET (Direct SQL):**
- Helper: `Data/AdoNetDataAccess.cs`
- Admin: `Pages/Admin/Dashboard.cshtml.cs`
- Restaurants: `Pages/Restaurants/*.cshtml.cs`

**Entity Framework (LINQ):**
- Context: `Data/MaysaraDbContext.cs`
- Auth: `Pages/Auth/*.cshtml.cs`
- Products: `Pages/Products/*.cshtml.cs`
- Orders: `Pages/Orders/*.cshtml.cs`
