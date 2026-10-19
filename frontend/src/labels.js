// The API sends English enum names; the screens show them in Portuguese.
export const statusLabels = {
  Open: 'Aberta',
  InProgress: 'Em andamento',
  Completed: 'Concluída',
  Cancelled: 'Cancelada',
};

export const categoryLabels = {
  Installation: 'Instalação',
  Maintenance: 'Manutenção',
  Support: 'Suporte',
};

export const priorityLabels = {
  Low: 'Baixa',
  Medium: 'Média',
  High: 'Alta',
};

export function formatDate(value) {
  return new Date(value).toLocaleString('pt-BR');
}
