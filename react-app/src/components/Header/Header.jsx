import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import ilogo from '../../assets/logo.svg';
import icart from '../../assets/cart.svg';
import ilist from '../../assets/list.svg';
import iprofile from '../../assets/profile.svg';
import './Header.css'

export default function Header() {
    const { currentUser, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
      logout();
      navigate('/login');
    };

    return (
      <header className='header'>
        <div className='container'>
          <Link to="/" className="logo-link">
            <img src={ilogo} alt="Логотип" />
            <span>FoodDel</span>
          </Link>

          {currentUser ? (
            <span>Приветствуем, {currentUser.fullName}!</span>
          ) : (
            <span>Добро пожаловать!</span>)
          }

          <div className='routes-section'>
            <Link to="/orders" className="orders-link">
              <img src={ilist} alt="Заказы" className="orders-icon" />
            </ Link>

            <Link to="/cart" className="cart-link">
              <img src={icart} alt="Корзина" className="cart-icon" />
            </ Link>

            <Link to="/profile" className="profile-link">
              <img src={iprofile} alt="Профиль" className="profile-icon" />
            </ Link>

            {currentUser ? (
              <>
                <button onClick={handleLogout} className='auth-button'>Выйти</button>
              </>
            ) : (
              <Link to="/login" className='auth-link'>Войти</Link>
            )}
          </div>
        </div>
      </header>
  );
}