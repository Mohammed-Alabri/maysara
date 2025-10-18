import { Link, Outlet, useNavigate } from 'react-router-dom';
import { Container, Navbar, Nav, Button } from 'react-bootstrap';
import { useCart } from '../contexts/CartContext';
import { useAuth } from '../contexts/AuthContext';

function Layout() {
    const { getItemCount } = useCart();
    const { isAuthenticated, logout } = useAuth();
    const navigate = useNavigate();
    const itemCount = getItemCount();

    const handleLogout = () => {
        logout();
        navigate('/');
    };

    return (
        <>
            <Navbar bg="primary" variant="dark" expand="lg" className="mb-4">
                <Container>
                    <Navbar.Brand as={Link} to="/">
                        <strong>Maysara</strong> Delivery Hub
                    </Navbar.Brand>
                    <Navbar.Toggle aria-controls="basic-navbar-nav" />
                    <Navbar.Collapse id="basic-navbar-nav">
                        <Nav className="ms-auto">
                            <Nav.Link as={Link} to="/">
                                <i className="bi bi-house-door me-1"></i>
                                Home
                            </Nav.Link>
                            <Nav.Link as={Link} to="/cart">
                                <i className="bi bi-cart me-1"></i>
                                Cart {itemCount > 0 && <span className="badge bg-light text-primary ms-1">{itemCount}</span>}
                            </Nav.Link>
                            {isAuthenticated ? (
                                <Button
                                    variant="outline-light"
                                    size="sm"
                                    onClick={handleLogout}
                                    className="ms-2"
                                >
                                    <i className="bi bi-box-arrow-right me-1"></i>
                                    Logout
                                </Button>
                            ) : (
                                <Nav.Link as={Link} to="/login">
                                    <i className="bi bi-box-arrow-in-right me-1"></i>
                                    Login
                                </Nav.Link>
                            )}
                        </Nav>
                    </Navbar.Collapse>
                </Container>
            </Navbar>

            <Container>
                <Outlet />
            </Container>

            <footer className="bg-light text-center text-muted py-3 mt-5">
                <Container>
                    <p className="mb-0">&copy; 2025 Maysara - Omani Delivery Hub. All rights reserved.</p>
                </Container>
            </footer>
        </>
    );
}

export default Layout;
