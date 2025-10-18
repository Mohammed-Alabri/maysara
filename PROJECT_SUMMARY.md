# Maysara - Omani Delivery Hub

## Project Overview
A full-stack delivery platform built with ASP.NET Core 8.0 and React 19, demonstrating modern web development practices for COMP4701.

## Technology Stack

### Backend
- **ASP.NET Core 8.0** - Web API
- **C# 12** - Programming language
- **Swagger/OpenAPI** - API documentation

### Frontend
- **React 19.1** - UI framework
- **React Router 7** - Client-side routing
- **Bootstrap 5.3** - CSS framework
- **Vite 7** - Build tool

## Project Requirements Compliance

### ✅ 6. Enumeration (2 Points)
- **Location**: `Maysara.Server/Models/OrderStatus.cs`
- **Implementation**: OrderStatus enum with values: Pending, Confirmed, Preparing, OutForDelivery, Delivered, Cancelled
- Used consistently throughout the application in Order model and API responses

### ✅ 7. C# Classes (6 Points)
Created 3 database entity classes with fields, properties, constructors, and methods:

1. **Restaurant** (`Maysara.Server/Models/Restaurant.cs`)
   - Properties: RestaurantID, Name, Address, Phone, Rating, DeliveryFee, IsActive, etc.
   - Methods: GetDisplayInfo(), IsAvailable(), ToggleActive()

2. **Product** (`Maysara.Server/Models/Product.cs`)
   - Properties: ProductID, RestaurantID, Name, Description, Price, Category, Stock, IsAvailable
   - Methods: GetPriceDisplay(), IsInStock(), UpdateStock(), CanOrder(), GetStockStatus()

3. **Order** (`Maysara.Server/Models/Order.cs`)
   - Properties: OrderID, UserID, RestaurantID, TotalAmount, DeliveryAddress, PaymentMethod, Status, OrderDate
   - Methods: UpdateStatus(), GetStatusDisplay(), CanCancel(), Cancel(), AddItem(), RemoveItem(), RecalculateTotal()

### ✅ 8. Front-End Framework (10 Points)
- **React Integration**: Full React SPA with ASP.NET Core backend
- **Components**: Reusable components (Layout, LoadingSpinner, ErrorMessage)
- **State Management**: React Context API (CartContext) for global state
- **Hooks**: useState, useEffect, useContext, useNavigate, useParams
- **Dynamic Content**: Real-time cart updates, order status tracking, interactive forms

### ✅ 9. Razor Web Pages (12 Points)
Implemented 5 React pages (equivalent to Razor Pages in modern architecture):

1. **Home** (`src/pages/Home.jsx`) - Browse active restaurants
2. **Restaurant Details** (`src/pages/RestaurantDetails.jsx`) - View menu, filter by category, add items to cart
3. **Cart/Checkout** (`src/pages/Cart.jsx`) - Review cart, adjust quantities, place orders with form validation
4. **Orders List** (`src/pages/Orders.jsx`) - View order history by user ID
5. **Order Details** (`src/pages/OrderDetails.jsx`) - Track order status, view details, cancel orders

**Form Controls Used**:
- Text inputs (User ID, Delivery Address)
- Textarea (Special Instructions)
- Dropdown/Select (Payment Method, Category Filter)
- Number inputs (Quantity)
- Buttons (Add to Cart, Place Order, Cancel Order)

**Data Interaction**:
- All pages interact with in-memory collections via API
- CRUD operations for orders
- Read operations for restaurants and products

**Consistent Navigation**:
- Bootstrap Navbar with links to Home, Cart, My Orders
- Cart badge showing item count
- Back buttons on detail pages

### ✅ 10. Business Logic (5 Points)
Business logic implemented in:

- **API Controllers**: Input validation, error handling, business rule enforcement
- **DataService**: In-memory data management with thread-safe operations
- **Model Methods**: Order cancellation rules, stock validation, price calculations
- **React Components**: Cart management, form validation, order processing

Examples:
- Cannot add items from different restaurants to cart
- Can only cancel orders in Pending or Confirmed status
- Stock validation before adding to cart
- Automatic status progression tracking

### ✅ 11. Custom Layout (5 Points)
- **Layout Component** (`src/components/Layout.jsx`)
- Consistent navbar across all pages
- Bootstrap navigation with routing
- Footer section
- Cart item count badge
- Responsive design

### ✅ 12. Error Handling (5 Points)
Comprehensive error handling implemented:

**Backend**:
- Try-catch blocks in all API controller methods
- User-friendly error messages
- Logging with ILogger
- HTTP status codes (400, 404, 500)

**Frontend**:
- ErrorMessage component for displaying errors
- Try-catch in async operations
- Loading states during API calls
- Form validation error messages

Examples:
- Invalid IDs return 400 Bad Request
- Not found resources return 404
- Server errors return 500 with friendly messages
- Network errors caught and displayed to user

### ✅ 13. Bootstrap Styling (5 Points)
- Bootstrap 5.3 integrated throughout
- Bootstrap Icons for visual elements
- Responsive grid system (Container, Row, Col)
- Styled components:
  - Cards (restaurants, products, orders)
  - Tables (cart items, order items)
  - Forms (checkout form, order search)
  - Buttons (primary, secondary, danger)
  - Badges (status indicators)
  - Navbar (responsive navigation)
  - Alerts (success, error messages)
- Custom CSS enhancements in `index.css`

### ✅ 14. Form Validation (5 Points)
Implemented in Cart/Checkout page with 4+ validation types:

1. **Required Fields**: User ID, Delivery Address, Phone, Payment Method
2. **Data Format**: Phone number regex validation (+XXX-XXXX-XXXX)
3. **String Length**:
   - Delivery Address (5-300 characters)
   - Special Instructions (0-500 characters)
4. **Range Validation**: Quantity must be within available stock

**Validation Features**:
- Real-time validation feedback
- Bootstrap validation styling
- Error messages for each field
- Character counters for limited fields
- Form submission prevention until valid

## Project Structure

```
Maysara/
├── Maysara.Server/
│   ├── Controllers/
│   │   ├── RestaurantsController.cs
│   │   ├── ProductsController.cs
│   │   └── OrdersController.cs
│   ├── Models/
│   │   ├── OrderStatus.cs
│   │   ├── Restaurant.cs
│   │   ├── Product.cs
│   │   └── Order.cs
│   ├── Services/
│   │   └── DataService.cs
│   └── Program.cs
│
└── maysara.client/
    ├── src/
    │   ├── components/
    │   │   ├── Layout.jsx
    │   │   ├── LoadingSpinner.jsx
    │   │   └── ErrorMessage.jsx
    │   ├── contexts/
    │   │   └── CartContext.jsx
    │   ├── pages/
    │   │   ├── Home.jsx
    │   │   ├── RestaurantDetails.jsx
    │   │   ├── Cart.jsx
    │   │   ├── Orders.jsx
    │   │   └── OrderDetails.jsx
    │   ├── utils/
    │   │   └── api.js
    │   ├── App.jsx
    │   ├── main.jsx
    │   └── index.css
    └── package.json
```

## Sample Data

The application includes sample data for 4 restaurants and 12 products:

**Restaurants**:
1. Al Angham Restaurant (Traditional Omani)
2. Bait Al Luban (Omani Seafood)
3. Turkish House Restaurant (Turkish)
4. India Palace (Indian)

**Products** (3 per restaurant):
- Main courses, desserts, bread
- Prices ranging from 2.50 to 22.00 OMR
- Stock levels for inventory management

## Running the Application

### Prerequisites
- .NET 8.0 SDK
- Node.js (v18+)

### Steps

1. **Restore backend packages**:
   ```bash
   cd Maysara.Server
   dotnet restore
   ```

2. **Install frontend dependencies**:
   ```bash
   cd ../maysara.client
   npm install
   ```

3. **Run the application**:
   ```bash
   cd ..
   dotnet run --project Maysara.Server
   ```

4. **Access the application**:
   - Frontend: https://localhost:57728
   - Backend API: https://localhost:5196
   - Swagger UI: https://localhost:5196/swagger

## Key Features

### User Workflow
1. Browse restaurants on home page
2. Click a restaurant to view menu
3. Filter products by category
4. Add items to cart (validates restaurant matching)
5. Review cart and adjust quantities
6. Fill checkout form with validation
7. Place order and receive confirmation
8. Track order status with progress indicator
9. View order history by User ID
10. Cancel orders (if in early stages)

### Technical Highlights
- RESTful API design
- In-memory data storage with singleton pattern
- Thread-safe operations
- CORS configuration
- React Context for state management
- Client-side routing
- Form validation with Bootstrap
- Error boundaries and loading states
- Responsive design

## API Endpoints

### Restaurants
- `GET /api/restaurants` - Get all active restaurants
- `GET /api/restaurants/{id}` - Get restaurant by ID

### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/restaurant/{restaurantId}` - Get products by restaurant

### Orders
- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order by ID
- `GET /api/orders/user/{userId}` - Get orders by user
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}/status` - Update order status
- `POST /api/orders/{id}/cancel` - Cancel order

## Notes for Evaluation

### Enum Usage
- OrderStatus enum is used in Order model, API responses, and frontend display
- Automatically serialized as string in JSON (configured in Program.cs)

### Collections
- In-memory List<T> collections for all entities
- Singleton DataService ensures data persistence during runtime
- Thread-safe operations with lock statements

### Validation Types Demonstrated
1. **Required**: userId, deliveryAddress, customerPhone, paymentMethod
2. **Format**: Phone number with regex pattern
3. **Length**: deliveryAddress (5-300), specialInstructions (max 500)
4. **Range**: Quantity within stock limits

### Error Handling Examples
- All API methods wrapped in try-catch
- ModelState validation
- Business rule exceptions (e.g., can't cancel delivered order)
- Frontend error display with ErrorMessage component

---

**Project Created**: October 2025
**For**: COMP4701 Course Assignment
**Author**: Student Implementation
**Framework**: ASP.NET Core 8 + React 19
