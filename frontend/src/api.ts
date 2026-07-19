import axios from 'axios';
import { useAuthStore } from '@/stores/auth';

const baseURL =
import.meta.env.VITE_API_BASE as string || 'http://localhost:5000/api';

const api = axios.create({
  baseURL,
  timeout: 10000,
});

api.interceptors.request.use(config => {
  try {
  const auth = useAuthStore();
  const token = auth.token;
  if (token && config.headers) {
     config.headers.Authorization = 'Bearer ${token}'
    }
  } catch (e){
  }
  return config;
});

api.interceptors.response.use(
  r => r,
  err =>{
    return Promise.reject(err);
  }
);

export default api;