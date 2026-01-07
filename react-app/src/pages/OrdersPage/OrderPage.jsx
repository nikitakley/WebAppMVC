import { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { orderService } from '../../services/orderService';
import OrderCard from '../../components/Order/OrderCard'
import OrderTable from '../../components/Order/OrderTable'
import './OrderPage.css';

export default function OrdersPage() {
    const { currentUser } = useAuth();
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [viewMode, setViewMode] = useState('cards');

    // поиск и сортировка
    const [searchTerm, setSearchTerm] = useState('');
    const [sortOrder, setSortOrder] = useState('desc');

    useEffect(() => {
        const fetchOrders = async () => {
        try {
            let data;
            if (currentUser?.role === 'Admin') {
                data = await orderService.getAllOrders();
            } else {
                data = await orderService.getOrderByCustomer(currentUser.customerId);
            }
            setOrders(data);
        } catch (err) {
            console.error('Failed to fetch orders:', err);
            setError('Не удалось загрузить заказы');
        } finally {
            setLoading(false);
        }
        };

        if (currentUser) {
        fetchOrders();
        }
    }, [currentUser]);

    const filteredOrders = orders
        .filter(order => {
            const matchesSearch = order.orderId.toString().includes(searchTerm);
            return matchesSearch;
        })
        .sort((a, b) => {
            if (sortOrder === 'asc') {
                return a.orderId - b.orderId;
            } else {
                return b.orderId - a.orderId;
            }
        });

    if (loading) return <div className="orders-page">Загрузка заказов...</div>;
    if (error) return <div className="orders-page error">{error}</div>;

    return (
        <div className='orders-page'>
            <div className="view-toggle">
                <button
                    className={`view-btn ${viewMode === 'cards' ? 'active' : ''}`}
                    onClick={() => setViewMode('cards')}
                >
                    Карточки
                </button>
                <button
                    className={`view-btn ${viewMode === 'table' ? 'active' : ''}`}
                    onClick={() => setViewMode('table')}
                >
                    Таблица
                </button>
            </div>

            <div className="filters-section">
                <div className="filter-group">
                    <label htmlFor="search">Поиск:</label>
                    <input
                        id="search"
                        type="text"
                        placeholder="Номер заказа..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className="search-input"
                    />
                </div>   

                <div className="filter-group">
                    <label htmlFor="sort">Сортировка:</label>
                    <select
                        id="sort"
                        value={sortOrder}
                        onChange={(e) => setSortOrder(e.target.value)}
                        className="sort-select"
                    >
                        <option value="desc">Сначала новые</option>
                        <option value="asc">Сначала старые</option>
                    </select>
                </div>        
            </div>

            <h1>
                {currentUser?.role === 'Admin' ? 'Все заказы' : 'Мои заказы'}
            </h1>

            {filteredOrders.length === 0 ? (
                <p className="empty-message">Заказов пока нет.</p>
                ) : viewMode === 'cards' ? (
                <div className='orders-list'>
                    {filteredOrders.map(order => (
                        <OrderCard 
                            key={order.orderId} 
                            order={order}
                            showCustomerName={currentUser?.role === 'Admin'} />
                    ))}
                </div>
                ) : (
                    <div className='orders-table'>
                        <OrderTable
                            orders={filteredOrders}
                            showCustomerName={currentUser?.role === 'Admin'}
                        />
                    </div>
                )}
        </div>
    );
}

