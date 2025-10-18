import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Card, Form, Button, Alert, InputGroup } from 'react-bootstrap';
import { useAuth } from '../contexts/AuthContext';

function Login() {
    const navigate = useNavigate();
    const location = useLocation();
    const { login, isAuthenticated } = useAuth();

    const [formData, setFormData] = useState({
        username: '',
        password: ''
    });

    const [formErrors, setFormErrors] = useState({});
    const [validated, setValidated] = useState(false);
    const [showPassword, setShowPassword] = useState(false);
    const [loginError, setLoginError] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);

    // Redirect if already authenticated
    useEffect(() => {
        if (isAuthenticated) {
            navigate('/');
        }
    }, [isAuthenticated, navigate]);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));

        // Clear error for this field when user starts typing
        if (formErrors[name]) {
            setFormErrors(prev => ({ ...prev, [name]: '' }));
        }
        if (loginError) {
            setLoginError('');
        }
    };

    const validateForm = () => {
        const errors = {};

        // 1. Required field validation
        if (!formData.username.trim()) {
            errors.username = 'Username is required';
        }

        if (!formData.password) {
            errors.password = 'Password is required';
        }

        // 2. Minimum length validation
        if (formData.username.trim() && formData.username.trim().length < 3) {
            errors.username = 'Username must be at least 3 characters';
        }

        if (formData.password && formData.password.length < 8) {
            errors.password = 'Password must be at least 8 characters';
        }

        // 3. Pattern validation - Username must be alphanumeric only
        if (formData.username.trim()) {
            const alphanumericRegex = /^[a-zA-Z0-9]+$/;
            if (!alphanumericRegex.test(formData.username.trim())) {
                errors.username = 'Username must contain only letters and numbers (no spaces or special characters)';
            }
        }

        // 4. Format validation - Password must contain at least one number
        if (formData.password && formData.password.length >= 8) {
            const hasNumber = /\d/.test(formData.password);
            if (!hasNumber) {
                errors.password = 'Password must contain at least one number';
            }
        }

        setFormErrors(errors);
        return Object.keys(errors).length === 0;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setValidated(true);

        // Run validation
        if (!validateForm()) {
            return;
        }

        try {
            setIsSubmitting(true);
            setLoginError('');

            // 5. Custom validation - Check against hardcoded credentials
            const result = login(formData.username.trim(), formData.password);

            if (result.success) {
                // Success - redirect to home
                const from = location.state?.from?.pathname || '/';
                navigate(from, { replace: true });
            } else {
                // Failed authentication
                setLoginError(result.error);
            }
        } catch (error) {
            setLoginError('An unexpected error occurred. Please try again.');
            console.error('Login error:', error);
        } finally {
            setIsSubmitting(false);
        }
    };

    const isFormValid = () => {
        return formData.username.trim().length >= 3 &&
               formData.password.length >= 8 &&
               /^[a-zA-Z0-9]+$/.test(formData.username.trim()) &&
               /\d/.test(formData.password);
    };

    return (
        <div className="d-flex justify-content-center align-items-center" style={{ minHeight: '70vh' }}>
            <Card className="shadow-sm" style={{ width: '100%', maxWidth: '450px' }}>
                <Card.Body className="p-4">
                    <div className="text-center mb-4">
                        <h2 className="mb-2">Welcome to Maysara</h2>
                        <p className="text-muted">Login to your account</p>
                    </div>

                    {location.state?.message && (
                        <Alert variant="info" className="mb-3">
                            {location.state.message}
                        </Alert>
                    )}

                    {loginError && (
                        <Alert variant="danger" dismissible onClose={() => setLoginError('')}>
                            <i className="bi bi-exclamation-triangle me-2"></i>
                            {loginError}
                        </Alert>
                    )}

                    <Form noValidate validated={validated} onSubmit={handleSubmit}>
                        <Form.Group className="mb-3">
                            <Form.Label>
                                Username <span className="text-danger">*</span>
                            </Form.Label>
                            <Form.Control
                                type="text"
                                name="username"
                                value={formData.username}
                                onChange={handleInputChange}
                                isInvalid={!!formErrors.username}
                                placeholder="Enter your username"
                                autoComplete="username"
                                autoFocus
                            />
                            <Form.Control.Feedback type="invalid">
                                {formErrors.username}
                            </Form.Control.Feedback>
                            <Form.Text className="text-muted">
                                Min 3 characters, letters and numbers only
                            </Form.Text>
                        </Form.Group>

                        <Form.Group className="mb-3">
                            <Form.Label>
                                Password <span className="text-danger">*</span>
                            </Form.Label>
                            <InputGroup>
                                <Form.Control
                                    type={showPassword ? 'text' : 'password'}
                                    name="password"
                                    value={formData.password}
                                    onChange={handleInputChange}
                                    isInvalid={!!formErrors.password}
                                    placeholder="Enter your password"
                                    autoComplete="current-password"
                                />
                                <Button
                                    variant="outline-secondary"
                                    onClick={() => setShowPassword(!showPassword)}
                                    tabIndex={-1}
                                >
                                    <i className={`bi bi-eye${showPassword ? '-slash' : ''}`}></i>
                                </Button>
                                <Form.Control.Feedback type="invalid">
                                    {formErrors.password}
                                </Form.Control.Feedback>
                            </InputGroup>
                            <Form.Text className="text-muted">
                                Min 8 characters, must contain at least one number
                            </Form.Text>
                        </Form.Group>

                        <div className="mb-3">
                            <small className="text-muted">
                                <i className="bi bi-info-circle me-1"></i>
                                Demo credentials: username = maysara, password = 12341234
                            </small>
                        </div>

                        <Button
                            variant="primary"
                            type="submit"
                            className="w-100"
                            disabled={!isFormValid() || isSubmitting}
                        >
                            {isSubmitting ? (
                                <>
                                    <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                                    Logging in...
                                </>
                            ) : (
                                <>
                                    <i className="bi bi-box-arrow-in-right me-2"></i>
                                    Login
                                </>
                            )}
                        </Button>
                    </Form>

                    <div className="text-center mt-3">
                        <small className="text-muted">
                            New to Maysara? <a href="/" className="text-decoration-none">Browse as guest</a>
                        </small>
                    </div>
                </Card.Body>
            </Card>
        </div>
    );
}

export default Login;
