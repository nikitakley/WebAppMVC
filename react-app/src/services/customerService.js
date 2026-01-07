import api from './api'

export const customerService = {
    updateCustomer: (customerId, customerData) =>
        api.put(`/customers/${customerId}`, customerData).then(res => res.data),

    getCustomerById: (customerId) =>
        api.get(`/customers/${customerId}`).then(res => res.data)
}