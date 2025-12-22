import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authService } from '../../services/authService';
import { isValidEmail, isValidPhone, isValidPassword, isRequired } from '../../utils/validators';
import './RegisterPage.css'

export default function RegisterPage() {
    const [formData, setFormData] = useState({
        username: '',
        password: '',
        fullName: '',
        phone: '',
        email: '',
        address: ''
    });
    const [errors, setErrors] = useState({});
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));

        if (errors[name]){
            setErrors(prev => ({...prev, [name]: '' }));
        }
    };

    const validate = () => {
        const newErrors = {};

        if (!isRequired(formData.username)) newErrors.username = 'Придумайте логин пользователя';
        if (!isValidPassword(formData.password)) newErrors.password = 'Пароль должен содержать не менее 6 символов';
        if (!isRequired(formData.fullName)) newErrors.fullName = 'Укажите свои имя и фамилию';
        if (!isRequired(formData.phone)) {
            newErrors.phone = 'Укажите номер телефона';
        } else if (!isValidPhone(formData.phone)) {
            newErrors.phone = 'Введите номер в формате +79991234567';
        }
        if (formData.email && !isValidEmail(formData.email)) {
            newErrors.email = 'Укажите email в верном формате';
        }
        if (!isRequired(formData.address)) newErrors.address = 'Укажите адрес доставки';
    
        return newErrors;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setErrors({});
        setLoading(true);

        const newErrors = validate();
        if (Object.keys(newErrors).length > 0) {
            setErrors(newErrors);
            setLoading(false);
            return;
        }

        try {
            await authService.register(formData);
            alert('Регистрация прошла успешно! Можете войти в аккаунт.');
            navigate('/login');
        } catch (err) {
            console.error('Registration error:', err);
            if (err.response?.status === 400) {
                const message = err.response.data || 'Ошибка регистрации';
                setErrors(typeof message === 'string' ? message : 'Неверные данные');
            } else {
                setErrors('Не удалось подключиться к серверу');
            }
            setLoading(false);
        }
    };

    return (
        <div className='form-register'>
            <h2>Регистрация</h2>

            {errors.submit && <div className='error-submit'>{errors.submit}</div>}

            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="username">Логин</label>
                    <input
                        id="username"
                        name="username"
                        type="text"
                        value={formData.username}
                        onChange={handleChange}
                        className={`input ${errors.username ? 'error' : ''}`}
                        required
                        disabled={loading}
                    />
                     {errors.username && <p className='field-error'>{errors.username}</p>}
                </div>

                <div>
                    <label htmlFor="password">Пароль</label>
                    <input
                        id="password"
                        name="password"
                        type="password"
                        value={formData.password}
                        onChange={handleChange}
                        className={`input ${errors.password ? 'error' : ''}`}
                        required
                        disabled={loading}
                    />
                    {errors.password && <p className='field-error'>{errors.password}</p>}
                </div>

                <div>
                    <label htmlFor="fullName">Имя Фамилия</label>
                    <input
                        id="fullName"
                        name="fullName"
                        type="text"
                        value={formData.fullName}
                        onChange={handleChange}
                        className={`input ${errors.fullName ? 'error' : ''}`}
                        required
                        disabled={loading}
                    />
                    {errors.fullName && <p className='field-error'>{errors.fullName}</p>}
                </div>

                <div>
                    <label htmlFor="phone">Телефон</label>
                    <input
                        id="phone"
                        name="phone"
                        type="tel"
                        value={formData.phone}
                        onChange={handleChange}
                        className={`input ${errors.phone ? 'error' : ''}`}
                        required
                        disabled={loading}
                    />
                    {errors.phone && <p className='field-error'>{errors.phone}</p>}
                </div>

                <div>
                    <label htmlFor="email">Email</label>
                    <input
                        id="email"
                        name="email"
                        type="email"
                        value={formData.email}
                        onChange={handleChange}
                        className={`input ${errors.email ? 'error' : ''}`}
                        disabled={loading}
                    />
                    {errors.email && <p className='field-error'>{errors.email}</p>}
                </div>

                <div>
                    <label htmlFor="address">Адрес</label>
                    <input
                        id="address"
                        name="address"
                        type="text"
                        value={formData.address}
                        onChange={handleChange}
                        className={`input ${errors.address ? 'error' : ''}`}
                        required
                        disabled={loading}
                    />
                    {errors.address && <p className='field-error'>{errors.address}</p>}
                </div>

                <button type="submit" disabled={loading}>
                    {loading ? 'Регистрация...' : 'Зарегистрироваться'}
                </button>
            </form>

            <div className="login-link">
                <p>
                    Уже есть аккаунт?{' '}
                    <Link to="/login">Войти</Link>
                </p>
            </div>
        </div>
    );
}