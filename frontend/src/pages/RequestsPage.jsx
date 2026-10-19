import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { apiFetch } from '../api';
import Pagination from '../components/Pagination';
import { categoryLabels, formatDate, priorityLabels, statusLabels } from '../labels';

const PAGE_SIZE = 10;

export default function RequestsPage() {
  const [status, setStatus] = useState('');
  const [page, setPage] = useState(1);
  const [data, setData] = useState(null);

  // Runs on first display and again whenever the filter or the page changes.
  useEffect(() => {
    async function load() {
      const params = new URLSearchParams({ page, pageSize: PAGE_SIZE });
      if (status) {
        params.append('status', status);
      }

      const response = await apiFetch(`/api/requests?${params}`);
      if (response.ok) {
        setData(await response.json());
      }
    }
    load();
  }, [status, page]);

  function handleStatusChange(event) {
    setStatus(event.target.value);
    setPage(1);
  }

  return (
    <div className="page">
      <h1>Minhas solicitações</h1>

      <div className="toolbar">
        <label>
          Status
          <select value={status} onChange={handleStatusChange}>
            <option value="">Todos</option>
            {Object.entries(statusLabels).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </select>
        </label>
        <Link to="/requests/new">Nova solicitação</Link>
      </div>

      {data === null && <p>Carregando...</p>}

      {data !== null && data.items.length === 0 && <p>Nenhuma solicitação encontrada.</p>}

      {data !== null && data.items.length > 0 && (
        <>
          <table>
            <thead>
              <tr>
                <th>Título</th>
                <th>Categoria</th>
                <th>Prioridade</th>
                <th>Status</th>
                <th>Criada em</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((request) => (
                <tr key={request.id}>
                  <td>
                    <Link to={`/requests/${request.id}`}>{request.title}</Link>
                  </td>
                  <td>{categoryLabels[request.category]}</td>
                  <td className={`priority priority-${request.priority}`}>{priorityLabels[request.priority]}</td>
                  <td>
                    <span className={`badge badge-${request.status}`}>{statusLabels[request.status]}</span>
                  </td>
                  <td>{formatDate(request.createdAt)}</td>
                </tr>
              ))}
            </tbody>
          </table>
          <Pagination page={data.page} pageSize={data.pageSize} totalCount={data.totalCount} onPageChange={setPage} />
        </>
      )}
    </div>
  );
}
