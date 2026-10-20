import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { apiFetch } from '../api';
import Pagination from '../components/Pagination';
import { categoryLabels, formatDate, priorityLabels, statusLabels } from '../labels';

const PAGE_SIZE = 10;

export default function RequestsPage() {
  const [status, setStatus] = useState(''); // the filter; '' means all statuses
  const [page, setPage] = useState(1);
  const [data, setData] = useState(null); // the API's answer: { items, page, pageSize, totalCount }; null while loading

  // Runs on first display and again whenever the filter or the page changes.
  useEffect(() => {
    async function load() {
      // Builds the query string, for example ?page=1&pageSize=10&status=Open
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

  // A new filter starts again from page 1, otherwise we could ask for page 2 of a one-page result.
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

      {/* Three cases: still loading, no results, or the table. */}
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
              {/* One row per request. key helps React track rows; the class names drive the badge and priority dot colors in index.css. */}
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
