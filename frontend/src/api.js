// In Docker or production the address comes from VITE_API_URL; locally it is the dotnet run port.
const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5208';

export async function apiFetch(path, options = {}) {
  const token = localStorage.getItem('token');
  const headers = { 'Content-Type': 'application/json' };
  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(API_URL + path, { ...options, headers });

  // A 401 with a token means it expired or is invalid: log out and go back to the login page.
  if (response.status === 401 && token) {
    logout();
    window.location.href = '/login';
  }

  return response;
}

export function saveLogin(data) {
  localStorage.setItem('token', data.token);
  localStorage.setItem('name', data.name);
  localStorage.setItem('role', data.role);
}

// Keeps the name in the nav bar in sync after a profile update.
export function saveName(name) {
  localStorage.setItem('name', name);
}

export function logout() {
  localStorage.removeItem('token');
  localStorage.removeItem('name');
  localStorage.removeItem('role');
}

export function getRole() {
  return localStorage.getItem('role');
}

export function getName() {
  return localStorage.getItem('name');
}

export function isLoggedIn() {
  return localStorage.getItem('token') !== null;
}
