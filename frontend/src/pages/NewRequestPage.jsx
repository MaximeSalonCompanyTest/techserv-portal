import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiFetch } from '../api';
import { categoryLabels, priorityLabels } from '../labels';

export default function NewRequestPage() {
  const [form, setForm] = useState({ title: '', description: '', category: '', priority: '' });
  const [error, setError] = useState('');
  const navigate = useNavigate();

  // One handler for every field: the input's name says which property of the form to update.
  function handleChange(event) {
    setForm({ ...form, [event.target.name]: event.target.value });
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setError('');

    const response = await apiFetch('/api/requests', {
      method: 'POST',
      body: JSON.stringify(form),
    });

    if (!response.ok) {
      setError('Verifique os dados informados.');
      return;
    }

    // The API answers 201 with the new request's id, so we open its detail page.
    const data = await response.json();
    navigate(`/requests/${data.id}`);
  }

  return (
    <div className="page narrow">
      <h1>Nova solicitação</h1>

      <form onSubmit={handleSubmit}>
        <label>
          Título
          <input name="title" value={form.title} onChange={handleChange} maxLength={150} required />
        </label>
        <label>
          Descrição
          <textarea name="description" value={form.description} onChange={handleChange} maxLength={2000} rows={5} required />
        </label>
        {/* Dropdown options come from labels.js: the value is the English name the API expects, the text is Portuguese.
            The empty "Selecione..." option plus required forces the user to choose. */}
        <label>
          Categoria
          <select name="category" value={form.category} onChange={handleChange} required>
            <option value="">Selecione...</option>
            {Object.entries(categoryLabels).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </select>
        </label>
        <label>
          Prioridade
          <select name="priority" value={form.priority} onChange={handleChange} required>
            <option value="">Selecione...</option>
            {Object.entries(priorityLabels).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </select>
        </label>
        {error && <p className="error">{error}</p>}
        <button type="submit">Enviar solicitação</button>
      </form>
    </div>
  );
}
