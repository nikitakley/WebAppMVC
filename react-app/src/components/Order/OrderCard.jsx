import './OrderCard.css'
import { getStatusColor } from '../../utils/getcolor';

export default function OrderCard({ order, showCustomerName = false}) {
    const total = order.items?.reduce(
        (sum, item) => sum + item.unitPrice * item.quantity,
        0
    ) || 0;

    return (
        <div className="order-card">
            <div className="order-header">
                <span className="order-id">Заказ #{order.orderId}</span>
                <span 
                className="order-status"
                style={{ color: getStatusColor(order.statusId) }}
                >
                {order.statusName || 'Без статуса'}
                </span>
            </div>

            <div className="order-details">
                {showCustomerName && order.customerName && (
                <p>Клиент: {order.customerName}</p>
                )}

                <p>Ресторан: {order.restaurantName || '—'}</p>
                <p>Дата: {new Date(order.createdAt).toLocaleString()}</p>
                
                <div className="order-items">
                <h4>Блюда:</h4>
                <ul>
                    {order.items?.map(item => (
                    <li key={item.orderItemId}>
                        {item.dishName || '—'} — {item.unitPrice} ₽ × {item.quantity}
                    </li>
                    ))}
                </ul>
                </div>

                <div className="order-total">
                    Итого: <strong>{total} ₽</strong>
                </div>
            </div>
        </div>
    );
}