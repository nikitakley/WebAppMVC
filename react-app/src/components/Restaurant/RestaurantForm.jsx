import { useState, useEffect } from 'react';
import './RestaurantForm.css'

export default function RestaurantForm({ initialData, onSave, onCancel }) {
    const [name, setName] = useState(initialData?.name || '');
    const [rating, setRating] = useState(initialData?.rating?.toString() || '0.0');

    useEffect(() => {
        if (initialData) {
        setName(initialData.name);
        setRating(initialData.rating.toString());
        }
    }, [initialData]);

    const handleSubmit = (e) => {
        e.preventDefault();
        const trimmedName = name.trim();
        const numRating = parseFloat(rating);
        
        if (trimmedName && !isNaN(numRating) && numRating >= 0 && numRating <= 5) {
            onSave({ name: trimmedName, rating: numRating });
        } else {
            alert('Проверьте данные: название не должно быть пустым, рейтинг — от 0.0 до 5.0');
        }
    };

    return (
        <form onSubmit={handleSubmit} className='restaurant-form'>
        <div className='form-group'>
            <label htmlFor="name">Название ресторана:</label>
            <input
            id="name"
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
            className='form-input'
            />
        </div>
        <div className='form-group'>
            <label htmlFor="rating">Рейтинг (0.0 – 5.0):</label>
            <input
            id="rating"
            type="number"
            min="0"
            max="5"
            step="0.1"
            value={rating}
            onChange={(e) => setRating(e.target.value)}
            required
            className='form-input'
            />
        </div>
        <div className='form-actions'>
            <button type="submit" className='btn-primary'>
            {initialData ? 'Сохранить' : 'Добавить'}
            </button>
            <button type="button" onClick={onCancel} className='btn-secondary'>
            Отмена
            </button>
        </div>
        </form>
  );
}