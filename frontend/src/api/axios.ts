import axios from "axios";

export const apiClient = axios.create();
export const authClient = axios.create({ withCredentials: true });

const authBaseUrl = import.meta.env.VITE_AUTH_URL;

const refreshAccessToken = async () => {
  const rememberMe = localStorage.getItem("rememberMe") === "true";
  const response = await authClient.post(`${authBaseUrl}/api/identity/refreshtoken`, { rememberMe });
  const data = response.data as { accessToken: string; refreshToken: string };
  localStorage.setItem("accessToken", data.accessToken);
  return data.accessToken;
};

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");
  if (token) {
    config.headers = config.headers ?? {};
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const original = error.config as (typeof error.config & { _retry?: boolean });
    if (error.response?.status === 401 && original && !original._retry && !original.url?.includes("/api/identity/refreshtoken")) {
      original._retry = true;
      try {
        const token = await refreshAccessToken();
        original.headers = original.headers ?? {};
        original.headers.Authorization = `Bearer ${token}`;
        return apiClient(original);
      } catch {
        localStorage.removeItem("accessToken");
      }
    }
    return Promise.reject(error);
  }
);
