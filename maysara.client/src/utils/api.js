// API utility functions for making requests to the backend

const API_BASE_URL = '/api';

// Generic fetch wrapper with error handling
async function fetchAPI(endpoint, options = {}) {
    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers,
            },
            ...options,
        });

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            throw new Error(errorData.error || `HTTP error! status: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error(`API Error (${endpoint}):`, error);
        throw error;
    }
}

// Restaurant API
export const restaurantAPI = {
    getAll: () => fetchAPI('/restaurants'),

    getById: (id) => fetchAPI(`/restaurants/${id}`),

    getActive: () => fetchAPI('/restaurants')
};

// Product API
export const productAPI = {
    getAll: () => fetchAPI('/products'),

    getById: (id) => fetchAPI(`/products/${id}`),

    getByRestaurant: (restaurantId) => fetchAPI(`/products/restaurant/${restaurantId}`),

    getByCategory: (category) => fetchAPI(`/products/category/${category}`)
};

// Order API
export const orderAPI = {
    getAll: () => fetchAPI('/orders'),

    getById: (id) => fetchAPI(`/orders/${id}`),

    getByUser: (userId) => fetchAPI(`/orders/user/${userId}`),

    create: (orderData) => fetchAPI('/orders', {
        method: 'POST',
        body: JSON.stringify(orderData)
    }),

    updateStatus: (orderId, status) => fetchAPI(`/orders/${orderId}/status`, {
        method: 'PUT',
        body: JSON.stringify({ Status: status })
    }),

    cancel: (orderId) => fetchAPI(`/orders/${orderId}/cancel`, {
        method: 'POST'
    })
};

// Order Status enum (matching C# enum)
export const OrderStatus = {
    Pending: 0,
    Confirmed: 1,
    Preparing: 2,
    OutForDelivery: 3,
    Delivered: 4,
    Cancelled: 5
};

// Helper to get status display name
export function getStatusDisplay(status) {
    const statusMap = {
        [OrderStatus.Pending]: 'Pending Confirmation',
        [OrderStatus.Confirmed]: 'Order Confirmed',
        [OrderStatus.Preparing]: 'Being Prepared',
        [OrderStatus.OutForDelivery]: 'Out for Delivery',
        [OrderStatus.Delivered]: 'Delivered',
        [OrderStatus.Cancelled]: 'Cancelled'
    };
    return statusMap[status] || 'Unknown Status';
}
