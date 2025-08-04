import { axiosInstance } from "@/config/axiosInstance";
import {
  IResponsePoints,
  IResponseExchangeVoucher,
  IResponseVoucherList,
} from "@/types/promotion";

export const retrievePoints = async (): Promise<IResponsePoints> => {
  const res = await axiosInstance.get("/v1/points/");
  return res.data;
};

export const exchangeVoucher = async (
  pointsToUse: number
): Promise<IResponseExchangeVoucher> => {
  const res = await axiosInstance.post("/v1/points/exchange-voucher", {
    pointsToUse,
  });
  return res.data;
};

export const getVouchers = async (): Promise<IResponseVoucherList> => {
  const res = await axiosInstance.get("/v1/points/vouchers");
  return res.data;
};

export const applyVoucher = async (voucherCode: string, orderTotal: number) => {
  const res = await axiosInstance.post("/v1/points/apply-voucher", {
    voucherCode,
    orderTotal,
  });
  return res.data;
};

export const updateVoucherAsUsed = async (
  voucherCode: string,
  orderId: string
) => {
  const res = await axiosInstance.put("/v1/points/status-voucher", {
    voucherCode,
    orderId,
  });
  return res.data;
};
