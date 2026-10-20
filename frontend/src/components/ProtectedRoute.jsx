import { Navigate } from 'react-router-dom';
import { getRole, isLoggedIn } from '../api';
import NavBar from './NavBar';

// Wraps every page that needs login. With a role, it also keeps customers and agents on their own pages.
// children is the page placed between the tags in App.jsx, for example <RequestsPage />.
export default function ProtectedRoute({ children, role }) {
  // 1. No token: go log in.
  if (!isLoggedIn()) {
    return <Navigate to="/login" />;
  }

  // 2. Logged in with the wrong role (an agent on a customer page): go to your own page.
  // This only keeps the screens tidy; the API refuses the wrong role with a 403 anyway.
  if (role && getRole() !== role) {
    return <Navigate to={getRole() === 'Agent' ? '/agent' : '/requests'} />;
  }

  // 3. Allowed: show the nav bar, then the page.
  return (
    <>
      <NavBar />
      {children}
    </>
  );
}
