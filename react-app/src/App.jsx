import { useState } from 'react'
import { BrowserRouter, Routes, Route } from 'react-router-dom';

import { AuthProvider } from './context/AuthContext';
import { CartProvider } from './context/CartContext';

import ProtectedRoute from './components/ProtectedRoute/ProtectedRoute';
import Header from './components/Header/Header';
import Footer from './components/Footer/Footer'

import RegisterPage from './pages/RegisterPage/RegisterPage'
import LoginPage from './pages/LoginPage/LoginPage';
import RestaurantsPage from './pages/RestaurantsPage/RestaurantPage';
import OrdersPage from './pages/OrdersPage/OrderPage'
import CartPage from './pages/CartPage/CartPage'
import ProfilePage from './pages/ProfilePage/ProfilePage'

import './App.css'

function App() {
  return (
    <AuthProvider>
      <CartProvider>
        <BrowserRouter>
          <div className="App">
            <Header />
              <Routes>
                <Route path="/register" element={<RegisterPage />} />
                <Route path="/login" element={<LoginPage />} />
                <Route path="/" element={<RestaurantsPage />} />
                <Route path="/restaurants" element={<RestaurantsPage />} />

                <Route element={<ProtectedRoute />}>
                  <Route path="/orders" element={<OrdersPage />} />
                  <Route path="/cart" element={<CartPage />} />
                  <Route path='/profile' element={<ProfilePage />} />
                </Route>
              </Routes>
              <Footer />
          </div>
        </BrowserRouter>
      </CartProvider>
    </AuthProvider>
  )
}

export default App
