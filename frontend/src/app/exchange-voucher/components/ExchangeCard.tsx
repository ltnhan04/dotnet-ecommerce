"use client";

import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Gift, Loader2 } from "lucide-react";
import { useState } from "react";

const POINT_SUGGESTIONS = [
  { points: 100, value: "100.000 VND" },
  { points: 300, value: "300.000 VND" },
  { points: 500, value: "500.000 VND" },
];

interface ExchangeCardProps {
  pointToUse: string;
  setPointToUse: (value: string) => void;
  onExchange: () => void;
  isExchanging: boolean;
  maxPoints: number;
}

export function ExchangeCard({
  pointToUse,
  setPointToUse,
  onExchange,
  isExchanging,
  maxPoints,
}: ExchangeCardProps) {
  const [error, setError] = useState<string>("");

  const handlePointChange = (value: string) => {
    setError("");
    const points = parseInt(value);

    if (isNaN(points)) {
      setError("Vui lòng nhập số điểm hợp lệ");
    } else if (points <= 0) {
      setError("Số điểm phải lớn hơn 0");
    } else if (points > maxPoints) {
      setError(`Bạn chỉ có ${maxPoints} điểm`);
    }

    setPointToUse(value);
  };

  return (
    <Card className="p-8 mb-8 bg-white rounded-2xl shadow-lg shadow-[#e6e6e6]">
      <h2 className="text-2xl font-bold mb-7 text-gray-900 text-center flex items-center justify-center gap-2">
        <Gift className="w-6 h-6 text-yellow-400" /> Đổi điểm lấy voucher
      </h2>
      <div className="space-y-5">
        <div>
          <div className="flex gap-4">
            <Input
              type="number"
              placeholder="Nhập số điểm muốn đổi"
              value={pointToUse}
              onChange={(e) => handlePointChange(e.target.value)}
              className={`flex-1 rounded-md shadow-[#e6e6e6] shadow-md px-5 py-3 text-lg placeholder:text-gray-400 focus:ring-2 focus:ring-yellow-400 focus:border-yellow-400 transition ${
                error ? "border-red-400 focus:ring-red-400" : ""
              }`}
            />
            <Button
              onClick={onExchange}
              disabled={isExchanging || !!error}
              className="flex items-center gap-2 rounded-xl bg-yellow-400 hover:bg-yellow-300 text-gray-900 font-bold px-8 py-3 shadow-lg transition disabled:bg-gray-200 disabled:text-gray-400"
            >
              {isExchanging ? (
                <Loader2 className="w-6 h-6 animate-spin" />
              ) : (
                <Gift className="w-6 h-6" />
              )}
              Đổi ngay
            </Button>
          </div>
          {error && (
            <p className="text-base text-red-500 mt-3 font-semibold">{error}</p>
          )}
        </div>
        <div className="flex gap-4 justify-center mt-2">
          {POINT_SUGGESTIONS.map((suggestion) => {
            const isActive = pointToUse === suggestion.points.toString();
            const isDisabled = suggestion.points > maxPoints;
            return (
              <button
                key={suggestion.points}
                type="button"
                onClick={() => handlePointChange(suggestion.points.toString())}
                disabled={isDisabled}
                className={`
                  px-6 py-2 rounded-full border-2
                  text-base font-bold transition
                  ${
                    isActive
                      ? "bg-yellow-400 text-gray-900 border-yellow-400 shadow-lg"
                      : "bg-[#e6e6e6] text-gray-400 border-yellow-100 hover:bg-yellow-100 hover:text-yellow-600"
                  }
                  ${isDisabled ? "opacity-50 cursor-not-allowed" : ""}
                `}
              >
                {suggestion.points}đ
              </button>
            );
          })}
        </div>
      </div>
    </Card>
  );
}
