import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { Card, Row, Col, Button, Badge, Alert } from 'react-bootstrap';
import { restaurantAPI, productAPI } from '../utils/api';
import { useCart } from '../contexts/CartContext';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorMessage from '../components/ErrorMessage';

function RestaurantDetails() {
    const { id } = useParams();
    const navigate = useNavigate();
    const { addToCart } = useCart();

    const [restaurant, setRestaurant] = useState(null);
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [successMessage, setSuccessMessage] = useState('');
    const [selectedCategory, setSelectedCategory] = useState('All');

    useEffect(() => {
        loadData();
    }, [id]);

    const loadData = async () => {
        try {
            setLoading(true);
            setError(null);

            const [restaurantData, productsData] = await Promise.all([
                restaurantAPI.getById(id),
                productAPI.getByRestaurant(id)
            ]);

            setRestaurant(restaurantData);
            setProducts(productsData);
        } catch (err) {
            setError(err.message || 'Failed to load restaurant details. Please try again later.');
            console.error('Error loading restaurant details:', err);
        } finally {
            setLoading(false);
        }
    };

    const handleAddToCart = (product) => {
        try {
            if (!product.IsAvailable || product.Stock <= 0) {
                setError('This item is currently unavailable.');
                return;
            }

            addToCart(product, restaurant);
            setSuccessMessage(`${product.Name} added to cart!`);
            setTimeout(() => setSuccessMessage(''), 3000);
        } catch (err) {
            setError(err.message || 'Failed to add item to cart.');
        }
    };

    const getCategories = () => {
        const categories = ['All', ...new Set(products.map(p => p.Category))];
        return categories;
    };

    const filteredProducts = selectedCategory === 'All'
        ? products
        : products.filter(p => p.Category === selectedCategory);

    if (loading) {
        return <LoadingSpinner message="Loading restaurant details..." />;
    }

    if (!restaurant) {
        return (
            <div className="text-center py-5">
                <h2>Restaurant not found</h2>
                <Link to="/" className="btn btn-primary mt-3">Back to Home</Link>
            </div>
        );
    }

    return (
        <div>
            <Button variant="outline-secondary" className="mb-4" onClick={() => navigate(-1)}>
                <i className="bi bi-arrow-left me-2"></i>Back
            </Button>

            <Card className="mb-4 shadow-sm">
                <Card.Body>
                    <div className="d-flex justify-content-between align-items-start">
                        <div>
                            <h1>{restaurant.Name}</h1>
                            <p className="text-muted mb-2">{restaurant.Cuisine}</p>
                            <p className="mb-2">
                                <i className="bi bi-geo-alt me-2"></i>
                                {restaurant.Address}
                            </p>
                            <p className="mb-2">
                                <i className="bi bi-telephone me-2"></i>
                                {restaurant.Phone}
                            </p>
                        </div>
                        <div className="text-end">
                            <div className="mb-2">
                                <i className="bi bi-star-fill text-warning me-1"></i>
                                <strong>{restaurant.Rating.toFixed(1)}</strong>
                            </div>
                            <div className="text-muted">
                                Delivery: <strong>{restaurant.DeliveryFee.toFixed(2)} OMR</strong>
                            </div>
                            {restaurant.IsActive && (
                                <Badge bg="success" className="mt-2">Open Now</Badge>
                            )}
                        </div>
                    </div>
                </Card.Body>
            </Card>

            {successMessage && (
                <Alert variant="success" dismissible onClose={() => setSuccessMessage('')}>
                    {successMessage}
                </Alert>
            )}

            <ErrorMessage message={error} onClose={() => setError(null)} />

            {products.length === 0 ? (
                <div className="text-center py-5">
                    <p className="text-muted">No menu items available.</p>
                </div>
            ) : (
                <>
                    <div className="mb-4">
                        <h2 className="mb-3">Menu</h2>
                        <div className="d-flex gap-2 flex-wrap">
                            {getCategories().map(category => (
                                <Button
                                    key={category}
                                    variant={selectedCategory === category ? 'primary' : 'outline-primary'}
                                    size="sm"
                                    onClick={() => setSelectedCategory(category)}
                                >
                                    {category}
                                </Button>
                            ))}
                        </div>
                    </div>

                    <Row xs={1} md={2} lg={3} className="g-4">
                        {filteredProducts.map((product) => (
                            <Col key={product.ProductID}>
                                <Card className="h-100 shadow-sm">
                                    <Card.Body className="d-flex flex-column">
                                        <div className="mb-2">
                                            <Badge bg="secondary">{product.Category}</Badge>
                                            {!product.IsAvailable && (
                                                <Badge bg="danger" className="ms-2">Unavailable</Badge>
                                            )}
                                            {product.Stock > 0 && product.Stock < 5 && product.IsAvailable && (
                                                <Badge bg="warning" text="dark" className="ms-2">Low Stock</Badge>
                                            )}
                                        </div>
                                        <Card.Title>{product.Name}</Card.Title>
                                        <Card.Text className="text-muted">
                                            {product.Description}
                                        </Card.Text>
                                        <div className="mt-auto">
                                            <div className="d-flex justify-content-between align-items-center">
                                                <h5 className="mb-0 text-primary">{product.Price.toFixed(2)} OMR</h5>
                                                <Button
                                                    variant="primary"
                                                    size="sm"
                                                    onClick={() => handleAddToCart(product)}
                                                    disabled={!product.IsAvailable || product.Stock <= 0}
                                                >
                                                    <i className="bi bi-cart-plus me-1"></i>
                                                    Add to Cart
                                                </Button>
                                            </div>
                                            {product.IsAvailable && product.Stock > 0 && (
                                                <small className="text-muted">Stock: {product.Stock}</small>
                                            )}
                                        </div>
                                    </Card.Body>
                                </Card>
                            </Col>
                        ))}
                    </Row>
                </>
            )}
        </div>
    );
}

export default RestaurantDetails;
