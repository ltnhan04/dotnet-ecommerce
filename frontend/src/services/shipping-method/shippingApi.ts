import { axiosInstance } from "@/config/axiosInstance";

export const shippingFee = async (
  shippingAddress: string,
  weight: number,
  height: number,
  length: number,
  width: number
) => {
  const res = await axiosInstance.post(`/v1/shipping-methods/calculate-fee`, {
    shippingAddress,
    weight,
    height,
    length,
    width,
  });
  return res.data;
};
