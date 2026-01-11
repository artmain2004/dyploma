export type UserInfo = {
  userId: string;
  email: string;
  name?: string;
};

export type ProductListItem = {
  id: string;
  name: string;
  price: number;
  imageUrl?: string;
  categoryId?: string;
  isFeatured?: boolean;
  averageRating: number;
  reviewsCount: number;
};

export type ProductDetails = {
  id: string;
  name: string;
  description?: string;
  price: number;
  imageUrl?: string;
  categoryId?: string;
  isFeatured?: boolean;
  averageRating: number;
  reviewsCount: number;
};

export type Category = {
  id: string;
  name: string;
};

export type ProductListResponse = {
  items: ProductListItem[];
  page: number;
  pageSize: number;
  totalCount: number;
};

export type OrderSummary = {
  id: string;
  orderNumber: string;
  status: string;
  totalPrice: number;
  createdAtUtc: string;
};

export type OrderItem = {
  id: string;
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
};

export type OrderDetails = {
  id: string;
  orderNumber: string;
  status: string;
  totalPrice: number;
  promoCode?: string;
  discountAmount: number;
  createdAtUtc: string;
  customerEmail: string;
  shippingAddress?: string;
  items: OrderItem[];
};

export type OrderCreateResponse = {
  id: string;
  orderNumber: string;
  status: string;
  totalPrice: number;
  promoCode?: string;
  discountAmount: number;
  createdAtUtc: string;
};

export type Review = {
  id: string;
  rating: number;
  comment?: string;
  userName?: string;
  createdAtUtc: string;
};

export type CartItem = {
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  imageUrl?: string;
};
