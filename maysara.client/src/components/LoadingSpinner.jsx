import { Spinner } from 'react-bootstrap';

function LoadingSpinner({ message = 'Loading...' }) {
    return (
        <div className="text-center py-5">
            <Spinner animation="border" role="status" variant="primary" className="mb-3">
                <span className="visually-hidden">Loading...</span>
            </Spinner>
            <p className="text-muted">{message}</p>
        </div>
    );
}

export default LoadingSpinner;
