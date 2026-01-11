import { apiClient } from "./axios";
import type { Category, ProductDetails, ProductListResponse, Review } from "../types";

const baseUrl = import.meta.env.VITE_CATALOG_URL;

type AdminProductPayload = {
  name: string;
  description?: string;
  price: number;
  imageUrl?: string;
  imageFile?: File | null;
  isFeatured: boolean;
  categoryId?: string;
};

const buildAdminProductFormData = (payload: AdminProductPayload) => {
  const data = new FormData();
  data.append("name", payload.name);
  data.append("price", payload.price.toString());
  data.append("isFeatured", payload.isFeatured ? "true" : "false");
  if (payload.description) {
    data.append("description", payload.description);
  }
  if (payload.imageUrl) {
    data.append("imageUrl", payload.imageUrl);
  }
  if (payload.categoryId) {
    data.append("categoryId", payload.categoryId);
  }
  if (payload.imageFile) {
    data.append("imageFile", payload.imageFile);
  }
  return data;
};

export const catalogApi = {
  getProducts: async (params: {
    search?: string;
    categoryId?: string;
    featured?: boolean;
    minPrice?: number;
    maxPrice?: number;
    sort?: string;
    page?: number;
    pageSize?: number;
  }) => {
    const response = await apiClient.get(`${baseUrl}/api/products`, { params });
    return response.data as ProductListResponse;
  },
  getProduct: async (id: string) => {
    const response = await apiClient.get(`${baseUrl}/api/products/${id}`);
    return response.data as ProductDetails;
  },
  getCategories: async () => {
    const response = await apiClient.get(`${baseUrl}/api/categories`);
    return response.data as Category[];
  },
  getReviews: async (productId: string) => {
    const response = await apiClient.get(`${baseUrl}/api/products/${productId}/reviews`);
    return response.data as Review[];
  },
  addReview: async (productId: string, payload: { rating: number; comment?: string }) => {
    const response = await apiClient.post(`${baseUrl}/api/products/${productId}/reviews`, payload);
    return response.data as Review;
  },
  getFavorites: async () => {
    const response = await apiClient.get(`${baseUrl}/api/favorites`);
    return response.data as ProductListResponse["items"];
  },
  addFavorite: async (productId: string) => {
    await apiClient.post(`${baseUrl}/api/favorites/${productId}`);
  },
  removeFavorite: async (productId: string) => {
    await apiClient.delete(`${baseUrl}/api/favorites/${productId}`);
  },
  adminGetProducts: async (params: { search?: string; categoryId?: string }) => {
    const response = await apiClient.get(`${baseUrl}/api/admin/products`, { params });
    return response.data as { id: string; name: string; description?: string; price: number; imageUrl?: string; isFeatured: boolean; categoryId?: string }[];
  },
  adminCreateProduct: async (payload: AdminProductPayload) => {
    const data = buildAdminProductFormData(payload);
    const response = await apiClient.post(`${baseUrl}/api/admin/products`, data);
    return response.data as { id: string; name: string; description?: string; price: number; imageUrl?: string; isFeatured: boolean; categoryId?: string };
  },
  adminUpdateProduct: async (id: string, payload: AdminProductPayload) => {
    const data = buildAdminProductFormData(payload);
    const response = await apiClient.put(`${baseUrl}/api/admin/products/${id}`, data);
    return response.data as { id: string; name: string; description?: string; price: number; imageUrl?: string; isFeatured: boolean; categoryId?: string };
  },
  adminDeleteProduct: async (id: string) => {
    await apiClient.delete(`${baseUrl}/api/admin/products/${id}`);
  },
  adminGetCategories: async () => {
    const response = await apiClient.get(`${baseUrl}/api/admin/categories`);
    return response.data as { id: string; name: string }[];
  },
  adminCreateCategory: async (payload: { name: string }) => {
    const response = await apiClient.post(`${baseUrl}/api/admin/categories`, payload);
    return response.data as { id: string; name: string };
  },
  adminUpdateCategory: async (id: string, payload: { name: string }) => {
    const response = await apiClient.put(`${baseUrl}/api/admin/categories/${id}`, payload);
    return response.data as { id: string; name: string };
  },
  adminDeleteCategory: async (id: string) => {
    await apiClient.delete(`${baseUrl}/api/admin/categories/${id}`);
  }
};
