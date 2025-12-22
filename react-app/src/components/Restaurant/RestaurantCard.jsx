import { useState } from 'react';
import { useCart } from '../../context/CartContext'
import { restaurantService } from '../../services/restaurantService';
import restaurantImage from '../../assets/restaurant.png'
import './RestaurantCard.css'

export default function RestaurantCard({ restaurant, isAdmin, onEdit, onDelete}) {
    const [showMenu, setShowMenu] = useState(false);
    const [dishes, setDishes] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const { addToCart } = useCart();

    const handleViewMenu = async () => {
        if (showMenu && dishes.length > 0) {
            setShowMenu(false);
            return;
        }

        setShowMenu(true);
        setLoading(true);
        setError(null);

        try {
            const menu = await restaurantService.getMenu(restaurant.restaurantId);
            setDishes(menu);
        } catch (err) {
            console.error('Failed to load menu:', err);
            setError('Не удалось загрузить меню');
        } finally {
            setLoading(false);
        }
    };

    return (
    <div className='restaurant-card'>
        <img
            src={restaurantImage}
            alt={restaurant.name}
            className='restaurant-image'
        />

        {isAdmin && (
        <div className='admin-actions'>
            <button onClick={onEdit} className='btn-edit'>Редактировать</button>
            <button onClick={onDelete} className='btn-delete'>Удалить</button>
        </div>
        )}

        <h3>{restaurant.name}</h3>
        <div className='rating'>
            <span className='star'>★</span>
            <span className='value'>{restaurant.rating.toFixed(1)}</span>
        </div>

        <button
            onClick={handleViewMenu}
            className='menu-btn'
            disabled={loading}
        >
        {loading ? 'Загрузка...' : showMenu ? 'Скрыть меню' : 'Меню'}
        </button>

        {showMenu && (
            <div className='menu-content'>
            {error ? (
                <p className='error'>{error}</p>
            ) : loading ? (
                <p>Загрузка меню...</p>
            ) : dishes.length > 0 ? (
                <ul>
                    {dishes.map(dish => (
                        <li key={dish.dishId}>
                            <strong>{dish.name}</strong> — {dish.price} ₽
                            <button
                                onClick={() => addToCart(dish, restaurant.restaurantId, restaurant.name)}
                                className='btn-add-to-cart'
                                disabled={!dish.isAvailable}
                            >
                            +
                            </button>
                        </li>
                    ))}
                </ul>
            ) : (
                <p>Меню пусто</p>
            )}
            </div>
        )}
    </div>
  );
}