import { categoryLabels, formatDate, priorityLabels, statusLabels } from '../labels';

// Shared by the customer and agent detail pages: all fields plus the status history.
export default function RequestDetails({ request }) {
  return (
    <>
      <h1>{request.title}</h1>
      <dl className="details">
        <dt>Status</dt>
        <dd>
          <span className={`badge badge-${request.status}`}>{statusLabels[request.status]}</span>
        </dd>
        <dt>Cliente</dt>
        <dd>{request.customerName}</dd>
        <dt>Categoria</dt>
        <dd>{categoryLabels[request.category]}</dd>
        <dt>Prioridade</dt>
        <dd className={`priority priority-${request.priority}`}>{priorityLabels[request.priority]}</dd>
        <dt>Criada em</dt>
        <dd>{formatDate(request.createdAt)}</dd>
        <dt>Descrição</dt>
        <dd className="description">{request.description}</dd>
      </dl>

      {/* Oldest change first (the API sorts it). A null fromStatus is the creation line. */}
      <h2>Histórico</h2>
      <ul className="history">
        {request.history.map((change, index) => (
          <li key={index}>
            {formatDate(change.changedAt)}:{' '}
            {change.fromStatus === null
              ? `criada como ${statusLabels[change.toStatus]}`
              : `de ${statusLabels[change.fromStatus]} para ${statusLabels[change.toStatus]}`}{' '}
            por {change.changedByName}
          </li>
        ))}
      </ul>
    </>
  );
}
