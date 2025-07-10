import { ProductVariant } from "./profile";

export interface Color {
  colorName: string;
  colorCode: string;
}

export interface Review {
  _id: string;
  variant: string;
  rating: number;
  comment: string;
}

export interface User {
  _id: string;
  name: string;
  email: string;
}

export interface ProductResponse {
  _id: string;
  name: string;
  description: string;
  category: {
    _id: string;
    name: string;
    parent_category: string;
  };
  variants: ProductVariant[];
  createdAt: string;
  updatedAt: string;
}
