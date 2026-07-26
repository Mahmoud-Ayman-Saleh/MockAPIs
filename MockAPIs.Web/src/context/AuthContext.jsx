import { createContext, useContext, useState, useEffect } from 'react';
import { api } from '../services/api';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const stored = localStorage.getItem('user');
    return stored ? JSON.parse(stored) : null;
  });
  const [token, setToken] = useState(() => localStorage.getItem('token'));
  const [loading, setLoading] = useState(false);

  const saveAuthSession = (authData) => {
    const { token, refreshToken, user } = authData;
    setToken(token);
    setUser(user);
    localStorage.setItem('token', token);
    localStorage.setItem('refreshToken', refreshToken);
    localStorage.setItem('user', JSON.stringify(user));
  };

  const login = async (userName, password) => {
    setLoading(true);
    try {
      const response = await api.post('/api/Auth/Login', { userName, password });
      saveAuthSession(response.data);
      return response.data;
    } finally {
      setLoading(false);
    }
  };

  const register = async (userName, email, password) => {
    setLoading(true);
    try {
      const response = await api.post('/api/Auth/Register', { userName, email, password });
      saveAuthSession(response.data);
      return response.data;
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    setUser(null);
    setToken(null);
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  };

  return (
    <AuthContext.Provider value={{ user, token, isAuthenticated: !!token, loading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
