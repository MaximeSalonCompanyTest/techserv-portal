import { Navigate } from 'react-router-dom';
import { getRole, isLoggedIn } from '../api';
import NavBar from './NavBar';

// Wraps every page that needs login. With a role, it also keeps customers and agents on their own pages.
export default function ProtectedRoute({ children, role }) {
  if (!isLoggedIn()) {
    return <Navigate to="/login" />;
  }

  if (role && getRole() !== role) {
    return <Navigate to={getRole() === 'Agent' ? '/agent' : '/requests'} />;
  }

  return (
    <>
      <NavBar />
      {children}
    </>
  );
}
