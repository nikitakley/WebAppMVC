import './OrderTable.css';
import { getStatusColor } from '../../utils/getcolor';

function calculateTotalPrice(items) {
  if (!items || items.length === 0) return 0;
  return items.reduce((total, item) => total + (item.unitPrice * item.quantity), 0);
}

export default function OrderTable({ orders, showCustomerName = false }) {
    return (
      <table className='order-table'>
        <thead>
          <tr>
            {showCustomerName && <th>Клиент</th>}
            <th>Заказ №</th>
            <th>Ресторан</th>
            <th>Дата</th>
            <th>Статус</th>
            <th>Блюда</th>
            <th>Итоговая стоимость</th>
          </tr>
        </thead>
        <tbody>
          {orders.map(order => {
            const totalPrice = calculateTotalPrice(order.items);
            return (
            <tr key={order.orderId}>
              {showCustomerName && (
                <td>{order.customerName || '—'}</td>
              )}
              <td>#{order.orderId}</td>
              <td>{order.restaurantName || '—'}</td>
              <td>{new Date(order.createdAt).toLocaleString()}</td>
              <td>
                <span className='order-status' style={{ color: getStatusColor(order.statusId) }}>
                  {order.statusName || '—'}
                </span>
              </td>
              <td>
                {order.items?.length > 0 ? (
                  <ul className='order-dishes-list'>
                    {order.items.map(item => (
                      <li key={item.orderItemId}>
                        {item.dishName} — {item.unitPrice} ₽ × {item.quantity} 
                      </li>
                    ))}
                  </ul>
                ) : (
                  <em>Нет блюд</em>
                )}
              </td>
              <td>
                <strong>{totalPrice.toFixed(2)} ₽</strong>
              </td>
            </tr>
            );
        })}
        </tbody>
      </table>
  );
}