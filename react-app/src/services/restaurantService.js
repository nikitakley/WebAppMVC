import api from './api';

export const restaurantService = {
    getAll: () => 
        api.get('/restaurants').then(response => response.data),

    getMenu: (restaurantId) => 
        api.get(`/restaurants/${restaurantId}/menu`).then(response => response.data.dishes),

    createRestaurant: (data) =>
        api.post(`/restaurants`, data).then(response => response.data),

    updateRestaurant: (restaurantId, data) =>
        api.put(`/restaurants/${restaurantId}`, data).then(response => response.data),

    deleteRestaurant: (restaurantId) => 
        api.delete(`/restaurants/${restaurantId}`).then(() => id)
};