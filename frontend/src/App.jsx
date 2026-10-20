import { Navigate, Route, Routes } from 'react-router-dom';
import ProtectedRoute from './components/ProtectedRoute';
import AgentRequestDetailPage from './pages/AgentRequestDetailPage';
import AgentRequestsPage from './pages/AgentRequestsPage';
import LoginPage from './pages/LoginPage';
import NewRequestPage from './pages/NewRequestPage';
import ProfilePage from './pages/ProfilePage';
import RegisterPage from './pages/RegisterPage';
import RequestDetailPage from './pages/RequestDetailPage';
import RequestsPage from './pages/RequestsPage';

// The route list: which page shows for which URL.
// ProtectedRoute requires a token, and role="Customer" or role="Agent" keeps each type of user on its own pages.
export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route
        path="/requests"
        element={
          <ProtectedRoute role="Customer">
            <RequestsPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/requests/new"
        element={
          <ProtectedRoute role="Customer">
            <NewRequestPage />
          </ProtectedRoute>
        }
      />
      {/* :id is a route parameter: the page reads it with useParams(). */}
      <Route
        path="/requests/:id"
        element={
          <ProtectedRoute role="Customer">
            <RequestDetailPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/profile"
        element={
          <ProtectedRoute>
            <ProfilePage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/agent"
        element={
          <ProtectedRoute role="Agent">
            <AgentRequestsPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/agent/requests/:id"
        element={
          <ProtectedRoute role="Agent">
            <AgentRequestDetailPage />
          </ProtectedRoute>
        }
      />
      {/* Any unknown URL goes to /requests; ProtectedRoute then sends logged-out users to /login. */}
      <Route path="*" element={<Navigate to="/requests" />} />
    </Routes>
  );
}
