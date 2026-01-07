import api from './api';

export const authService = {
  login: async (username, password) => {
    const response = await api.post('/auth/login', { username, password });
    const {token, customer} = response.data;

    if (token && customer){
      localStorage.setItem('token', token);
      localStorage.setItem('currentUser', JSON.stringify(customer));
      return {token, customer};
    }
    throw new Error('Неверный ответ от сервера');
  },

  register: async (userData) => {
    // userData = { username, password, fullName, phone, email, address }
    const response = await api.post('/auth/register', userData);
    return response.data;
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');

    localStorage.removeItem('cart')
  },

  getCurrentToken: () => localStorage.getItem('token'),
  
  getCurrentUser: () => {
    const user = localStorage.getItem('currentUser');
    return user ? JSON.parse(user) : null;
  },

  setCurrentUser: (user) => {
    localStorage.setItem('currentUser', JSON.stringify(user));
  },
};