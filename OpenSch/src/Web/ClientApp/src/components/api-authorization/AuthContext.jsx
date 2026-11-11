import { createContext, useContext, useState, useEffect } from 'react';
import { UsersClient, LoginRequest, RegisterRequest } from '../../web-api-client';

const AuthContext = createContext(null);

const createClient = (turnstileToken) =>
  new UsersClient(undefined, {
    fetch: (url, init) => {
      const headers = new Headers(init?.headers);
      if (turnstileToken) {
        headers.set('X-Turnstile-Token', turnstileToken);
      }
      return fetch(url, { ...init, headers, credentials: 'include' });
    },
  });

const client = createClient();

const loadAdminStatus = () =>
  fetch('/api/admin/me', { credentials: 'include', headers: { Accept: 'application/json' } })
    .then((response) => (response.ok ? response.json() : null))
    .then((data) => Boolean(data?.isAdministrator))
    .catch(() => false);

export function AuthProvider({ children }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isAdmin, setIsAdmin] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    client.infoGET()
      .then(() => {
        setIsAuthenticated(true);
        return loadAdminStatus().then(setIsAdmin);
      })
      .catch(() => {
        setIsAuthenticated(false);
        setIsAdmin(false);
      })
      .finally(() => setIsLoading(false));
  }, []);

  const login = (email, password, remember = true) =>
    client.login(true, !remember, new LoginRequest({ email, password }))
      .then(() => setIsAuthenticated(true))
      .then(() => loadAdminStatus().then(setIsAdmin));

  const register = (email, password, turnstileToken) =>
    createClient(turnstileToken).register(new RegisterRequest({ email, password }));

  const logout = () =>
    client.logout({})
      .then(() => setIsAuthenticated(false))
      .then(() => setIsAdmin(false));

  return (
    <AuthContext.Provider value={{ isAuthenticated, isAdmin, isLoading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
