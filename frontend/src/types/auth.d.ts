export interface SignUpState {
  email: string;
  message: string;
}

export interface LoginState {
  message: string;
  accessToken: string;
  name: string;
}
export interface VerifySignUpState {
  message: string;
  accessToken: string;
  name: string;
}
export interface SignUpType {
  email: string;
  name: string;
  password: string;
}

export interface LoginType {
  email: string;
  password: string;
}

export interface VerifySignUpType {
  email: string;
  otp: string;
}

export interface IResponseProfileType {
  message: string;
  data: ProfileType;
}

export interface ProfileType {
  _id: string;
  name: string;
  email: string;
  phoneNumber?: string;
  address?: {
    street?: string;
    ward?: string;
    district?: string;
    city?: string;
    country: string;
  };
  role: string;
  createdAt: string;
  updatedAt: string;
  __v: number;
}
