import { createContext, useContext, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const AuthContext = createContext();

// Hardcoded credentials
const VALID_CREDENTIALS = {
    username: 'maysara',
    password: '12341234'
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};

export const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [username, setUsername] = useState(null);

    // Load authentication state from localStorage on mount
    useEffect(() => {
        const savedAuth = localStorage.getItem('maysaraAuth');
        if (savedAuth) {
            try {
                const authData = JSON.parse(savedAuth);
                if (authData.isAuthenticated && authData.username) {
                    setIsAuthenticated(true);
                    setUsername(authData.username);
                }
            } catch (error) {
                console.error('Error loading auth state:', error);
                localStorage.removeItem('maysaraAuth');
            }
        }
    }, []);

    // Save authentication state to localStorage whenever it changes
    useEffect(() => {
        if (isAuthenticated && username) {
            localStorage.setItem('maysaraAuth', JSON.stringify({
                isAuthenticated,
                username
            }));
        } else {
            localStorage.removeItem('maysaraAuth');
        }
    }, [isAuthenticated, username]);

    const login = (inputUsername, inputPassword) => {
        // Validate credentials
        if (inputUsername === VALID_CREDENTIALS.username &&
            inputPassword === VALID_CREDENTIALS.password) {
            setIsAuthenticated(true);
            setUsername(inputUsername);
            return { success: true };
        } else {
            return {
                success: false,
                error: 'Invalid username or password. Please try again.'
            };
        }
    };

    const logout = () => {
        setIsAuthenticated(false);
        setUsername(null);
        localStorage.removeItem('maysaraAuth');
    };

    const value = {
        isAuthenticated,
        username,
        login,
        logout
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
