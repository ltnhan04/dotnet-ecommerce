/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/no-explicit-any */
"use client";
import { useState, useEffect } from "react";
import withAuth from "@/components/common/withAuth";
import { useToast } from "@/hooks/use-toast";
import { useAppSelector } from "@/lib/hooks";
import { useOrders } from "@/hooks/useOrders";
import {
  createCheckoutSession,
  createMomoPayment,
} from "@/services/payment/paymentApi";
import { useShippingFee } from "@/hooks/useShippingMethod";
import PromotionSection from "@/app/cart/components/promotion";
import AddressSection from "@/app/cart/components/address";
import PaymentMethodSection from "@/app/cart/components/payment-method";
import { useProfile } from "@/hooks/useProfile";
import OrderSummary from "./components/OrderSummary";
import ShippingMethodSection from "./components/ShippingMethodSection";
import Breadcrumb from "@/components/common/breadcrumb";
import { IShippingMethod } from "@/types/checkout";

const CheckoutPage = () => {
  const { total, cart } = useAppSelector((state) => state.cart);
  const [showConfirmDialog, setShowConfirmDialog] = useState(false);
  const [selectedShippingMethod, setSelectedShippingMethod] = useState("");
  const [voucherCode, setVoucherCode] = useState<string | null>(null);
  const [discountAmount, setDiscountAmount] = useState<number | null>(null);
  const [checkoutData, setCheckoutData] = useState<{
    variants: { variant: string; quantity: number }[];
    totalAmount: number;
    shippingAddress: string;
    paymentMethod: string;
    shippingMethod?: string;
    voucherCode?: string | null
  }>({
    variants: cart.map((item) => ({
      variant: item.id,
      quantity: item.quantity,
    })),
    totalAmount: total,
    shippingAddress: "",
    paymentMethod: "",
    voucherCode: ""
  });

  const { toast } = useToast();
  const { createOrder, isLoading: isCreatingOrder } = useOrders();
  const { profile } = useProfile();
  const address = `${profile?.address?.street}, ${profile?.address?.ward}, ${profile?.address?.district}, ${profile?.address?.city}`;
  const { data: shippingFee, isLoading: isLoadingShippingFee } =
    useShippingFee(address);
  const handleShippingMethodChange = (value: string) => {
    setSelectedShippingMethod(value);
    setCheckoutData((prev) => ({
      ...prev,
      shippingMethod: value,
      totalAmount: total,
    }));
  };
  useEffect(() => {
    if (profile?.address) {
      const fullAddress = `${profile.address.street}, ${profile.address.ward}, ${profile.address.district}, ${profile.address.city}`;
      setCheckoutData((prev) => ({
        ...prev,
        shippingAddress: fullAddress,
      }));
    }
  }, [profile]);
  const selectedShippingMethodDetail: IShippingMethod =
    shippingFee?.data?.methods?.find(
      (item: IShippingMethod) => item.name == selectedShippingMethod
    );
  const shippingCost = selectedShippingMethodDetail?.fee || 0;
  const finalTotal = total + shippingCost - discountAmount!;
  const handleConfirmOrder = async () => {
    try {
      if (!checkoutData.shippingAddress) {
        toast({
          title: "Cần thêm địa chỉ giao hàng!",
          description: "Vui lòng cập nhật địa chỉ giao hàng để tiếp tục.",
          variant: "destructive",
        });
        return;
      }
      if (!checkoutData.paymentMethod) {
        toast({
          title: "Cần chọn phương thức thanh toán!",
          description: "Vui lòng chọn một phương thức thanh toán để hoàn tất.",
          variant: "destructive",
        });
        return;
      }
      if (!checkoutData.shippingMethod) {
        toast({
          title: "Cần chọn phương thức vận chuyển!",
          description: "Vui lòng chọn một phương thức vận chuyển để tiếp tục.",
          variant: "destructive",
        });
        return;
      }
      checkoutData.voucherCode = voucherCode ? voucherCode : null;
      const response = await createOrder({
        ...checkoutData,
        totalAmount: finalTotal
      });
      if (response.status === 201) {
        const { variants } = checkoutData;
        const orderId = response.data.data._id;
        if (checkoutData.paymentMethod === "stripe") {
          const checkoutSession = await createCheckoutSession({
            variants,
            orderId,
          });
          if (checkoutSession.status === 201) {
            window.location.href = checkoutSession.data.data.url;
          }
        } else if (checkoutData.paymentMethod === "momo") {
          const momoResponse = await createMomoPayment({
            orderId,
            amount: finalTotal,
            orderInfo: `Thanh toán đơn hàng ${orderId}`,
          });
          console.log(momoResponse)
          if (momoResponse.status === 201) {
            window.location.href = momoResponse.data.data.url;
          }
        } else if (checkoutData.paymentMethod === "cash on delivery") {
          window.location.href = `/checkout/success?orderId=${orderId}`;
        }

        toast({
          title: "Đã đặt hàng thành công",
          description: response.data.message,
          variant: "default",
        });
      }
    } catch (error: any) {
      console.log(error)
      toast({
        title: "Đã xảy ra lỗi",
        description: "Có lỗi trong quá trình đặt hàng. Vui lòng thử lại.",
        variant: "destructive",
      });
    } finally {
      setShowConfirmDialog(false);
    }
  };

  return (
    <div className="container mx-auto px-4 md:px-6 max-w-7xl">
      <Breadcrumb />
      <h1 className="text-3xl md:text-4xl font-semibold mb-8 text-center">
        Thanh Toán
      </h1>

      <div className="grid gap-8 md:gap-12 md:grid-cols-12">
        <div className="md:col-span-7 space-y-8">
          <AddressSection setCheckoutData={setCheckoutData} />

          <ShippingMethodSection
            isLoadingMethods={isLoadingShippingFee}
            shippingMethods={shippingFee}
            selectedShippingMethod={selectedShippingMethod}
            handleShippingMethodChange={handleShippingMethodChange}
          />

          <PaymentMethodSection
            checkoutData={checkoutData}
            setCheckoutData={setCheckoutData}
          />

          <PromotionSection
            setVoucherCode={setVoucherCode}
            setDiscounted={setDiscountAmount}
          />
        </div>
        <div className="md:col-span-5 space-y-8">
          <OrderSummary
            cart={cart}
            checkoutData={checkoutData}
            selectedMethod={selectedShippingMethodDetail}
            shippingCost={shippingCost}
            finalTotal={finalTotal}
            isCreatingOrder={isCreatingOrder}
            showConfirmDialog={showConfirmDialog}
            setShowConfirmDialog={setShowConfirmDialog}
            handleConfirmOrder={handleConfirmOrder}
            discountAmount={discountAmount}
          />
        </div>
      </div>
    </div>
  );
};

export default withAuth(CheckoutPage);
