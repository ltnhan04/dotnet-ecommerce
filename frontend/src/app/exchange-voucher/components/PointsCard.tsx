import { Card } from "@/components/ui/card";
import { Coins, Loader2 } from "lucide-react";

interface PointsCardProps {
  points: number | undefined;
  isLoading: boolean;
}

export function PointsCard({ points, isLoading }: PointsCardProps) {
  const current = points || 0;
  const required = 1000;
  const enough = current >= required;
  const missing = required - current;

  return (
    <Card className="p-8 mb-8 bg-white rounded-2xl shadow-lg shadow-[#e6e6e6] flex flex-col items-center">
      <div className="flex flex-col items-center mb-4 w-full">
        <Coins className="w-12 h-12 text-yellow-400 mb-3 drop-shadow-lg" />
        <div className="text-5xl font-extrabold text-gray-900 tracking-tight mb-1 drop-shadow-sm">
          {isLoading ? (
            <Loader2 className="w-7 h-7 animate-spin text-yellow-300" />
          ) : (
            current
          )}
        </div>
        <div className="text-lg text-gray-500 font-semibold">Điểm của bạn</div>
      </div>
      <div className="w-full mb-2">
        <div className="relative w-full h-4 rounded-full bg-[#e6e6e6] overflow-hidden">
          <div
            className="absolute left-0 top-0 h-4 rounded-full bg-gradient-to-r from-yellow-300 via-yellow-400 to-yellow-500 transition-all"
            style={{ width: `${(current / required) * 100}%` }}
          />
        </div>
      </div>
      <div
        className={`text-base mt-3 w-full text-center font-semibold ${
          enough ? "text-emerald-600" : "text-gray-600"
        }`}
      >
        {enough ? (
          "Bạn đã đủ điều kiện để đổi!"
        ) : (
          <>
            Cần thêm{" "}
            <span className="font-bold text-yellow-600">{missing}</span> điểm để
            đổi voucher tiếp theo
          </>
        )}
      </div>
    </Card>
  );
}
