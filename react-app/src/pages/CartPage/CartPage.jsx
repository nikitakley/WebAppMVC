import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { useCart } from '../../context/CartContext';
import { orderService } from '../../services/orderService';
import './CartPage.css'

export default function CheckoutPage() {
    const { currentUser } = useAuth();
    const { cart, setQuantity, removeFromCart, isEmpty } = useCart();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    // Группируем блюда по ресторану
    const groupedCart = cart.reduce((acc, item) => {
        if (!acc[item.restaurantId]) {
            acc[item.restaurantId] = {
                restaurantName: item.restaurantName || 'Ресторан',
                items: []
            };
        }
        acc[item.restaurantId].items.push(item);
        return acc;
    }, {});

    const restaurantTotals = {};
        Object.entries(groupedCart).forEach(([restaurantId, group]) => {
        const total = group.items.reduce((sum, item) => {
            return sum + (item.unitPrice * item.quantity);
        }, 0);
        restaurantTotals[restaurantId] = total;
    });

    const restaurantIds = Object.keys(groupedCart);
    const hasMultipleRestaurants = restaurantIds.length > 1;

    const handleOrder = async (restaurantId) => {
        if (!currentUser) return;

        const itms = groupedCart[restaurantId].items;

        setLoading(true);
        setError(null);

        try {
            // 1. Создаём заказ
            const orderData = {
                customerId: currentUser.customerId,
                courierId: 1, // назначить позже
                restaurantId: Number(restaurantId),
                statusId: 1 // статус «Создан»
            };

            const order = await orderService.createOrder(orderData);

            // 2. Добавляем каждую позицию
            for (const item of itms) {
                await orderService.addOrderItem({
                    orderId: order.orderId,
                    dishId: item.dishId,
                    quantity: item.quantity,
                    unitPrice: item.unitPrice
                });
            }

            // 3. Очищаем корзину и переходим далее
            itms.forEach(item => {
                removeFromCart(item.dishId, item.restaurantId);
            });
            alert('Заказ успешно создан!');
            navigate('/orders'); // или на страницу заказов
            } catch (err) {
                console.error('Order failed:', err);
                setError('Не удалось создать заказ. Попробуйте позже.');
            } finally {
                setLoading(false);
        }
    };

    if (isEmpty()) {
        return (
        <div className="p-6 text-center">
            <h2 className="text-2xl font-bold mb-4">Ваша корзина пуста</h2>
            <p>Добавьте блюда из меню ресторанов.</p>
        </div>
        );
    }

    return (
        <div className='cart-page'>
            <h2>Ваша корзина</h2>

            {error && (<div className='error'>{error}</div>)}

            <div className='restaurant-groups'>
                {Object.entries(groupedCart).map(([restaurantId, group]) => (
                    <div key={restaurantId} className='restaurant-group'>
                        <div className='restaurant-header'>
                            <h3>{group.restaurantName}</h3>
                            {!hasMultipleRestaurants && (
                                <button
                                onClick={() => handleOrder(restaurantId)}
                                disabled={loading}
                                className='btn-order'
                                >
                                {loading ? 'Оформление...' : 'Оформить заказ'}
                                </button>
                            )}
                        </div>

                        <ul className='cart-items'>
                            {group.items.map(item => (
                                <li
                                    key={`${item.restaurantId}-${item.dishId}`}
                                    className='cart-item'
                                >
                                    <div>
                                        <strong>{item.name}</strong> — {item.unitPrice} ₽
                                    </div>

                                    <div className='quantity-controls'>
                                        <button
                                        onClick={() => setQuantity(item.dishId, item.restaurantId, item.quantity - 1)}
                                        className='btn-quantity'
                                        >
                                        −
                                        </button>
                                        <span className='quantity-value'>{item.quantity}</span>
                                        <button
                                        onClick={() => setQuantity(item.dishId, item.restaurantId, item.quantity + 1)}
                                        className='btn-quantity'
                                        >
                                        +
                                        </button>
                                        <button
                                        onClick={() => removeFromCart(item.dishId, item.restaurantId)}
                                        className='btn-remove'
                                        >
                                        Удалить
                                        </button>
                                    </div>
                                </li>
                            ))}
                        </ul>

                        <div className="cart-total">
                            Итого: <strong>{restaurantTotals[restaurantId].toFixed(2)} ₽</strong>
                        </div>

                        {hasMultipleRestaurants && (
                            <div className='group-warning'>
                                Удалите блюда из других ресторанов, чтобы оформить заказ.
                            </div>
                        )}
                    </div>
                ))}
            </div>

            {hasMultipleRestaurants && (
                <div className='global-warning'>
                    <p>Вы добавили блюда из нескольких ресторанов.</p>
                    <p className="mt-2">Оформить заказ можно только для одного ресторана за раз.</p>
                </div>
            )}
        </div>
    );
}