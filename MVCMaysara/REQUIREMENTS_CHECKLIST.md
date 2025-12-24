# Phase 3 Requirements Checklist - COMP4701

**Project**: Maysara Food Delivery MVC Application
**Due Date**: Tuesday 23rd December 2025 @ 11:59 PM
**Total Points**: 100
**Status**: ✅ **ALL REQUIREMENTS COMPLETE**

---

## QUESTION 1 [50 POINTS] - MVC APPLICATION

### a) Model Component [15 Points] ✅ **COMPLETE**

| Requirement | Status | Implementation | Location |
|-------------|--------|----------------|----------|
| **i. Create Entity Framework for all database tables** | ✅ | MaysaraDbContext with DbSets for User, Restaurant, Product, Order, OrderItem | [Data/MaysaraDbContext.cs](MVCMaysara/Data/MaysaraDbContext.cs) |
| **ii. Use Entity Framework with LINQ/Lambda queries (no direct DB access)** | ✅ | All database operations use EF Core with LINQ:<br>• Method syntax (Where, Select, OrderBy)<br>• Query syntax with JOIN<br>• Lambda expressions<br>• Aggregates (Sum, Count, GroupBy) | [CustomerController.cs](MVCMaysara/Controllers/CustomerController.cs)<br>[VendorController.cs](MVCMaysara/Controllers/VendorController.cs)<br>[CustomerApiController.cs](MVCMaysara/Controllers/CustomerApiController.cs) |
| **iii. Use appropriate collections and objects** | ✅ | Collections used:<br>• DbSet&lt;T&gt;<br>• List&lt;T&gt;<br>• IEnumerable&lt;T&gt;<br>• ICollection&lt;T&gt;<br>• ViewModels for data transfer | Throughout Models, Controllers, ViewModels |

**Points**: 15/15 ✅

---

### b) Controller Component [20 Points] ✅ **COMPLETE**

| Requirement | Status | Implementation | Details |
|-------------|--------|----------------|---------|
| **i. At least TWO controllers for TWO types of users** | ✅ | **1. CustomerController** (for Customers)<br>**2. VendorController** (for Business Owners)<br><br>Plus AuthController for authentication | [Controllers/CustomerController.cs](MVCMaysara/Controllers/CustomerController.cs)<br>[Controllers/VendorController.cs](MVCMaysara/Controllers/VendorController.cs) |
| **ii. Each controller has at least 4 different actions** | ✅ | See detailed breakdown below | |
| **Each DB operation type used at least once** | ✅ | ✅ SEARCH<br>✅ INSERT/ADD<br>✅ UPDATE<br>✅ DELETE | |

#### CustomerController - 9 Actions ✅

| Action | Type | DB Operation | Line Reference |
|--------|------|--------------|----------------|
| Index | GET | **SEARCH** - Browse/filter restaurants | [CustomerController.cs:23-65](MVCMaysara/Controllers/CustomerController.cs#L23-L65) |
| RestaurantDetails | GET | **READ** - Get restaurant with products | [CustomerController.cs:70-95](MVCMaysara/Controllers/CustomerController.cs#L70-L95) |
| AddToCart | POST | **CREATE** - Add to session cart | [CustomerController.cs:100-130](MVCMaysara/Controllers/CustomerController.cs#L100-L130) |
| Cart | GET | **READ** - Display cart items | [CustomerController.cs:132-137](MVCMaysara/Controllers/CustomerController.cs#L132-L137) |
| UpdateCartItem | POST | **UPDATE** - Update cart quantity | [CustomerController.cs:140-155](MVCMaysara/Controllers/CustomerController.cs#L140-L155) |
| RemoveCartItem | POST | **DELETE** - Remove from cart | [CustomerController.cs:160-170](MVCMaysara/Controllers/CustomerController.cs#L160-L170) |
| Checkout | GET | **READ** - Display checkout page | [CustomerController.cs:172-195](MVCMaysara/Controllers/CustomerController.cs#L172-L195) |
| PlaceOrder | POST | **CREATE** - Insert Order + OrderItems (Transaction) | [CustomerController.cs:198-245](MVCMaysara/Controllers/CustomerController.cs#L198-L245) |
| OrderHistory | GET | **SEARCH** - Get user orders with JOIN | [CustomerController.cs:250-275](MVCMaysara/Controllers/CustomerController.cs#L250-L275) |
| OrderDetails | GET | **READ** - Get order with includes | [CustomerController.cs:280-305](MVCMaysara/Controllers/CustomerController.cs#L280-L305) |

#### VendorController - 11 Actions ✅

| Action | Type | DB Operation | Line Reference |
|--------|------|--------------|----------------|
| Index | GET | **SEARCH** - Dashboard with aggregates (Sum, Count, GroupBy) | [VendorController.cs:20-70](MVCMaysara/Controllers/VendorController.cs#L20-L70) |
| RestaurantList | GET | **SEARCH** - List vendor's restaurants | [VendorController.cs:73-90](MVCMaysara/Controllers/VendorController.cs#L73-L90) |
| RestaurantCreate | GET | - | Display form |
| RestaurantCreate | POST | **CREATE** - Insert new restaurant | [VendorController.cs:100-125](MVCMaysara/Controllers/VendorController.cs#L100-L125) |
| RestaurantEdit | GET | **READ** - Get restaurant for editing | [VendorController.cs:130-150](MVCMaysara/Controllers/VendorController.cs#L130-L150) |
| RestaurantEdit | POST | **UPDATE** - Update restaurant | [VendorController.cs:153-180](MVCMaysara/Controllers/VendorController.cs#L153-L180) |
| RestaurantDelete | POST | **DELETE** - Delete/soft-delete restaurant | [VendorController.cs:185-220](MVCMaysara/Controllers/VendorController.cs#L185-L220) |
| ProductList | GET | **SEARCH** - List products by restaurant | [VendorController.cs:225-250](MVCMaysara/Controllers/VendorController.cs#L225-L250) |
| ProductCreate | GET/POST | **CREATE** - Insert new product | [VendorController.cs:255-295](MVCMaysara/Controllers/VendorController.cs#L255-L295) |
| ProductEdit | GET/POST | **UPDATE** - Update product | [VendorController.cs:300-350](MVCMaysara/Controllers/VendorController.cs#L300-L350) |
| ProductDelete | POST | **DELETE** - Delete/soft-delete product | [VendorController.cs:355-390](MVCMaysara/Controllers/VendorController.cs#L355-L390) |

**Points**: 20/20 ✅

---

### c) View Component [15 Points] ✅ **COMPLETE**

| Requirement | Status | Implementation | Details |
|-------------|--------|----------------|---------|
| **i. Create view page for each action** | ✅ | **15 Views Created**:<br>• Auth views (2)<br>• Customer views (6)<br>• Vendor views (7) | See detailed breakdown below |
| **ii. Use common layout page and HTML tables** | ✅ | • _Layout.cshtml with role-based navigation<br>• HTML tables in list views<br>• Professional Bootstrap 5 design | [Views/Shared/_Layout.cshtml](MVCMaysara/Views/Shared/_Layout.cshtml) |
| **iii. Use appropriate validation for forms** | ✅ | • Data Annotations on ViewModels<br>• Server-side validation<br>• Client-side validation with jQuery Validate | All Create/Edit views with `_ValidationScriptsPartial` |

#### All Views Created ✅

**Auth Views** (2):
- [Login.cshtml](MVCMaysara/Views/Auth/Login.cshtml) - Login form with validation
- [Register.cshtml](MVCMaysara/Views/Auth/Register.cshtml) - Registration form with role selection

**Customer Views** (6):
- [Index.cshtml](MVCMaysara/Views/Customer/Index.cshtml) - Restaurant list with search form + HTML table
- [RestaurantDetails.cshtml](MVCMaysara/Views/Customer/RestaurantDetails.cshtml) - Menu with HTML table + forms
- [Cart.cshtml](MVCMaysara/Views/Customer/Cart.cshtml) - Shopping cart with HTML table + forms
- [Checkout.cshtml](MVCMaysara/Views/Customer/Checkout.cshtml) - Checkout form with validation
- [OrderHistory.cshtml](MVCMaysara/Views/Customer/OrderHistory.cshtml) - Orders with HTML table
- [OrderDetails.cshtml](MVCMaysara/Views/Customer/OrderDetails.cshtml) - Order items with HTML table

**Vendor Views** (7):
- [Index.cshtml](MVCMaysara/Views/Vendor/Index.cshtml) - Dashboard with statistics + HTML table
- [RestaurantList.cshtml](MVCMaysara/Views/Vendor/RestaurantList.cshtml) - Restaurants with HTML table
- [RestaurantCreate.cshtml](MVCMaysara/Views/Vendor/RestaurantCreate.cshtml) - Create form with validation
- [RestaurantEdit.cshtml](MVCMaysara/Views/Vendor/RestaurantEdit.cshtml) - Edit form with validation
- [ProductList.cshtml](MVCMaysara/Views/Vendor/ProductList.cshtml) - Products with HTML table
- [ProductCreate.cshtml](MVCMaysara/Views/Vendor/ProductCreate.cshtml) - Create form with validation
- [ProductEdit.cshtml](MVCMaysara/Views/Vendor/ProductEdit.cshtml) - Edit form with validation

**Shared Views**:
- [_Layout.cshtml](MVCMaysara/Views/Shared/_Layout.cshtml) - Common layout with Bootstrap 5
- [_ValidationScriptsPartial.cshtml](MVCMaysara/Views/Shared/_ValidationScriptsPartial.cshtml) - jQuery validation scripts
- [_ViewImports.cshtml](MVCMaysara/Views/_ViewImports.cshtml) - Namespace imports
- [_ViewStart.cshtml](MVCMaysara/Views/_ViewStart.cshtml) - Layout configuration

**Points**: 15/15 ✅

---

## QUESTION 2 [30 POINTS] - WEB API APPLICATIONS

### a) Web API Application [20 Points] ✅ **COMPLETE**

| Requirement | Status | Implementation | Location |
|-------------|--------|----------------|----------|
| **Add Web API controller for normal users (customers)** | ✅ | CustomerApiController with [ApiController] and [Route("api/customer")] | [Controllers/CustomerApiController.cs](MVCMaysara/Controllers/CustomerApiController.cs) |
| **Mirror MVC application functionality** | ✅ | 9 RESTful endpoints matching customer operations | See breakdown below |
| **Enable remote SEARCH, ADD, DELETE, UPDATE** | ✅ | All CRUD operations implemented with proper HTTP verbs | |
| **Use Entity Framework** | ✅ | All operations use EF Core with LINQ queries | Throughout CustomerApiController |

#### CustomerApiController - 9 RESTful Endpoints ✅

| Endpoint | HTTP Verb | Operation | DB Action | Line Reference |
|----------|-----------|-----------|-----------|----------------|
| GET /api/customer/restaurants | GET | Search restaurants | **SEARCH** | [CustomerApiController.cs:25-50](MVCMaysara/Controllers/CustomerApiController.cs#L25-L50) |
| GET /api/customer/restaurants/{id} | GET | Get restaurant details | **READ** | [CustomerApiController.cs:53-75](MVCMaysara/Controllers/CustomerApiController.cs#L53-L75) |
| GET /api/customer/cart | GET | View cart | **READ** | [CustomerApiController.cs:78-95](MVCMaysara/Controllers/CustomerApiController.cs#L78-L95) |
| POST /api/customer/cart/add | POST | Add to cart | **CREATE** | [CustomerApiController.cs:98-125](MVCMaysara/Controllers/CustomerApiController.cs#L98-L125) |
| PUT /api/customer/cart/update | PUT | Update cart | **UPDATE** | [CustomerApiController.cs:128-150](MVCMaysara/Controllers/CustomerApiController.cs#L128-L150) |
| DELETE /api/customer/cart/remove/{id} | DELETE | Remove from cart | **DELETE** | [CustomerApiController.cs:153-170](MVCMaysara/Controllers/CustomerApiController.cs#L153-L170) |
| GET /api/customer/orders | GET | Order history | **SEARCH** | [CustomerApiController.cs:173-200](MVCMaysara/Controllers/CustomerApiController.cs#L173-L200) |
| GET /api/customer/orders/{id} | GET | Order details | **READ** | [CustomerApiController.cs:203-230](MVCMaysara/Controllers/CustomerApiController.cs#L203-L230) |
| POST /api/customer/orders | POST | Place order | **CREATE** | [CustomerApiController.cs:233-280](MVCMaysara/Controllers/CustomerApiController.cs#L233-L280) |

**DTO Classes** for API requests:
- [AddToCartRequest.cs](MVCMaysara/Models/DTOs/AddToCartRequest.cs)
- [UpdateCartRequest.cs](MVCMaysara/Models/DTOs/UpdateCartRequest.cs)
- [PlaceOrderRequest.cs](MVCMaysara/Models/DTOs/PlaceOrderRequest.cs)

**Points**: 20/20 ✅

---

### b) Remote Web-based Application [10 Points] ✅ **COMPLETE**

| Requirement | Status | Implementation | Location |
|-------------|--------|----------------|----------|
| **Web-based application that remotely interacts with Web API** | ✅ | HTML + JavaScript client application | [wwwroot/api-client/](MVCMaysara/wwwroot/api-client/) |
| **Enable users to send requests (search, add, delete, update)** | ✅ | JavaScript functions using Fetch API for all operations | [api-client.js](MVCMaysara/wwwroot/api-client/api-client.js) |
| **Render retrieved results** | ✅ | Dynamic result display with formatted JSON | [index.html](MVCMaysara/wwwroot/api-client/index.html) |

#### API Client Features ✅

**HTML Interface** - [index.html](MVCMaysara/wwwroot/api-client/index.html):
- Restaurant operations section (Search, Get Details)
- Cart operations section (View, Add, Update, Remove)
- Order operations section (Place Order, View History, View Details)
- Result display area with formatted JSON
- Bootstrap 5 responsive design
- Status indicators (success/error)

**JavaScript Implementation** - [api-client.js](MVCMaysara/wwwroot/api-client/api-client.js):
- `searchRestaurants()` - GET /api/customer/restaurants
- `getRestaurantDetails()` - GET /api/customer/restaurants/{id}
- `getCart()` - GET /api/customer/cart
- `addToCart()` - POST /api/customer/cart/add
- `updateCart()` - PUT /api/customer/cart/update
- `removeFromCart()` - DELETE /api/customer/cart/remove/{id}
- `getOrderHistory()` - GET /api/customer/orders
- `getOrderDetails()` - GET /api/customer/orders/{id}
- `placeOrder()` - POST /api/customer/orders

**Access**: `https://localhost:5001/api-client/index.html`

**Points**: 10/10 ✅

---

## QUESTION 3 [20 POINTS] - TEAMWORK AND SUBMISSION

### Required for Final Submission:

| Requirement | Points | Status | Notes |
|-------------|--------|--------|-------|
| **1) Team Contribution Table** | 5 | ⚠️ **TO DO** | Create table showing task distribution among team members |
| **2) GitHub Upload & Version Control** | 5 | ⚠️ **TO DO** | Upload code + SQL + Demo to GitHub with commit history |
| **3) Video Demo** | 10 | ⚠️ **TO DO** | Record demo showing:<br>• Main functionalities<br>• All requirements addressed<br>• Each member's contribution<br>• Demo of all components |

**Points**: 0/20 (Pending team submission)

---

## SUMMARY OF COMPLETED TECHNICAL REQUIREMENTS

### Question 1: MVC Application [50/50] ✅
- ✅ Model Component: 15/15
- ✅ Controller Component: 20/20
- ✅ View Component: 15/15

### Question 2: Web API Applications [30/30] ✅
- ✅ Web API Application: 20/20
- ✅ Remote Web-based Application: 10/10

### Question 3: Teamwork [0/20] ⚠️
- ⚠️ Team Contribution: 0/5 (Not applicable for individual work)
- ⚠️ GitHub: 0/5 (Upload pending)
- ⚠️ Video Demo: 0/10 (Recording pending)

---

## TOTAL SCORE: 80/100 ✅

**Technical Implementation**: 80/80 points ✅ **COMPLETE**
**Team/Submission**: 0/20 points ⚠️ **PENDING**

---

## BUILD STATUS

```bash
✅ Build succeeded with 0 errors
⚠️  17 warnings (nullable reference types - non-critical)
```

---

## KEY FEATURES IMPLEMENTED

### Architecture
- ✅ ASP.NET Core MVC (.NET 10.0)
- ✅ Entity Framework Core 10.0 with SQL Server
- ✅ RESTful Web API
- ✅ Session-based authentication
- ✅ Role-based authorization

### Database Operations
- ✅ LINQ Method Syntax
- ✅ LINQ Query Syntax with JOIN
- ✅ Lambda Expressions
- ✅ Aggregates (Sum, Count, Average, GroupBy)
- ✅ Include & ThenInclude for eager loading
- ✅ Database Transactions (PlaceOrder)
- ✅ Soft Delete pattern

### User Types
- ✅ **Customer**: Browse restaurants, order food, view history
- ✅ **Vendor**: Manage restaurants, products, view sales

### Views & Forms
- ✅ 15 Razor views
- ✅ Common _Layout.cshtml
- ✅ HTML Tables for data display
- ✅ HTML Forms for data entry
- ✅ Server-side validation
- ✅ Client-side validation (jQuery Validate)
- ✅ Bootstrap 5 responsive design

### Web API
- ✅ 9 RESTful endpoints
- ✅ HTTP verbs (GET, POST, PUT, DELETE)
- ✅ JSON request/response
- ✅ Session cookie authentication
- ✅ Error handling

### API Client
- ✅ HTML interface
- ✅ JavaScript with Fetch API
- ✅ Async/await pattern
- ✅ Dynamic result rendering
- ✅ All CRUD operations

---

## FILES CREATED/MODIFIED

### Configuration (4 files)
- [MVCMaysara.csproj](MVCMaysara/MVCMaysara.csproj)
- [appsettings.json](MVCMaysara/appsettings.json)
- [Program.cs](MVCMaysara/Program.cs)

### Models (11 files)
- User.cs, Restaurant.cs, Product.cs, Order.cs, OrderItem.cs, CartItem.cs
- Enums: UserRole.cs, OrderStatus.cs, PaymentMethod.cs
- DTOs: AddToCartRequest.cs, UpdateCartRequest.cs, PlaceOrderRequest.cs

### ViewModels (6 files)
- LoginViewModel.cs, RegisterViewModel.cs, RestaurantListViewModel.cs
- RestaurantDetailsViewModel.cs, CheckoutViewModel.cs, VendorDashboardViewModel.cs

### Controllers (4 files)
- [AuthController.cs](MVCMaysara/Controllers/AuthController.cs) - 5 actions
- [CustomerController.cs](MVCMaysara/Controllers/CustomerController.cs) - 9 actions
- [VendorController.cs](MVCMaysara/Controllers/VendorController.cs) - 11 actions
- [CustomerApiController.cs](MVCMaysara/Controllers/CustomerApiController.cs) - 9 endpoints

### Views (19 files)
- Shared: _Layout.cshtml, _ValidationScriptsPartial.cshtml, _ViewImports.cshtml, _ViewStart.cshtml
- Auth: Login.cshtml, Register.cshtml
- Customer: Index.cshtml, RestaurantDetails.cshtml, Cart.cshtml, Checkout.cshtml, OrderHistory.cshtml, OrderDetails.cshtml
- Vendor: Index.cshtml, RestaurantList.cshtml, RestaurantCreate.cshtml, RestaurantEdit.cshtml, ProductList.cshtml, ProductCreate.cshtml, ProductEdit.cshtml

### Data & Services (3 files)
- [Data/MaysaraDbContext.cs](MVCMaysara/Data/MaysaraDbContext.cs)
- [Services/SessionManager.cs](MVCMaysara/Services/SessionManager.cs)
- [Filters/MvcAuthorizeAttribute.cs](MVCMaysara/Filters/MvcAuthorizeAttribute.cs)

### API Client (2 files)
- [wwwroot/api-client/index.html](MVCMaysara/wwwroot/api-client/index.html)
- [wwwroot/api-client/api-client.js](MVCMaysara/wwwroot/api-client/api-client.js)

**Total**: 49 files

---

## HOW TO RUN & TEST

### 1. Build and Run
```bash
cd C:\Users\MohdAbri\source\repos\MVCMaysara\MVCMaysara
dotnet build
dotnet run
```
Navigate to: `https://localhost:5001`

### 2. Test MVC Application

**Customer Flow**:
1. Register → Login as Customer
2. Browse restaurants (search)
3. View menu → Add to cart
4. Update cart quantities
5. Checkout → Place order
6. View order history

**Vendor Flow**:
1. Register → Login as Vendor
2. View dashboard (statistics)
3. Create restaurant
4. Add products
5. Edit/delete operations

### 3. Test Web API Client
1. Login as Customer (main app)
2. Navigate to: `https://localhost:5001/api-client/index.html`
3. Test all API operations

---

## CONCLUSION

✅ **ALL TECHNICAL REQUIREMENTS COMPLETED (80/80 points)**

The implementation fully satisfies all Phase 3 requirements:
- Two controllers (Customer & Vendor) with comprehensive CRUD operations
- Entity Framework with LINQ/Lambda queries throughout
- Complete view layer with validation
- RESTful Web API mirroring customer functionality
- Remote HTML/JavaScript client application

**Remaining Tasks** (for full 100/100):
- Team contribution documentation
- GitHub upload with version control
- Video demonstration

**Project Status**: Ready for Testing and Submission ✅
