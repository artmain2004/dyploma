import { apiClient } from "./axios";
import type { CartItem } from "../types";

const baseUrl = import.meta.env.VITE_ORDERS_URL;

export const cartApi = {
  getCart: async () => {
    const response = await apiClient.get(`${baseUrl}/api/cart`);
    return response.data as { items: CartItem[] };
  },
  addItem: async (payload: CartItem) => {
    const response = await apiClient.post(`${baseUrl}/api/cart/items`, payload);
    return response.data as { items: CartItem[] };
  },
  updateQuantity: async (productId: string, quantity: number) => {
    const response = await apiClient.patch(`${baseUrl}/api/cart/items/${productId}`, { quantity });
    return response.data as { items: CartItem[] };
  },
  removeItem: async (productId: string) => {
    const response = await apiClient.delete(`${baseUrl}/api/cart/items/${productId}`);
    return response.data as { items: CartItem[] };
  },
  clear: async () => {
    const response = await apiClient.delete(`${baseUrl}/api/cart`);
    return response.data as { items: CartItem[] };
  }
};
