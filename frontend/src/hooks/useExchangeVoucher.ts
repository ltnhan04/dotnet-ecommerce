import {
  retrievePoints,
  exchangeVoucher,
  getVouchers,
  applyVoucher,
  updateVoucherAsUsed,
} from "@/services/promotions/promotionApi";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const useRetrievePoints = () => {
  return useQuery({
    queryKey: ["points"],
    queryFn: retrievePoints,
  });
};

export const useExchangeVoucher = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (pointsToUse: number) => exchangeVoucher(pointsToUse),
    onSuccess: () => {
      // eslint-disable-next-line @typescript-eslint/no-unused-expressions
      queryClient.invalidateQueries({
        queryKey: ['vouchers']
      }),
      queryClient.invalidateQueries({
        queryKey: ['points']
      })
    }
  });
};

export const useGetVouchers = () => {
  return useQuery({
    queryKey: ["vouchers"],
    queryFn: getVouchers,
  });
};

export const useApplyVoucher = () => {
  return useMutation({
    mutationFn: ({
      voucherCode,
      orderTotal,
    }: {
      voucherCode: string;
      orderTotal: number;
    }) => applyVoucher(voucherCode, orderTotal),
  });
};

export const useUpdateVoucherAsUsed = () => {
  return useMutation({
    mutationFn: ({
      voucherId,
      orderId,
    }: {
      voucherId: string;
      orderId: string;
    }) => updateVoucherAsUsed(voucherId, orderId),
  });
};
