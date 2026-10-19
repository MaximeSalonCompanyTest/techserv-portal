import { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { apiFetch, saveLogin } from '../api';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const location = useLocation();

  async function handleSubmit(event) {
    event.preventDefault();
    setError('');

    const response = await apiFetch('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
      setError('E-mail ou senha inválidos.');
      return;
    }

    const data = await response.json();
    saveLogin(data);
    navigate(data.role === 'Agent' ? '/agent' : '/requests');
  }

  return (
    <div className="page narrow">
      <h1>Entrar</h1>
      {location.state?.registered && <p className="success">Conta criada. Faça login para continuar.</p>}

      <form onSubmit={handleSubmit}>
        <label>
          E-mail
          <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
        </label>
        <label>
          Senha
          <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
        </label>
        {error && <p className="error">{error}</p>}
        <button type="submit">Entrar</button>
      </form>

      <p>
        Não tem conta? <Link to="/register">Cadastre-se</Link>
      </p>
    </div>
  );
}
