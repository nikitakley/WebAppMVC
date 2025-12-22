import { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { restaurantService } from '../../services/restaurantService';
import RestaurantCard from '../../components/Restaurant/RestaurantCard';
import RestaurantTable from '../../components/Restaurant/RestaurantTable'
import RestaurantForm from '../../components/Restaurant/RestaurantForm';
import './RestaurantPage.css'

export default function RestaurantsPage() {
    // получаем роль пользователя
    const { currentUser } = useAuth();
    const isAdmin = currentUser?.role === 'Admin';

    const [restaurants, setRestaurants] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [viewMode, setViewMode] = useState('cards');

    const [menuMap, setMenuMap] = useState({});     // { 1: [dish1, dish2], ... }
    const [loadingMenu, setLoadingMenu] = useState({});     // { 1: true, ... }

    // поиск и фильтр
    const [searchTerm, setSearchTerm] = useState('');
    const [minRating, setMinRating] = useState('');

    // для формы
    const [showAddModal, setShowAddModal] = useState(false);
    const [editingRestaurant, setEditingRestaurant] = useState(null);

    useEffect(() => {
        const fetchRestaurants = async () => {
        try {
            const data = await restaurantService.getAll();
            setRestaurants(data);
        } catch (err) {
            console.error('Failed to fetch restaurants:', err);
            setError('Не удалось загрузить рестораны');
        } finally {
            setLoading(false);
        }
        };

        fetchRestaurants();
    }, []);

    useEffect(() => {
        if (viewMode === 'table' && restaurants.length > 0) {
            const loadAllMenus = async () => {
                const newLoadingMenu = {};
                const toLoad = restaurants.filter(r => !menuMap[r.restaurantId]);
                toLoad.forEach(r => { newLoadingMenu[r.restaurantId] = true; });
                if (Object.keys(newLoadingMenu).length === 0) return;
                
                setLoadingMenu(prev => ({ ...prev, ...newLoadingMenu }));
                const results = await Promise.all(
                    toLoad.map(async (r) => {
                        try {
                            const dishes = await restaurantService.getMenu(r.restaurantId);
                            return { id: r.restaurantId, dishes };
                        } catch (err) {
                            console.error(`Failed to load menu for restaurant ${r.restaurantId}:`, err);
                            return { id: r.restaurantId, dishes: [] };
                        }
                    })
                );

                const newMenuMap = {};
                results.forEach(({ id, dishes }) => { newMenuMap[id] = dishes; });
                setMenuMap(prev => ({ ...prev, ...newMenuMap }));
                setLoadingMenu(prev => {
                    const updated = { ...prev };
                    results.forEach(({ id }) => { updated[id] = false; });
                    return updated;
                });
            };
            loadAllMenus();
        }
    }, [viewMode, restaurants, menuMap]);

    const filteredRestaurants = restaurants.filter(restaurant => {
        const matchesSearch = restaurant.name.toLowerCase().includes(searchTerm.toLowerCase());
        const matchesRating = minRating ? restaurant.rating >= parseFloat(minRating) : true;
        return matchesSearch && matchesRating;
    });

    // обработчики для формы
    const handleAddRestaurant = async (data) => {
        try {
            const newRestaurant = await restaurantService.createRestaurant(data);
            setRestaurants(prev => [...prev, newRestaurant]);
            setShowAddModal(false);
        } catch (err) {
            alert('Ошибка при добавлении ресторана');
        }
    };

    const handleEditRestaurant = async (data) => {
        try {
            const updatedRestaurant = await restaurantService.updateRestaurant(editingRestaurant.restaurantId, data);
            setRestaurants(prev =>
                prev.map(r => r.restaurantId === updatedRestaurant.restaurantId ? updatedRestaurant : r)
            );
            setEditingRestaurant(null);
        } catch (err) {
            alert('Ошибка при обновлении ресторана');
        }
    };

    const handleDeleteRestaurant = async (id) => {
        if (!window.confirm('Удалить ресторан?')) return;
        try {
            await restaurantService.deleteRestaurant(id);
            setRestaurants(prev => prev.filter(r => r.restaurantId !== id));
        } catch (err) {
            alert('Ошибка при удалении ресторана');
        }
    };
    
    if (loading) return <div>Загрузка ресторанов...</div>;
    if (error) return <div>{error}</div>;

    return (
        <div className='restaurant-page'>
            {isAdmin && (
            <button
            className='btn-admin-add'
            onClick={() => setShowAddModal(true)}
            >
            + Добавить ресторан
            </button>
            )}
            
            <div className='view-toggle'>
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

            <div className='filters-section'>
                <div className='filter-group'>
                    <label htmlFor="search">Поиск:</label>
                    <input
                        id="search"
                        type="text"
                        placeholder="Название ресторана..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className='search-input'
                    />
                </div>

                <div className='filter-group'>
                    <label htmlFor="rating">Мин. рейтинг:</label>
                    <select
                        id="rating"
                        value={minRating}
                        onChange={(e) => setMinRating(e.target.value)}
                        className='rating-select'
                    >
                        <option value="">Все</option>
                        <option value="4.5">4.5 и выше</option>
                        <option value="4.0">4.0 и выше</option>
                        <option value="3.5">3.5 и выше</option>
                    </select>
                </div>            
            </div>

            <h1>Доступные рестораны</h1>

            {filteredRestaurants.length === 0 ? (
                <p className='empty-message'>Рестораны пока не добавлены.</p>
                ) : viewMode === 'cards' ? (
                <div className='restaurant-cards-container'>
                    {filteredRestaurants.map(restaurant => (
                        <RestaurantCard 
                            key={restaurant.restaurantId} 
                            restaurant={restaurant} 
                            isAdmin={isAdmin}
                            onEdit={() => setEditingRestaurant(restaurant)}
                            onDelete={() => handleDeleteRestaurant(restaurant.restaurantId)}
                        />
                    ))}
                </div>
                ) : (
                    <div className='restaurant-table-container'>
                        <RestaurantTable
                            restaurants={filteredRestaurants}
                            menuMap={menuMap}
                            loadingMenu={loadingMenu}
                        />
                    </div>
                )}

            {showAddModal && (
            <div className='modal-overlay' onClick={() => setShowAddModal(false)}>
                <div className='modal-content' onClick={(e) => e.stopPropagation()}>
                <h3>Добавить ресторан</h3>
                <RestaurantForm 
                    onSave={handleAddRestaurant} 
                    onCancel={() => setShowAddModal(false)} 
                />
                </div>
            </div>
            )}

            {editingRestaurant && (
            <div className='modal-overlay' onClick={() => setEditingRestaurant(null)}>
                <div className='modal-content' onClick={(e) => e.stopPropagation()}>
                <h3>Редактировать ресторан</h3>
                <RestaurantForm 
                    initialData={editingRestaurant}
                    onSave={handleEditRestaurant} 
                    onCancel={() => setEditingRestaurant(null)} 
                />
                </div>
            </div>
            )}
        </div>
    );
}