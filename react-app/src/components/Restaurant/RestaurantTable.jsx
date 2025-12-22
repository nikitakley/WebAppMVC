import './RestaurantTable.css'

export default function RestaurantTable({ restaurants, menuMap, loadingMenu }) {
  return (
      <table className='restaurant-table'>
        <thead>
          <tr>
            <th>Название</th>
            <th>Рейтинг</th>
            <th>Меню</th>
          </tr>
        </thead>
        <tbody>
          {restaurants.map(restaurant => (
            <tr key={restaurant.restaurantId}>
              <td>{restaurant.name}</td>
              <td>★ {restaurant.rating.toFixed(1)}</td>
              <td>
                {loadingMenu[restaurant.restaurantId] ? (
                  <em>Загрузка...</em>
                ) : menuMap[restaurant.restaurantId]?.length > 0 ? (
                  <ul className='dishes-list'>
                    {menuMap[restaurant.restaurantId].map(dish => (
                      <li key={dish.dishId}>
                        {dish.name} — {dish.price} ₽
                        {!dish.isAvailable && (
                          <span className='dish-unavailable'> (недоступно)</span>
                        )}
                      </li>
                    ))}
                  </ul>
                ) : (
                  <em>Меню пусто</em>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
  );
}
