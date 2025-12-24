const API_BASE = '/api/customer';

// Helper function to display response
function displayResponse(status, data) {
    const statusDiv = document.getElementById('responseStatus');
    const dataDiv = document.getElementById('responseData');

    if (status >= 200 && status < 300) {
        statusDiv.innerHTML = `<span class="status-success">✓ Success (${status})</span>`;
    } else {
        statusDiv.innerHTML = `<span class="status-error">✗ Error (${status})</span>`;
    }

    dataDiv.innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
}

// Helper function to handle API errors
async function handleResponse(response) {
    const data = await response.json();
    displayResponse(response.status, data);
    return data;
}

// Restaurant Operations
async function searchRestaurants() {
    try {
        const searchTerm = document.getElementById('searchTerm').value;
        const url = searchTerm
            ? `${API_BASE}/restaurants?search=${encodeURIComponent(searchTerm)}`
            : `${API_BASE}/restaurants`;

        const response = await fetch(url, {
            method: 'GET',
            credentials: 'include', // Include session cookie
            headers: {
                'Accept': 'application/json'
            }
        });

        await handleResponse(response);
    } catch (error) {
        displayResponse(0, { error: error.message });
    }
}

async function getRestaurantDetails() {
    try {
        const restaurantId = document.getElementById('restaurantId').value;

        if (!restaurantId) {
            alert('Please enter a restaurant ID');
            return;
        }

        const response = await fetch(`${API_BASE}/restaurants/${restaurantId}`, {
            method: 'GET',
            credentials: 'include',
            headers: {
                'Accept': 'application/json'
            }
        });

        await handleResponse(response);
    } catch (error) {
        displayResponse(0, { error: error.message });
    }
}

// Cart Operations
async function getCart() {
    try {
        const response = await fetch(`${API_BASE}/cart`, {
            method: 'GET',
            credentials: 'include',
            headers: {
                'Accept': 'application/json'
            }
        });

        await handleResponse(response);
    } catch (error) {
        displayResponse(0, { error: error.message });
    }
}

async function addToCart() {
    try {
        const productId = document.getElementById('addProductId').value;
        const quantity = document.getElementById('addQuantity').value;

        if (!productId || !quantity) {
            alert('Please enter product ID and quantity');
            return;
        }

        const response = await fetch(`${API_BASE}/cart/add`, {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({
                ProductId: parseInt(productId),
                Quantity: parseInt(quantity)
            })
        });

        await handleResponse(response);

        // Clear inputs on success
        if (response.ok) {
            document.getElementById('addProductId').value = '';
            document.getElementById('addQuantity').value = '1';
        }
    } catch (error) {
        displayResponse(0, { error: error.message });
    }
}

async function updateCart() {
    try {
        const productId = document.getElementById('updateProductId').value;
        const quantity = document.getElementById('updateQuantity').value;

        if (!productId || !quantity) {
            alert('Please enter product ID and quantity');
            return;
        }

        const response = await fetch(`${API_BASE}/cart/update`, {
            method: 'PUT',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({
                ProductId: parseInt(productId),
                Quantity: parseInt(quantity)
            })
        });

        await handleResponse(response);

        // Clear inputs on success
        if (response.ok) {
            document.getElementById('updateProductId').value = '';
            document.getElementById('updateQuantity').value = '1';
        }
    } catch (error) {
        displayResponse(0, { error: error.message });
    }
}

async function removeFromCart() {
    try {
        const productId = document.getElementById('removeProductId').value;

        if (!productId) {
            alert('Please enter a product ID');
            return;
        }

        const response = await fetch(`${API_BASE}/cart/remove/${productId}`, {
            method: 'DELETE',
            credentials: 'include',
            headers: {
                'Accept': 'application/json'
            }
        });

        await handleResponse(response);

        // Clear input on success
        if (response.ok) {
            document.getElementById('removeProductId').value = '';
        }
    } catch (error) {
        displayResponse(0, { error: error.message });
    }
}

// Order Operations
async function getOrderHistory() {
    try {
        const response = await fetch(`${API_BASE}/orders`, {
            method: 'GET',
            credentials: 'include',
            headers: {
                'Accept': 'application/json'
            }
        });

        await handleResponse(response);
    } catch (error) {
        displayResponse(0, { error: error.message });
    }
}

async function getOrderDetails() {
    try {
        const orderId = document.getElementById('orderId').value;

        if (!orderId) {
            alert('Please enter an order ID');
            return;
        }

        const response = await fetch(`${API_BASE}/orders/${orderId}`, {
            method: 'GET',
            credentials: 'include',
            headers: {
                'Accept': 'application/json'
            }
        });

        await handleResponse(response);
    } catch (error) {
        displayResponse(0, { error: error.message });
    }
}

async function placeOrder() {
    try {
        const deliveryAddress = document.getElementById('deliveryAddress').value;
        const paymentMethod = document.getElementById('paymentMethod').value;

        if (!deliveryAddress) {
            alert('Please enter a delivery address');
            return;
        }

        const response = await fetch(`${API_BASE}/orders`, {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({
                DeliveryAddress: deliveryAddress,
                PaymentMethod: paymentMethod
            })
        });

        await handleResponse(response);

        // Clear inputs on success
        if (response.ok) {
            document.getElementById('deliveryAddress').value = '';
            document.getElementById('paymentMethod').value = 'Cash';
        }
    } catch (error) {
        displayResponse(0, { error: error.message });
    }
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', function() {
    console.log('Maysara API Client initialized');
    console.log('API Base URL:', API_BASE);
    console.log('Note: Make sure you are logged in as a Customer in the main application');
});
