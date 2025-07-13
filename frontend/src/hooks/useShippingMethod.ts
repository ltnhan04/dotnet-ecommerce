import { shippingFee } from "@/services/shipping-method/shippingApi";
import { useQuery } from "@tanstack/react-query";


export const useShippingFee = (shippingAddress: string) => {
  return useQuery({
    queryKey: ["shipping-fee", shippingAddress],
    queryFn: () => shippingFee(shippingAddress, 1, 0, 0, 0),
    enabled: !!shippingAddress
  });
};
