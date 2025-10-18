import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, Row, Col, Badge } from 'react-bootstrap';
import { restaurantAPI } from '../utils/api';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorMessage from '../components/ErrorMessage';

function Home() {
    const [restaurants, setRestaurants] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        loadRestaurants();
    }, []);

    const loadRestaurants = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await restaurantAPI.getActive();
            setRestaurants(data);
        } catch (err) {
            setError(err.message || 'Failed to load restaurants. Please try again later.');
            console.error('Error loading restaurants:', err);
        } finally {
            setLoading(false);
        }
    };

    const handleRestaurantClick = (restaurantId) => {
        navigate(`/restaurant/${restaurantId}`);
    };

    if (loading) {
        return <LoadingSpinner message="Loading restaurants..." />;
    }

    return (
        <div>
            <div className="mb-4">
                <h1 className="display-4">Welcome to Maysara</h1>
                <p className="lead text-muted">Your trusted Omani delivery platform</p>
            </div>

            <ErrorMessage message={error} onClose={() => setError(null)} />

            {restaurants.length === 0 ? (
                <div className="text-center py-5">
                    <p className="text-muted">No restaurants available at the moment.</p>
                </div>
            ) : (
                <>
                    <h2 className="mb-4">Browse Restaurants</h2>
                    <Row xs={1} md={2} lg={3} className="g-4">
                        {restaurants.map((restaurant) => (
                            <Col key={restaurant.RestaurantID}>
                                <Card
                                    className="h-100 restaurant-card shadow-sm"
                                    style={{ cursor: 'pointer' }}
                                    onClick={() => handleRestaurantClick(restaurant.RestaurantID)}
                                >
                                    <Card.Body>
                                        <Card.Title className="d-flex justify-content-between align-items-start">
                                            <span>{restaurant.Name}</span>
                                            {restaurant.IsActive && (
                                                <Badge bg="success" className="ms-2">Open</Badge>
                                            )}
                                        </Card.Title>
                                        <Card.Subtitle className="mb-3 text-muted">
                                            {restaurant.Cuisine}
                                        </Card.Subtitle>
                                        <Card.Text>
                                            <div className="mb-2">
                                                <i className="bi bi-geo-alt me-2"></i>
                                                <small>{restaurant.Address}</small>
                                            </div>
                                            <div className="mb-2">
                                                <i className="bi bi-telephone me-2"></i>
                                                <small>{restaurant.Phone}</small>
                                            </div>
                                            <div className="d-flex justify-content-between align-items-center mt-3">
                                                <div>
                                                    <i className="bi bi-star-fill text-warning me-1"></i>
                                                    <strong>{restaurant.Rating.toFixed(1)}</strong>
                                                </div>
                                                <div className="text-muted">
                                                    Delivery: <strong>{restaurant.DeliveryFee.toFixed(2)} OMR</strong>
                                                </div>
                                            </div>
                                        </Card.Text>
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

export default Home;
