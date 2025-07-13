export type Promotion = {
  _id: string;
  code: string;
  discountPercentage: number;
  validFrom: string;
  validTo: string;
  isActive: boolean;
  maxUsage: number;
  usedCount: number;
};

export interface UserAddress {
  street?: string;
  ward?: string;
  district?: string;
  city?: string;
  country: string;
}
export interface IShippingMethod {
  name: string,
  type: string, 
  fee: number
}

export interface ShippingMethodSectionProps {
  isLoadingMethods: boolean;
  shippingMethods:
    | {
        data: {
          methods: ShippingMethod[];
        }
      }
    | undefined;
  selectedShippingMethod: string;
  handleShippingMethodChange: (value: string) => void;
}
