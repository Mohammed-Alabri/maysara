import { createContext, useContext, useState, useEffect } from 'react';

const CartContext = createContext();

export const useCart = () => {
    const context = useContext(CartContext);
    if (!context) {
        throw new Error('useCart must be used within a CartProvider');
    }
    return context;
};

export const CartProvider = ({ children }) => {
    const [cartItems, setCartItems] = useState([]);
    const [restaurantId, setRestaurantId] = useState(null);

    // Load cart from localStorage on mount
    useEffect(() => {
        const savedCart = localStorage.getItem('maysaraCart');
        if (savedCart) {
            try {
                const parsed = JSON.parse(savedCart);
                setCartItems(parsed.items || []);
                setRestaurantId(parsed.restaurantId || null);
            } catch (error) {
                console.error('Error loading cart from localStorage:', error);
            }
        }
    }, []);

    // Save cart to localStorage whenever it changes
    useEffect(() => {
        localStorage.setItem('maysaraCart', JSON.stringify({
            items: cartItems,
            restaurantId: restaurantId
        }));
    }, [cartItems, restaurantId]);

    const addToCart = (product, restaurant) => {
        // Check if adding from a different restaurant
        if (restaurantId && restaurantId !== restaurant.RestaurantID) {
            const confirmChange = window.confirm(
                `Your cart contains items from another restaurant. Do you want to clear the cart and add items from ${restaurant.Name}?`
            );
            if (confirmChange) {
                setCartItems([{ ...product, quantity: 1 }]);
                setRestaurantId(restaurant.RestaurantID);
            }
            return;
        }

        // Set restaurant if first item
        if (!restaurantId) {
            setRestaurantId(restaurant.RestaurantID);
        }

        // Check if item already in cart
        const existingItem = cartItems.find(item => item.ProductID === product.ProductID);

        if (existingItem) {
            setCartItems(cartItems.map(item =>
                item.ProductID === product.ProductID
                    ? { ...item, quantity: item.quantity + 1 }
                    : item
            ));
        } else {
            setCartItems([...cartItems, { ...product, quantity: 1 }]);
        }
    };

    const removeFromCart = (productId) => {
        const newItems = cartItems.filter(item => item.ProductID !== productId);
        setCartItems(newItems);

        // Clear restaurant if cart is empty
        if (newItems.length === 0) {
            setRestaurantId(null);
        }
    };

    const updateQuantity = (productId, quantity) => {
        if (quantity <= 0) {
            removeFromCart(productId);
            return;
        }

        setCartItems(cartItems.map(item =>
            item.ProductID === productId
                ? { ...item, quantity: Math.min(quantity, item.Stock) }
                : item
        ));
    };

    const clearCart = () => {
        setCartItems([]);
        setRestaurantId(null);
    };

    const getTotal = () => {
        return cartItems.reduce((total, item) => total + (item.Price * item.quantity), 0);
    };

    const getItemCount = () => {
        return cartItems.reduce((count, item) => count + item.quantity, 0);
    };

    const value = {
        cartItems,
        restaurantId,
        addToCart,
        removeFromCart,
        updateQuantity,
        clearCart,
        getTotal,
        getItemCount
    };

    return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
};
