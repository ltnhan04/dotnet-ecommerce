import { axiosInstance } from "@/config/axiosInstance";
import { IMomoCallback, PaymentType } from "@/types/payment";

export const createCheckoutSession = async ({
  variants,
  orderId,
}: PaymentType) => {
  return await axiosInstance.post("/v1/payment/create-checkout-session", {
    variants,
    orderId,
  });
};

export const updateOrderPayment = async ({
  stripeSessionId,
  orderId,
}: {
  stripeSessionId: string;
  orderId: string;
}) => {
  return await axiosInstance.post("/v1/orders/update-order-payment", {
    stripeSessionId,
    orderId,
  });
};

export const createMomoPayment = async (data: {
  orderId: string;
  amount: number;
  orderInfo: string;
}) => {
  const response = await axiosInstance.post(`/v1/payment/momo/create`, data);
  return response;
};

export const momoCallback = async (data: IMomoCallback) => {
  const response = await axiosInstance.post("/v1/payment/momo/callback", data);
  return response;
};
