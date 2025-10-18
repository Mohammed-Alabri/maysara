import { Alert } from 'react-bootstrap';

function ErrorMessage({ message, onClose }) {
    if (!message) return null;

    return (
        <Alert variant="danger" dismissible onClose={onClose} className="mb-3">
            <Alert.Heading>Error</Alert.Heading>
            <p className="mb-0">{message}</p>
        </Alert>
    );
}

export default ErrorMessage;
