import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { apiFetch } from '../api';
import RequestDetails from '../components/RequestDetails';

export default function RequestDetailPage() {
  const { id } = useParams();
  const [request, setRequest] = useState(null);
  const [notFound, setNotFound] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    async function load() {
      const response = await apiFetch(`/api/requests/${id}`);
      if (response.ok) {
        setRequest(await response.json());
      } else {
        setNotFound(true);
      }
    }
    load();
  }, [id]);

  async function handleCancel() {
    if (!window.confirm('Deseja cancelar esta solicitação?')) {
      return;
    }

    setError('');
    const response = await apiFetch(`/api/requests/${id}/cancel`, { method: 'POST' });
    if (response.ok) {
      // The API answers with the updated request, so the page refreshes without a second call.
      setRequest(await response.json());
    } else {
      setError('Não foi possível cancelar. Só solicitações abertas podem ser canceladas.');
    }
  }

  if (notFound) {
    return (
      <div className="page">
        <p>Solicitação não encontrada.</p>
        <Link to="/requests">Voltar para a lista</Link>
      </div>
    );
  }

  if (request === null) {
    return <p className="page">Carregando...</p>;
  }

  return (
    <div className="page">
      <Link to="/requests">Voltar para a lista</Link>
      <RequestDetails request={request} />

      {/* The API also checks this rule; hiding the button only helps honest users. */}
      {request.status === 'Open' && (
        <button className="danger" onClick={handleCancel}>
          Cancelar solicitação
        </button>
      )}
      {error && <p className="error">{error}</p>}
    </div>
  );
}
