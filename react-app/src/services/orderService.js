import api from './api';

export const orderService = {
    createOrder: (orderData) => 
        api.post('/orders', orderData).then(res => res.data),

    addOrderItem: (itemData) => 
        api.post('/items', itemData).then(res => res.data),

    getOrderById: (orderId) => 
        api.get(`/orders/${orderId}`).then(res => res.data),

    getOrderByCustomer: (customerId) =>
        api.get(`/orders/customer/${customerId}`).then(res => res.data),

    getAllOrders: () =>
        api.get('/orders').then(res => res.data)
};