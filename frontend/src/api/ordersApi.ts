import { apiClient } from "./axios";
import type { OrderCreateResponse, OrderDetails, OrderSummary } from "../types";

const baseUrl = import.meta.env.VITE_ORDERS_URL;

export const ordersApi = {
  createOrder: async (payload: {
    customerEmail: string;
    customerName?: string;
    customerPhone?: string;
    shippingAddress?: string;
    items: { productId: string; productName: string; unitPrice: number; quantity: number }[];
    promoCode?: string;
  }) => {
    const response = await apiClient.post(`${baseUrl}/api/orders`, payload);
    return response.data as OrderCreateResponse;
  },
  validatePromoCode: async (payload: { code: string; total: number }) => {
    const response = await apiClient.post(`${baseUrl}/api/promocodes/validate`, payload);
    return response.data as { code: string; type: string; value: number; discountAmount: number; totalAfterDiscount: number };
  },
  adminGetOrders: async (params: { page?: number; pageSize?: number }) => {
    const response = await apiClient.get(`${baseUrl}/api/admin/orders`, { params });
    return response.data as { items: OrderSummary[]; page: number; pageSize: number; totalCount: number };
  },
  adminGetOrder: async (id: string) => {
    const response = await apiClient.get(`${baseUrl}/api/admin/orders/${id}`);
    return response.data as OrderDetails;
  },
  adminUpdateOrderStatus: async (id: string, status: string) => {
    const response = await apiClient.patch(`${baseUrl}/api/admin/orders/${id}/status`, { status });
    return response.data as OrderDetails;
  },
  adminGetPromoCodes: async () => {
    const response = await apiClient.get(`${baseUrl}/api/admin/promocodes`);
    return response.data as { id: string; code: string; type: string; value: number; isActive: boolean; expiresAtUtc?: string; usageLimit?: number; timesUsed: number }[];
  },
  adminCreatePromoCode: async (payload: { code: string; type: string; value: number; isActive: boolean; expiresAtUtc?: string; usageLimit?: number }) => {
    const response = await apiClient.post(`${baseUrl}/api/admin/promocodes`, payload);
    return response.data as { id: string; code: string; type: string; value: number; isActive: boolean; expiresAtUtc?: string; usageLimit?: number; timesUsed: number };
  },
  adminUpdatePromoCode: async (id: string, payload: { code: string; type: string; value: number; isActive: boolean; expiresAtUtc?: string; usageLimit?: number }) => {
    const response = await apiClient.put(`${baseUrl}/api/admin/promocodes/${id}`, payload);
    return response.data as { id: string; code: string; type: string; value: number; isActive: boolean; expiresAtUtc?: string; usageLimit?: number; timesUsed: number };
  },
  adminDeletePromoCode: async (id: string) => {
    await apiClient.delete(`${baseUrl}/api/admin/promocodes/${id}`);
  },
  getMyOrders: async () => {
    const response = await apiClient.get(`${baseUrl}/api/orders/my`);
    return response.data as OrderSummary[];
  },
  getOrderById: async (id: string) => {
    const response = await apiClient.get(`${baseUrl}/api/orders/${id}`);
    return response.data as OrderDetails;
  }
};
