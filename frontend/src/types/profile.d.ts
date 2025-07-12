export interface ProfileType {
  _id: string;
  name: string;
  email: string;
  phoneNumber?: string;
  address?: UserAddress;
  password?: string;
  role: string;
  createdAt: string;
  updatedAt: string;
  __v: number;
}

export interface ProductVariant {
  productVariant: ProductVariant2;
  quantity: number;
  _id: string;
}

export interface ProductVariant2 {
  color: Color;
  _id: string;
  name: string;
  storage: string;
  price: number;
}

export interface Color {
  colorName: string;
  colorCode: string;
}
export interface UserAddress {
  street?: string;
  ward?: string;
  district?: string;
  city?: string;
  country: string;
}

export interface EditedProfile {
  isEdited: boolean;
  editedName: string;
  editedAddress: UserAddress;
  editedPhoneNumber: string;
}

export interface IProvinceType {
  name: string;
  code: number;
  districts: string[];
}

export interface IDistrictType {
  code: number;
  name: string;
  wards: string[];
}

export interface IWardType {
  code: number;
  name: string;
}
