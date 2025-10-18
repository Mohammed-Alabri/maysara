import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, Row, Col, Button, Form, Table, Alert } from 'react-bootstrap';
import { useCart } from '../contexts/CartContext';
import { restaurantAPI, orderAPI } from '../utils/api';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorMessage from '../components/ErrorMessage';

function Cart() {
    const navigate = useNavigate();
    const { cartItems, restaurantId, updateQuantity, removeFromCart, getTotal, clearCart } = useCart();

    const [restaurant, setRestaurant] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [showCheckout, setShowCheckout] = useState(false);

    // Form state
    const [formData, setFormData] = useState({
        userId: '',
        deliveryAddress: '',
        customerPhone: '',
        paymentMethod: '',
        specialInstructions: ''
    });

    // Validation state
    const [formErrors, setFormErrors] = useState({});
    const [validated, setValidated] = useState(false);

    useEffect(() => {
        if (restaurantId) {
            loadRestaurant();
        }
    }, [restaurantId]);

    const loadRestaurant = async () => {
        try {
            const data = await restaurantAPI.getById(restaurantId);
            setRestaurant(data);
        } catch (err) {
            console.error('Error loading restaurant:', err);
        }
    };

    const handleQuantityChange = (productId, newQuantity) => {
        const quantity = parseInt(newQuantity);
        if (isNaN(quantity) || quantity < 0) return;
        updateQuantity(productId, quantity);
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));

        // Clear error for this field
        if (formErrors[name]) {
            setFormErrors(prev => ({ ...prev, [name]: '' }));
        }
    };

    const validateForm = () => {
        const errors = {};

        // Required field validation
        if (!formData.userId.trim()) {
            errors.userId = 'User ID is required';
        }

        // Delivery address validation (string length)
        if (!formData.deliveryAddress.trim()) {
            errors.deliveryAddress = 'Delivery address is required';
        } else if (formData.deliveryAddress.trim().length < 5) {
            errors.deliveryAddress = 'Delivery address must be at least 5 characters';
        } else if (formData.deliveryAddress.trim().length > 300) {
            errors.deliveryAddress = 'Delivery address cannot exceed 300 characters';
        }

        // Phone validation (data format)
        if (!formData.customerPhone.trim()) {
            errors.customerPhone = 'Phone number is required';
        } else {
            const phoneRegex = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/;
            if (!phoneRegex.test(formData.customerPhone.trim())) {
                errors.customerPhone = 'Invalid phone number format (e.g., +968-1234-5678)';
            }
        }

        // Payment method validation (required)
        if (!formData.paymentMethod) {
            errors.paymentMethod = 'Please select a payment method';
        }

        // Special instructions validation (range - optional but limited)
        if (formData.specialInstructions && formData.specialInstructions.length > 500) {
            errors.specialInstructions = 'Special instructions cannot exceed 500 characters';
        }

        setFormErrors(errors);
        return Object.keys(errors).length === 0;
    };

    const handleCheckout = () => {
        if (cartItems.length === 0) {
            setError('Your cart is empty');
            return;
        }
        setShowCheckout(true);
    };

    const handlePlaceOrder = async (e) => {
        e.preventDefault();
        setValidated(true);

        if (!validateForm()) {
            setError('Please fix the validation errors before submitting');
            return;
        }

        try {
            setLoading(true);
            setError(null);

            const orderData = {
                UserID: formData.userId.trim(),
                RestaurantID: restaurantId,
                TotalAmount: getTotal() + (restaurant?.DeliveryFee || 0),
                DeliveryAddress: formData.deliveryAddress.trim(),
                CustomerPhone: formData.customerPhone.trim(),
                PaymentMethod: formData.paymentMethod,
                SpecialInstructions: formData.specialInstructions.trim() || null,
                Items: cartItems.map(item => ({
                    ProductID: item.ProductID,
                    ProductName: item.Name,
                    UnitPrice: item.Price,
                    Quantity: item.quantity
                }))
            };

            const createdOrder = await orderAPI.create(orderData);

            // Clear cart and redirect to order tracking
            clearCart();
            navigate(`/orders/${createdOrder.OrderID}`);
        } catch (err) {
            setError(err.message || 'Failed to place order. Please try again.');
            console.error('Error placing order:', err);
        } finally {
            setLoading(false);
        }
    };

    if (cartItems.length === 0) {
        return (
            <div className="text-center py-5">
                <i className="bi bi-cart-x" style={{ fontSize: '4rem', color: '#ccc' }}></i>
                <h2 className="mt-3">Your cart is empty</h2>
                <p className="text-muted">Browse restaurants and add items to get started</p>
                <Button variant="primary" onClick={() => navigate('/')}>
                    Browse Restaurants
                </Button>
            </div>
        );
    }

    const subtotal = getTotal();
    const deliveryFee = restaurant?.DeliveryFee || 0;
    const total = subtotal + deliveryFee;

    return (
        <div>
            <h1 className="mb-4">Shopping Cart</h1>

            <ErrorMessage message={error} onClose={() => setError(null)} />

            <Row>
                <Col lg={8}>
                    {restaurant && (
                        <Card className="mb-3">
                            <Card.Body>
                                <h5>Ordering from: <strong>{restaurant.Name}</strong></h5>
                                <p className="text-muted mb-0">{restaurant.Address}</p>
                            </Card.Body>
                        </Card>
                    )}

                    <Card className="mb-4">
                        <Card.Body>
                            <Table responsive>
                                <thead>
                                    <tr>
                                        <th>Item</th>
                                        <th>Price</th>
                                        <th>Quantity</th>
                                        <th>Subtotal</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {cartItems.map((item) => (
                                        <tr key={item.ProductID}>
                                            <td>
                                                <strong>{item.Name}</strong>
                                                <br />
                                                <small className="text-muted">{item.Description}</small>
                                            </td>
                                            <td>{item.Price.toFixed(2)} OMR</td>
                                            <td>
                                                <Form.Control
                                                    type="number"
                                                    min="1"
                                                    max={item.Stock}
                                                    value={item.quantity}
                                                    onChange={(e) => handleQuantityChange(item.ProductID, e.target.value)}
                                                    style={{ width: '80px' }}
                                                />
                                            </td>
                                            <td>
                                                <strong>{(item.Price * item.quantity).toFixed(2)} OMR</strong>
                                            </td>
                                            <td>
                                                <Button
                                                    variant="outline-danger"
                                                    size="sm"
                                                    onClick={() => removeFromCart(item.ProductID)}
                                                >
                                                    <i className="bi bi-trash"></i>
                                                </Button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </Table>
                        </Card.Body>
                    </Card>
                </Col>

                <Col lg={4}>
                    <Card className="sticky-top" style={{ top: '20px' }}>
                        <Card.Body>
                            <h5 className="mb-3">Order Summary</h5>
                            <div className="d-flex justify-content-between mb-2">
                                <span>Subtotal:</span>
                                <strong>{subtotal.toFixed(2)} OMR</strong>
                            </div>
                            <div className="d-flex justify-content-between mb-2">
                                <span>Delivery Fee:</span>
                                <strong>{deliveryFee.toFixed(2)} OMR</strong>
                            </div>
                            <hr />
                            <div className="d-flex justify-content-between mb-3">
                                <strong>Total:</strong>
                                <strong className="text-primary">{total.toFixed(2)} OMR</strong>
                            </div>

                            {!showCheckout ? (
                                <Button variant="primary" className="w-100" onClick={handleCheckout}>
                                    Proceed to Checkout
                                </Button>
                            ) : (
                                <Form noValidate validated={validated} onSubmit={handlePlaceOrder}>
                                    <h6 className="mb-3">Checkout Details</h6>

                                    <Form.Group className="mb-3">
                                        <Form.Label>User ID <span className="text-danger">*</span></Form.Label>
                                        <Form.Control
                                            type="text"
                                            name="userId"
                                            value={formData.userId}
                                            onChange={handleInputChange}
                                            isInvalid={!!formErrors.userId}
                                            required
                                        />
                                        <Form.Control.Feedback type="invalid">
                                            {formErrors.userId}
                                        </Form.Control.Feedback>
                                    </Form.Group>

                                    <Form.Group className="mb-3">
                                        <Form.Label>Delivery Address <span className="text-danger">*</span></Form.Label>
                                        <Form.Control
                                            as="textarea"
                                            rows={3}
                                            name="deliveryAddress"
                                            value={formData.deliveryAddress}
                                            onChange={handleInputChange}
                                            isInvalid={!!formErrors.deliveryAddress}
                                            minLength={5}
                                            maxLength={300}
                                            required
                                        />
                                        <Form.Control.Feedback type="invalid">
                                            {formErrors.deliveryAddress}
                                        </Form.Control.Feedback>
                                        <Form.Text className="text-muted">
                                            {formData.deliveryAddress.length}/300 characters
                                        </Form.Text>
                                    </Form.Group>

                                    <Form.Group className="mb-3">
                                        <Form.Label>Phone Number <span className="text-danger">*</span></Form.Label>
                                        <Form.Control
                                            type="tel"
                                            name="customerPhone"
                                            value={formData.customerPhone}
                                            onChange={handleInputChange}
                                            isInvalid={!!formErrors.customerPhone}
                                            placeholder="+968-1234-5678"
                                            required
                                        />
                                        <Form.Control.Feedback type="invalid">
                                            {formErrors.customerPhone}
                                        </Form.Control.Feedback>
                                    </Form.Group>

                                    <Form.Group className="mb-3">
                                        <Form.Label>Payment Method <span className="text-danger">*</span></Form.Label>
                                        <Form.Select
                                            name="paymentMethod"
                                            value={formData.paymentMethod}
                                            onChange={handleInputChange}
                                            isInvalid={!!formErrors.paymentMethod}
                                            required
                                        >
                                            <option value="">Select payment method</option>
                                            <option value="Cash">Cash on Delivery</option>
                                            <option value="Card">Credit/Debit Card</option>
                                            <option value="E-Wallet">E-Wallet</option>
                                        </Form.Select>
                                        <Form.Control.Feedback type="invalid">
                                            {formErrors.paymentMethod}
                                        </Form.Control.Feedback>
                                    </Form.Group>

                                    <Form.Group className="mb-3">
                                        <Form.Label>Special Instructions (Optional)</Form.Label>
                                        <Form.Control
                                            as="textarea"
                                            rows={2}
                                            name="specialInstructions"
                                            value={formData.specialInstructions}
                                            onChange={handleInputChange}
                                            isInvalid={!!formErrors.specialInstructions}
                                            maxLength={500}
                                        />
                                        <Form.Control.Feedback type="invalid">
                                            {formErrors.specialInstructions}
                                        </Form.Control.Feedback>
                                        <Form.Text className="text-muted">
                                            {formData.specialInstructions.length}/500 characters
                                        </Form.Text>
                                    </Form.Group>

                                    <Button
                                        variant="success"
                                        type="submit"
                                        className="w-100"
                                        disabled={loading}
                                    >
                                        {loading ? 'Placing Order...' : 'Place Order'}
                                    </Button>
                                </Form>
                            )}
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </div>
    );
}

export default Cart;
