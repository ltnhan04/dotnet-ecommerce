import { Card } from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";
import {
  CheckCircle2,
  XCircle,
  Copy,
  Loader2,
  Gift,
  Ticket,
} from "lucide-react";
import { IVoucherList } from "@/types/promotion";
import { formatCurrency } from "@/utils/format-currency";
import { toast } from "@/hooks/use-toast";

interface VoucherListProps {
  vouchers: IVoucherList[] | undefined;
  isLoading: boolean;
}

export function VoucherList({ vouchers, isLoading }: VoucherListProps) {
  const getVoucherStatus = (status: string) => {
    switch (status.toLowerCase()) {
      case "active":
        return {
          icon: <CheckCircle2 className="w-5 h-5 text-emerald-600" />,
          text: "Còn hiệu lực",
          border: "border-l-4 border-emerald-400",
          bg: "bg-gradient-to-r from-emerald-50 to-white",
          textColor: "text-emerald-700",
        };
      case "used":
        return {
          icon: <XCircle className="w-5 h-5 text-gray-400" />,
          text: "Đã sử dụng",
          border: "border-l-4 border-gray-300",
          bg: "bg-gradient-to-r from-gray-100 to-white",
          textColor: "text-gray-500",
        };
      case "expired":
        return {
          icon: <XCircle className="w-5 h-5 text-red-400" />,
          text: "Hết hạn",
          border: "border-l-4 border-red-300",
          bg: "bg-gradient-to-r from-red-50 to-white",
          textColor: "text-red-500",
        };
      default:
        return {
          icon: <Ticket className="w-5 h-5 text-blue-400" />,
          text: status,
          border: "border-l-4 border-blue-300",
          bg: "bg-gradient-to-r from-blue-50 to-white",
          textColor: "text-blue-600",
        };
    }
  };

  const copyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text);
    toast({
      title: "Đã sao chép!",
      description: "Mã voucher đã được sao chép",
    });
  };

  return (
    <Card className="p-8 bg-white rounded-2xl shadow-lg border-0">
      <div className="flex items-center justify-center gap-2 mb-6">
        <Ticket className="w-7 h-7 text-yellow-400" />
        <h2 className="text-2xl font-bold text-gray-900 tracking-tight">
          Voucher của bạn
        </h2>
      </div>
      <ScrollArea className="h-[400px] pr-2">
        {isLoading ? (
          <div className="flex flex-col justify-center items-center h-32">
            <Loader2 className="w-10 h-10 animate-spin text-yellow-400 mb-2" />
            <div className="text-gray-500 mt-2">Đang tải voucher...</div>
          </div>
        ) : !vouchers?.length ? (
          <div className="flex flex-col items-center justify-center py-16">
            <Gift className="w-16 h-16 text-yellow-200 mb-3" />
            <div className="text-gray-500 text-xl font-semibold mb-1">
              Bạn chưa có voucher nào
            </div>
            <div className="text-blue-500 text-base mt-1 font-medium">
              Hãy đổi điểm để nhận voucher đầu tiên!
            </div>
          </div>
        ) : (
          <div className="space-y-5">
            {vouchers.map((voucher) => {
              const status = getVoucherStatus(voucher.status);
              return (
                <div
                  key={voucher._id}
                  className={`flex justify-between items-center rounded-xl p-5 ${status.bg} ${status.border} transition-shadow hover:shadow-xl`}
                >
                  <div className="flex flex-col gap-1">
                    <div className="flex items-center gap-2 mb-1">
                      <span className="font-bold text-lg text-gray-900">
                        Mã: {voucher.code}
                      </span>
                      <button
                        onClick={() => copyToClipboard(voucher.code)}
                        className="p-1 hover:bg-yellow-100 rounded-full border border-yellow-200 ml-1 transition"
                        title="Sao chép mã"
                      >
                        <Copy className="w-5 h-5 text-yellow-500" />
                      </button>
                      {status.icon}
                    </div>
                    <div className="text-xs text-gray-500">
                      Điểm đã dùng:{" "}
                      <span className="font-semibold text-gray-900">
                        {voucher.pointsUsed}
                      </span>
                    </div>
                  </div>
                  <div className="text-right min-w-[140px]">
                    <div className="font-bold text-gray-900 text-base">
                      {formatCurrency(voucher.discountAmount)} giảm giá
                    </div>
                    <div className="text-xs text-gray-500">
                      Hiệu lực:{" "}
                      {new Date(voucher.validFrom).toLocaleDateString()} -{" "}
                      {new Date(voucher.validTo).toLocaleDateString()}
                    </div>
                    <div
                      className={`text-xs mt-1 font-semibold ${status.textColor}`}
                    >
                      {status.text == "unused" ? "Chưa sử dụng" : "Đã sử dụng"}
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </ScrollArea>
    </Card>
  );
}
