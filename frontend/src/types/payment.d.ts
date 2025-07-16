export interface PaymentType {
  variants: Variant[];
  orderId: string;
}

export interface ProductVariant {
  variant: string;
  quantity: number;
}

export interface IMomoCallback {
  partnerCode: string
  orderId: string
  requestId: string
  amount: number
  orderInfo: string
  orderType: string
  transId: number
  resultCode: number
  message: string
  payType: string
  responseTime: string
  extraData: string
  signature: string
}