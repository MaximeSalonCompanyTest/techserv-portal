// Used by the customer and agent lists. The API sends totalCount so we know how many pages exist.
export default function Pagination({ page, pageSize, totalCount, onPageChange }) {
  // 12 requests with 10 per page gives 2 pages (Math.ceil rounds up). At least 1, even with no results.
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  // onPageChange is the parent's setPage: clicking changes the page, and the parent's useEffect reloads the list.
  return (
    <div className="pagination">
      <button onClick={() => onPageChange(page - 1)} disabled={page <= 1}>
        Anterior
      </button>
      <span>
        Página {page} de {totalPages}
      </span>
      <button onClick={() => onPageChange(page + 1)} disabled={page >= totalPages}>
        Próxima
      </button>
    </div>
  );
}
