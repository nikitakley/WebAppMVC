import { createContext, useContext, useState, useEffect } from 'react';
import { authService } from '../services/authService';

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [currentUser, setCurrentUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = authService.getCurrentToken();
    const savedUser = authService.getCurrentUser();

    if (token && savedUser) {
      setCurrentUser(savedUser);
    }
    setLoading(false);
  }, []);

  const login = async (username, password) => {
    const { token, customer } = await authService.login(username, password);
    setCurrentUser(customer);
    authService.setCurrentUser(customer);
  };

  const logout = () => {
    authService.logout();
    setCurrentUser(null);
  };

  const updateCurrentUser = (updatedUser) => {
    setCurrentUser(updatedUser);
    authService.setCurrentUser(updatedUser);
  };

  const value = {
    currentUser,
    login,
    logout,
    updateCurrentUser,
    loading
  };

  return (
    <AuthContext.Provider value={value}>
      {!loading && children}
    </AuthContext.Provider>
  );
}

// Хук для удобного доступа к контексту
export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}