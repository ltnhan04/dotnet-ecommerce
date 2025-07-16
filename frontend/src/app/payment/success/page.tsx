"use client";

import { useEffect, useState } from "react";
import { useSearchParams } from "next/navigation";
import Link from "next/link";
import {
  CheckCircle,
  Package,
  CreditCard,
  MapPin,
  Calendar,
  Clock4,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { useAppDispatch } from "@/lib/hooks";
import { toast } from "@/hooks/use-toast";
import { clearCart } from "@/lib/features/cart/cartSlice";
import {
  updateOrderPayment,
  momoCallback,
} from "@/services/payment/paymentApi";
import { ErrorType as ErrorResponse } from "@/types/common";
import { formatCurrency } from "@/utils/format-currency";
import { formatDate } from "@/utils/format-day";
import { OrderDetails } from "@/types/order";
import Image from "next/image";
import { updateVoucherAsUsed } from "@/services/promotions/promotionApi";

export default function SuccessPage() {
  const [order, setOrder] = useState<OrderDetails | null>(null);
  const dispatch = useAppDispatch();
  const searchParams = useSearchParams();

  //Stripe
  const stripeSessionId = searchParams.get("session_id") as string;

  const orderId = searchParams.get("orderId") as string;
  const voucherCode = searchParams.get("voucherCode") as string;

  //Momo
  const voucherMomoCode = searchParams.get("extraData") as string;
  const partnerCode = searchParams.get("partnerCode") as string;
  const requestId = searchParams.get("requestId") as string;
  const amount = parseFloat(searchParams.get("amount")!);
  const orderInfo = searchParams.get("orderInfo") as string;
  const orderType = searchParams.get("orderType") as string;
  const transId = parseFloat(searchParams.get("transId")!);
  const resultCode = parseFloat(searchParams.get("resultCode")!);
  const message = searchParams.get("message") as string;
  const payType = searchParams.get("payType") as string;
  const responseTime = searchParams.get("responseTime") as string;
  const extraData = searchParams.get("extraData") ?? "";
  const signature = searchParams.get("signature") as string;

  useEffect(() => {
    const paidKey = `orderPaid-${orderId}`;
    const voucherKey = voucherCode ? `voucherUsed-${voucherCode}` : null;
    const voucherMomoKey = voucherMomoCode ? `voucherUsed-${voucherMomoCode}` : null;

    const updatePayment = async () => {
      try {
        let response;
        let update;
        if (partnerCode) {
          response = await momoCallback({ partnerCode, orderId, requestId, amount, orderInfo, orderType, transId, resultCode, message, payType, responseTime, extraData, signature });
          if (voucherMomoCode && orderId && !localStorage.getItem(voucherMomoKey!)) {
            update = await updateVoucherAsUsed(voucherMomoCode, orderId);
          }
        }
        console.log(response)
        if (stripeSessionId) {
          response = await updateOrderPayment({ stripeSessionId, orderId });
          if (voucherCode && orderId && !localStorage.getItem(voucherKey!)) {
            update = await updateVoucherAsUsed(voucherCode, orderId);
          }
        }
        if (response?.status === 200) {
          toast({
            title: "Thành công",
            description: response.data.message,
            variant: "default",
          });
          setOrder(response.data.data);
          dispatch(clearCart());
          localStorage.setItem(paidKey, "true");
        }
        if (update?.status === 200) {
          localStorage.setItem(voucherMomoKey!, "true");
          localStorage.setItem(voucherKey!, "true");
        }
      } catch (error: unknown) {
        console.log(error)
        toast({
          title: "Lỗi",
          description: (error as ErrorResponse).response.data.message,
          variant: "destructive",
        });
      }
    };

    updatePayment();
  }, [amount, dispatch, extraData, message, orderId, orderInfo, orderType, partnerCode, payType, requestId, responseTime, resultCode, signature, stripeSessionId, transId, voucherCode, voucherMomoCode]);

  return (
    <div className="min-h-screen flex items-center justify-center">
      <Card className="w-full max-w-2xl shadow-xl">
        <CardHeader className="space-y-4">
          <div className="flex items-center justify-center">
            <div className="bg-green-100 rounded-full p-3">
              <CheckCircle className="h-8 w-8 md:h-12 md:w-12 text-green-500" />
            </div>
          </div>
          <CardTitle className="text-xl md:text-3xl font-bold text-center text-gray-800">
            Thanh toán thành công!
          </CardTitle>
          <CardDescription className="text-center text-sm md:text-base text-gray-600">
            Cảm ơn bạn đã mua hàng. Thanh toán của bạn đã được xử lý thành công.
          </CardDescription>
        </CardHeader>
        <CardContent className=" space-y-4 md:space-y-6">
          {order && (
            <>
              <div className="bg-white p-4 md:p-6 rounded-lg shadow-xl">
                <h3 className="font-semibold text-base md:text-lg mb-4 text-gray-800">
                  Thông tin đơn hàng
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-2 md:gap-4">
                  <div className="flex items-center space-x-3 bg-gray-50 p-3 rounded-lg">
                    <Package className="h-5 w-5 text-gray-600" />
                    <div className="flex flex-col">
                      <span className="text-sm text-gray-500">Mã đơn hàng</span>
                      <span className="font-medium text-gray-800">
                        {order._id}
                      </span>
                    </div>
                  </div>
                  <div className="flex items-center space-x-3 bg-gray-50 p-3 rounded-lg">
                    <CreditCard className="h-5 w-5 text-gray-600" />
                    <div className="flex flex-col">
                      <span className="text-sm text-gray-500">Tổng tiền</span>
                      <span className="font-medium text-gray-800">
                        {formatCurrency(order.totalAmount)}
                      </span>
                    </div>
                  </div>
                  <div className="flex items-center space-x-3 bg-gray-50 p-3 rounded-lg">
                    <Clock4 className="h-5 w-5 text-gray-600" />
                    <div className="flex flex-col">
                      <span className="text-sm text-gray-500">Trạng thái</span>
                      <span className="font-medium text-gray-800 capitalize">
                        {order.status}
                      </span>
                    </div>
                  </div>
                  <div className="flex items-center space-x-3 bg-gray-50 p-3 rounded-lg">
                    <Calendar className="h-5 w-5 text-gray-600" />
                    <div className="flex flex-col">
                      <span className="text-sm text-gray-500">
                        Ngày giao dịch
                      </span>
                      <span className="font-medium text-gray-800">
                        {formatDate(order.createdAt)}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
              <div className="bg-white p-4 md:p-6 rounded-lg shadow-xl">
                <h3 className="font-semibold text-base md:text-lg mb-4 text-gray-800">
                  Chi tiết đơn hàng
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-1 md:gap-4">
                  {order.variants.map((item, index) => (
                    <div
                      key={index}
                      className="flex justify-between items-center space-x-4 bg-gray-50 p-3 rounded-lg"
                    >
                      <div className="flex items-center space-x-2">
                        <div className="w-16 h-16 rounded-lg overflow-hidden flex-shrink-0">
                          <Image
                            src={item.variant.images[0]}
                            alt={item.variant.productName}
                            width={100}
                            height={100}
                            className="w-full h-full object-cover"
                          />
                        </div>
                        <div className="flex flex-col justify-between">
                          <span className="font-medium text-gray-800">
                            {item.variant.productName} {item.variant.colorName} {item.variant.storage}
                          </span>
                          <span className="text-sm text-gray-500">
                            Số lượng: {item.quantity}
                          </span>
                          <span className="text-sm text-gray-600">
                            Giá: {formatCurrency(item.variant.price)}
                          </span>
                        </div>
                      </div>
                      <div className="ml-auto font-semibold text-gray-800">
                        {formatCurrency(item.variant.price * item.quantity)}
                      </div>
                    </div>
                  ))}
                </div>
              </div>
              <div className="bg-white p-4 md:p-6 rounded-lg shadow-xl">
                <h3 className="font-semibold text-base md:text-lg mb-4 text-gray-800">
                  Thông tin giao hàng
                </h3>
                <div className="space-y-4">
                  <div className="flex items-start space-x-3 bg-gray-50 p-3 rounded-lg">
                    <MapPin className="h-5 w-5 flex-shrink-0 text-gray-600 mt-0.5" />
                    <div className="flex flex-col">
                      <span className="text-sm text-gray-500">
                        Địa chỉ giao hàng
                      </span>
                      <span className="text-gray-800">
                        {order.shippingAddress}
                      </span>
                    </div>
                  </div>
                  <div className="flex items-start space-x-3 bg-gray-50 p-3 rounded-lg">
                    {
                      order.status == "processing" ? (<Image src={"/assets/images/stripe.png"} alt="stripe" width={32} height={32} className="rounded-full" />) : (<Image src={"/assets/images/momo.png"} alt="momo" width={32} height={32} className="rounded-full" />)
                    }
                    <div className="flex flex-col">
                      <span className="text-sm text-gray-500">
                        Phương thức thanh toán
                      </span>
                      <span className="text-gray-800 capitalize">
                        {order.paymentMethod === "stripe"
                          ? "Stripe"
                          : order.paymentMethod === "momo"
                            ? "MoMo"
                            : "Thanh toán khi nhận hàng"}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </>
          )}
          <Separator className="my-6" />
        </CardContent>
        <CardFooter className="flex flex-col sm:flex-row justify-center gap-4 pb-6">
          <Button className="w-full sm:w-auto" asChild>
            <Link href="/">Về trang chủ</Link>
          </Button>
          <Button className="w-full sm:w-auto" variant="outline" asChild>
            <Link href="/orders">Theo dõi đơn hàng</Link>
          </Button>
        </CardFooter>
      </Card>
    </div>
  );
}
