import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { apiFetch } from '../api';
import RequestDetails from '../components/RequestDetails';
import { statusLabels } from '../labels';

// Mirrors CanMoveTo on the API so the dropdown only offers valid moves. The API still has the final say.
const nextStatuses = {
  Open: ['InProgress', 'Cancelled'],
  InProgress: ['Completed'],
  Completed: [],
  Cancelled: [],
};

export default function AgentRequestDetailPage() {
  const { id } = useParams();
  const [request, setRequest] = useState(null);
  const [notFound, setNotFound] = useState(false);
  const [newStatus, setNewStatus] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    async function load() {
      const response = await apiFetch(`/api/agent/requests/${id}`);
      if (response.ok) {
        setRequest(await response.json());
      } else {
        setNotFound(true);
      }
    }
    load();
  }, [id]);

  async function handleSubmit(event) {
    event.preventDefault();
    setError('');

    const response = await apiFetch(`/api/agent/requests/${id}/status`, {
      method: 'PUT',
      body: JSON.stringify({ status: newStatus }),
    });

    if (response.ok) {
      setRequest(await response.json());
      setNewStatus('');
    } else {
      setError('Não foi possível alterar o status.');
    }
  }

  if (notFound) {
    return (
      <div className="page">
        <p>Solicitação não encontrada.</p>
        <Link to="/agent">Voltar para a lista</Link>
      </div>
    );
  }

  if (request === null) {
    return <p className="page">Carregando...</p>;
  }

  const options = nextStatuses[request.status];

  return (
    <div className="page">
      <Link to="/agent">Voltar para a lista</Link>
      <RequestDetails request={request} />

      <h2>Alterar status</h2>
      {options.length === 0 ? (
        <p>Esta solicitação está finalizada e não pode mais mudar de status.</p>
      ) : (
        <form className="inline-form" onSubmit={handleSubmit}>
          <select value={newStatus} onChange={(e) => setNewStatus(e.target.value)} required>
            <option value="">Selecione...</option>
            {options.map((value) => (
              <option key={value} value={value}>
                {statusLabels[value]}
              </option>
            ))}
          </select>
          <button type="submit">Salvar status</button>
        </form>
      )}
      {error && <p className="error">{error}</p>}
    </div>
  );
}
