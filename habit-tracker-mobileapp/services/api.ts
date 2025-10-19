import { ACCESS_TOKEN_KEY, REFRESH_TOKEN_KEY, clearToken, getToken, setToken } from '@/utils/helpers/authHelpers';
import axios, { AxiosError } from 'axios';

export const BASE_URL = 'https://d4a83ca6bed3.ngrok-free.app/v1';

// ---- Endpoints ----
export const endpoints = {  
  // Auth
  login: '/Auth/login',
  register: '/Auth/register',
  refreshToken: '/Auth/refresh-token',

  // User
  currentUser: '/User/me', 

  //Habit
  habit: 'Habit'

};

// ---- API instances ----
const api = axios.create({
  baseURL: BASE_URL,
  withCredentials: false,
});

export const authApi = axios.create({
  baseURL: BASE_URL,
  withCredentials: true,
});

// ---- Request interceptor ----
authApi.interceptors.request.use(
  async (config) => {
    const accessToken = await getToken(ACCESS_TOKEN_KEY); 
    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// ---- Response interceptor ----
authApi.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as typeof error.config & { _retry?: boolean };

    if (error.response?.status === 401 && originalRequest && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = await getToken(REFRESH_TOKEN_KEY); 
        if (!refreshToken) {
          await clearToken(); 
          return Promise.reject(error);
        }

        const response = await api.post(endpoints.refreshToken, { refreshToken });
        const { accessToken, refreshToken: newRefreshToken } = response.data as {
          accessToken: string;
          refreshToken: string;
        };

        await setToken(ACCESS_TOKEN_KEY, accessToken); // await
        if (newRefreshToken) await setToken(REFRESH_TOKEN_KEY, newRefreshToken); // await

        if (originalRequest.headers) {
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
        }

        return authApi(originalRequest);
      } catch (refreshError) {
        await clearToken(); // await
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default api;