/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-unused-vars */
"use client";

import { useState } from "react";
import {
  useExchangeVoucher,
  useRetrievePoints,
  useGetVouchers,
} from "@/hooks/useExchangeVoucher";
import { PointsCard } from "./components/PointsCard";
import { ExchangeCard } from "./components/ExchangeCard";
import { VoucherList } from "./components/VoucherList";
import Breadcrumb from "@/components/common/breadcrumb";
import { toast } from "@/hooks/use-toast";
import { Info } from "lucide-react";

function RulesCard() {
  return (
    <div className="bg-blue-50 border border-blue-200 rounded-xl p-5 mt-2 shadow-sm">
      <div className="flex items-center mb-2">
        <Info className="w-5 h-5 text-blue-600 mr-2" />
        <span className="font-semibold text-blue-700">Điều lệ đổi điểm</span>
      </div>
      <ul className="list-disc pl-6 text-sm text-gray-700 space-y-1">
        <li>
          Mỗi lần đổi tối thiểu{" "}
          <span className="font-semibold text-gray-900">100 điểm</span>
        </li>
        <li>
          <span className="font-semibold text-gray-900">1000 điểm</span> = 1
          voucher
        </li>
        <li>Voucher không hoàn lại điểm</li>
        <li>
          Voucher có hạn sử dụng{" "}
          <span className="font-semibold text-gray-900">30 ngày</span> kể từ
          ngày đổi
        </li>
      </ul>
    </div>
  );
}

export default function ExchangeVoucherPage() {
  const [pointToUse, setPointToUse] = useState<string>("");
  const { data: pointsData, isLoading: isLoadingPoints } = useRetrievePoints();
  const { data: vouchersData, isLoading: isLoadingVouchers } = useGetVouchers();
  const { mutate: exchangeVoucher, isPending: isExchanging } =
    useExchangeVoucher();

  const handleExchange = async () => {
    const points = parseInt(pointToUse);
    if (points <= 0) {
      toast({
        title: "Lỗi",
        description: "Vui lòng nhập số điểm hợp lệ",
        variant: "destructive",
      });
      return;
    }

    if (points > (pointsData?.data?.points || 0)) {
      toast({
        title: "Không đủ điểm",
        description: "Bạn không có đủ điểm để đổi voucher",
        variant: "destructive",
      });
      return;
    }

    exchangeVoucher(points, {
      onSuccess: () => {
        toast({
          title: "Thành công",
          description: "Đổi voucher thành công",
        });
        setPointToUse("");
      },
      onError: (error: any) => {
        toast({
          title: "Lỗi",
          description:
            error?.response?.data?.message ||
            "Không thể đổi voucher. Vui lòng thử lại.",
          variant: "destructive",
        });
      },
    });
  };
  console.log(vouchersData?.data)
  return (
    <div className="min-h-screen sm:px-6 lg:px-8">
      <div className="max-w-7xl mx-auto">
        <Breadcrumb />
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          <div className="space-y-6">
            <PointsCard
              points={pointsData?.data?.points || 0}
              isLoading={isLoadingPoints}
            />
            <ExchangeCard
              pointToUse={pointToUse}
              setPointToUse={setPointToUse}
              onExchange={handleExchange}
              isExchanging={isExchanging}
              maxPoints={pointsData?.data?.points || 0}
            />
            <RulesCard />
          </div>
          <VoucherList
            vouchers={vouchersData?.data ?? []}
            isLoading={isLoadingVouchers}
          />
        </div>
      </div>
    </div>
  );
}
