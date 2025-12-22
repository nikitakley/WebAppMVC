export function getStatusColor(statusId) {
  switch (statusId) {
    case 1: return '#539b18ff'; // Создан
    case 2: return '#cd9431ff'; // Готовится
    case 3: return '#06563bff'; // Доставляется
    case 4: return '#28334aff'; // Завершён
    default: return '#000';
  }
}