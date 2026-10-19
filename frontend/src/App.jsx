import { Navigate, Route, Routes } from 'react-router-dom';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import NewRequestPage from './pages/NewRequestPage';
import RegisterPage from './pages/RegisterPage';
import RequestDetailPage from './pages/RequestDetailPage';
import RequestsPage from './pages/RequestsPage';

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
            <p className="page">Meu perfil (próximo passo)</p>
          </ProtectedRoute>
        }
      />
      <Route
        path="/agent"
        element={
          <ProtectedRoute role="Agent">
            <p className="page">Todas as solicitações (próximo passo)</p>
          </ProtectedRoute>
        }
      />
      <Route path="*" element={<Navigate to="/requests" />} />
    </Routes>
  );
}
