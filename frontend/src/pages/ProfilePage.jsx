import { useEffect, useState } from 'react';
import { apiFetch, saveName } from '../api';
import { formatDate } from '../labels';

export default function ProfilePage() {
  const [profile, setProfile] = useState(null); // everything the API sent
  const [form, setForm] = useState({ name: '', phone: '' }); // only the fields the user can edit
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  // The empty list [] means: run once, when the page opens.
  useEffect(() => {
    async function load() {
      const response = await apiFetch('/api/profile');
      if (response.ok) {
        const data = await response.json();
        setProfile(data);
        setForm({ name: data.name, phone: data.phone });
      }
    }
    load();
  }, []);

  function handleChange(event) {
    setForm({ ...form, [event.target.name]: event.target.value });
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setMessage('');
    setError('');

    // Only name and phone are sent: the API has no way to change the email.
    const response = await apiFetch('/api/profile', {
      method: 'PUT',
      body: JSON.stringify(form),
    });

    if (!response.ok) {
      setError('Verifique os dados informados.');
      return;
    }

    const data = await response.json();
    setProfile(data);
    saveName(data.name);
    setMessage('Dados atualizados.');
  }

  if (profile === null) {
    return <p className="page">Carregando...</p>;
  }

  return (
    <div className="page narrow">
      <h1>Meu perfil</h1>

      <form onSubmit={handleSubmit}>
        {/* disabled only locks the field on screen. The real protection is the API: its update DTO has no email. */}
        <label>
          E-mail (não pode ser alterado)
          <input value={profile.email} disabled />
        </label>
        <label>
          Nome
          <input name="name" value={form.name} onChange={handleChange} maxLength={100} required />
        </label>
        <label>
          Telefone
          <input name="phone" type="tel" value={form.phone} onChange={handleChange} maxLength={30} required />
        </label>
        {message && <p className="success">{message}</p>}
        {error && <p className="error">{error}</p>}
        <button type="submit">Salvar</button>
      </form>

      <p>Conta criada em {formatDate(profile.createdAt)}</p>
    </div>
  );
}
