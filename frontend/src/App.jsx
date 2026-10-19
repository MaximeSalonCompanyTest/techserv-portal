import { Navigate, Route, Routes } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/requests" element={<p className="page">Minhas solicitações (próximo passo)</p>} />
      <Route path="/agent" element={<p className="page">Todas as solicitações (próximo passo)</p>} />
      <Route path="*" element={<Navigate to="/login" />} />
    </Routes>
  );
}
