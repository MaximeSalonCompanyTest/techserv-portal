import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { apiFetch } from '../api';

export default function RegisterPage() {
  const [form, setForm] = useState({ name: '', email: '', phone: '', password: '' });
  const [error, setError] = useState('');
  const navigate = useNavigate();

  // One handler for every field: the input's name says which property to update.
  function handleChange(event) {
    setForm({ ...form, [event.target.name]: event.target.value });
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setError('');

    const response = await apiFetch('/api/auth/register', {
      method: 'POST',
      body: JSON.stringify(form),
    });

    if (response.status === 409) {
      setError('Este e-mail já está cadastrado.');
      return;
    }
    if (!response.ok) {
      setError('Verifique os dados informados.');
      return;
    }

    navigate('/login', { state: { registered: true } });
  }

  return (
    <div className="page narrow">
      <h1>Criar conta</h1>

      <form onSubmit={handleSubmit}>
        <label>
          Nome
          <input name="name" value={form.name} onChange={handleChange} maxLength={100} required />
        </label>
        <label>
          E-mail
          <input name="email" type="email" value={form.email} onChange={handleChange} maxLength={200} required />
        </label>
        <label>
          Telefone
          <input name="phone" type="tel" value={form.phone} onChange={handleChange} maxLength={30} required />
        </label>
        <label>
          Senha (mínimo 6 caracteres)
          <input name="password" type="password" value={form.password} onChange={handleChange} minLength={6} required />
        </label>
        {error && <p className="error">{error}</p>}
        <button type="submit">Cadastrar</button>
      </form>

      <p>
        Já tem conta? <Link to="/login">Entrar</Link>
      </p>
    </div>
  );
}
