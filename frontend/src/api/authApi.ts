import { apiClient, authClient } from "./axios";
import type { UserInfo } from "../types";

const baseUrl = import.meta.env.VITE_AUTH_URL;

export const authApi = {
  register: async (payload: { email: string; password: string; firstname: string; lastname: string; age: number; rememberMe?: boolean }) => {
    await authClient.post(`${baseUrl}/api/identity/register`, payload);
  },
  login: async (payload: { email: string; password: string; rememberMe?: boolean }) => {
    const response = await authClient.post(`${baseUrl}/api/identity/login`, payload);
    return response.data as { accessToken: string; refreshToken: string };
  },
  me: async () => {
    const response = await apiClient.get(`${baseUrl}/api/identity/me`);
    return response.data as UserInfo;
  },
  resetPassword: async (email: string) => {
    await apiClient.post(`${baseUrl}/api/identity/resetpassword`, { email });
  },
  confirmResetPassword: async (payload: { email: string; resetCode: string; newPassword: string }) => {
    await apiClient.post(`${baseUrl}/api/identity/confirmresetpassword`, payload);
  },
  changePassword: async (payload: { email: string; currentPassword: string; newPassword: string }) => {
    await apiClient.post(`${baseUrl}/api/identity/changepassword`, payload);
  },
  refreshToken: async (rememberMe?: boolean) => {
    const response = await authClient.post(`${baseUrl}/api/identity/refreshtoken`, { rememberMe: Boolean(rememberMe) });
    return response.data as { accessToken: string; refreshToken: string };
  },
  logout: async () => {
    await authClient.post(`${baseUrl}/api/identity/logout`, {});
  },
  getProfile: async () => {
    const response = await apiClient.get(`${baseUrl}/api/profile`);
    return response.data as { userId: string; email: string; firstname: string; lastname: string; age: number };
  },
  updateProfile: async (payload: { firstname: string; lastname: string; age: number }) => {
    const response = await apiClient.put(`${baseUrl}/api/profile`, payload);
    return response.data as { userId: string; email: string; firstname: string; lastname: string; age: number };
  },
  promoteUser: async (email: string) => {
    await apiClient.post(`${baseUrl}/api/admin/users/promote`, { email });
  },
  demoteUser: async (email: string) => {
    await apiClient.post(`${baseUrl}/api/admin/users/demote`, { email });
  },
  adminGetUsers: async () => {
    const response = await apiClient.get(`${baseUrl}/api/admin/users`);
    return response.data as { id: string; email: string; firstname: string; lastname: string; age: number; roles: string[] }[];
  },
  getAddresses: async () => {
    const response = await apiClient.get(`${baseUrl}/api/profile/addresses`);
    return response.data as { id: string; label?: string; line1: string; line2?: string; city: string; region?: string; postalCode: string; country: string; phone?: string; isDefault: boolean; createdAtUtc: string }[];
  },
  createAddress: async (payload: { label?: string; line1: string; line2?: string; city: string; region?: string; postalCode: string; country: string; phone?: string; isDefault: boolean }) => {
    const response = await apiClient.post(`${baseUrl}/api/profile/addresses`, payload);
    return response.data as { id: string; label?: string; line1: string; line2?: string; city: string; region?: string; postalCode: string; country: string; phone?: string; isDefault: boolean; createdAtUtc: string };
  },
  updateAddress: async (id: string, payload: { label?: string; line1: string; line2?: string; city: string; region?: string; postalCode: string; country: string; phone?: string; isDefault: boolean }) => {
    const response = await apiClient.put(`${baseUrl}/api/profile/addresses/${id}`, payload);
    return response.data as { id: string; label?: string; line1: string; line2?: string; city: string; region?: string; postalCode: string; country: string; phone?: string; isDefault: boolean; createdAtUtc: string };
  },
  deleteAddress: async (id: string) => {
    await apiClient.delete(`${baseUrl}/api/profile/addresses/${id}`);
  }
};
