import { createContext, useContext, useState, useEffect } from 'react';

const CartContext = createContext();

export function CartProvider({ children }) {
    const [cart, setCart] = useState(() => {
        const saved = localStorage.getItem('cart');
        return saved ? JSON.parse(saved) : {};
    });

    useEffect(() => {
        localStorage.setItem('cart', JSON.stringify(cart));
    }, [cart]);

    const addToCart = (dish, restaurantId, restaurantName) => {
        const key = `${restaurantId}-${dish.dishId}`;
        setCart(prev => {
            const existing = prev[key];
            return {
                ...prev,
                [key]: {
                    ...dish,
                    restaurantId,
                    restaurantName,
                    quantity: (existing?.quantity || 0) + 1,
                    unitPrice: dish.price
                }
            };
        });
    };

    const setQuantity = (dishId, restaurantId, quantity) => {
        if (quantity < 1) {
            removeFromCart(dishId, restaurantId);
            return;
        }
        const key = `${restaurantId}-${dishId}`;
        setCart(prev => ({
            ...prev,
            [key]: { ...prev[key], quantity }
        }));
    };

    const removeFromCart = (dishId, restaurantId) => {
        const key = `${restaurantId}-${dishId}`;
        setCart(prev => {
            const newCart = { ...prev };
            delete newCart[key];
            return newCart;
        });
    };

    const clearCart = () => {
        setCart({});
    };

    const getCartItems = () => Object.values(cart);

    const isEmpty = () => getCartItems().length === 0;

    return (
        <CartContext.Provider
        value={{
            cart: getCartItems(),
            addToCart,
            setQuantity,
            removeFromCart,
            clearCart,
            isEmpty
        }}
        >
        {children}
        </CartContext.Provider>
    );
}

export function useCart() {
    const context = useContext(CartContext);
    if (!context) {
        throw new Error('useCart must be used within a CartProvider');
    }
    return context;
}