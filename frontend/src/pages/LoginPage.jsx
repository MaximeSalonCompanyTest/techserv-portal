import { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { apiFetch, saveLogin } from '../api';

export default function LoginPage() {
  // useState is the component's memory: each value lives here, and changing it redraws the screen.
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const location = useLocation();

  async function handleSubmit(event) {
    // Stops the browser's own form submit (a full page reload); we send the data with fetch instead.
    event.preventDefault();
    setError('');

    const response = await apiFetch('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });

    // One message for every failure, like the API's single 401 message.
    if (!response.ok) {
      setError('E-mail ou senha inválidos.');
      return;
    }

    // Save the token, name and role in localStorage, then send customers and agents to their own pages.
    const data = await response.json();
    saveLogin(data);
    navigate(data.role === 'Agent' ? '/agent' : '/requests');
  }

  return (
    <div className="page narrow">
      <h1>Entrar</h1>
      {/* The sign-up page sends a "registered" flag through navigate, so we can confirm the account was created. */}
      {location.state?.registered && <p className="success">Conta criada. Faça login para continuar.</p>}

      {/* Controlled inputs: the value comes from state, and onChange writes every keystroke back to state. */}
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
