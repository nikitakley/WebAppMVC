import { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { customerService } from '../../services/customerService';
import './ProfilePage.css';

export default function ProfilePage() {
    const { currentUser, updateCurrentUser } = useAuth();

    const [formData, setFormData] = useState({
        fullName: '',
        phone: '',
        email: '',
        address: ''
    });

    const [isEditing, setIsEditing] = useState(false);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    useEffect(() => {
        if (currentUser) {
        setFormData({
            fullName: currentUser.fullName || '',
            phone: currentUser.phone || '',
            email: currentUser.email || '',
            address: currentUser.address || ''
        });
        }
    }, [currentUser]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSave = async () => {
        setLoading(true);
        setError('');
        setSuccess('');

        try {
            const updatedCustomer = await customerService.updateCustomer(
                currentUser.customerId, 
                formData
            );

            const newCurrentUser = { ...currentUser, ...updatedCustomer };
            updateCurrentUser(newCurrentUser);
            setSuccess('Профиль успешно обновлён!');
            setIsEditing(false);
        } catch (err) {
            console.error('Profile update error:', err);
            setError('Не удалось обновить профиль. Попробуйте позже.');
        } finally {
            setLoading(false);
        }
    };

    if (!currentUser) {
        return <div>Загрузка профиля...</div>;
    }

    return (
        <div className='profile-page'>
            <h1>Мой профиль</h1>

            {error && <div className='error-message'>{error}</div>}
            {success && <div className='success-message'>{success}</div>}

            <form className='profile-form'>
                <div className='form-group'>
                    <label htmlFor="fullName">ФИО:</label>
                    {isEditing ? (
                        <input
                        id="fullName"
                        name="fullName"
                        type="text"
                        value={formData.fullName}
                        onChange={handleChange}
                        className='form-input'
                        required
                        />
                    ) : (
                        <p>{formData.fullName}</p>
                    )}
                </div>
                <div className='form-group'>
                    <label htmlFor="phone">Телефон:</label>
                    {isEditing ? (
                        <input
                        id="phone"
                        name="phone"
                        type="tel"
                        value={formData.phone}
                        onChange={handleChange}
                        className='form-input'
                        required
                        />
                    ) : (
                        <p>{formData.phone}</p>
                    )}
                </div>
                <div className='form-group'>
                    <label htmlFor="email">Email:</label>
                    {isEditing ? (
                        <input
                        id="email"
                        name="email"
                        type="email"
                        value={formData.email}
                        onChange={handleChange}
                        className='form-input'
                        />
                    ) : (
                        <p>{formData.email || '—'}</p>
                    )}
                </div>
                <div className='form-group'>
                    <label htmlFor="address">Адрес:</label>
                    {isEditing ? (
                        <input
                        id="address"
                        name="address"
                        type="text"
                        value={formData.address}
                        onChange={handleChange}
                        className='form-input'
                        required
                        />
                    ) : (
                        <p>{formData.address}</p>
                    )}
                </div>

                <div className='form-actions'>
                    {isEditing ? (
                        <>
                        <button 
                            type="button" 
                            onClick={handleSave}
                            disabled={loading} 
                            className='btn-primary'
                        >
                            {loading ? 'Сохранение...' : 'Сохранить'}
                        </button> 
                        <button
                            type="button"
                            onClick={() => setIsEditing(false)}
                            className='btn-secondary'
                        >
                            Отмена
                        </button>
                        </>
                        ) : (
                            <button
                            type="button"
                            onClick={() => setIsEditing(true)}
                            className='btn-edit'
                            >
                            Редактировать профиль
                            </button>
                        )
                    }
                </div>
            </form>
        </div>
    );
}