export interface OrderType {
  variants: VariantOrder[];
  totalAmount: number;
  shippingAddress: string;
  paymentMethod: string | "cash on delivery";
}

export interface Orders {
  message: string;
  data: Order[];
}

export interface Order {
  _id: string;
  user: string;
  variants: Variant[];
  totalAmount: number;
  status: string;
  shippingAddress: string;
  paymentMethod: string;
  stripeSessionId?: string;
  isPaymentMomo: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface User {
  _id: string;
  name: string;
}

export interface VariantOrder {
  variant: string;
  quantity: number;
}
export interface Variant {
  variant: Variant2;
  quantity: number;
}

export interface Variant2 {
  _id: string;
  product: string;
  productName: string;
  colorName: string;
  colorCode: string;
  stock_quantity: number;
  storage: string;
  price: number;
  images: string[];
}

export interface Color {
  colorName: string;
  colorCode: string;
}

export interface OrderDetails {
  _id: string;
  totalAmount: number;
  status: string;
  createdAt: string;
  paymentMethod: string;
  shippingAddress: string;
  variants: {
    variant: {
      _id: string;
      productName: string;
      price: number;
      images: string;
      colorName: string;
      storage: number;
    };
    quantity: number;
  }[];
}
