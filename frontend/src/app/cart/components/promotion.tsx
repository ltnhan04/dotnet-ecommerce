import React, { useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  useGetVouchers,
} from "@/hooks/useExchangeVoucher";
import { IVoucherList } from "@/types/promotion";
import { formatCurrency } from "@/utils/format-currency";
import { formatDate } from '../../../utils/format-day';
import Image from "next/image";

interface PromotionProps {
  setVoucherCode: React.Dispatch<React.SetStateAction<string | null>>;
  setDiscounted: React.Dispatch<React.SetStateAction<number | null>>;
}

const PromotionSection: React.FC<PromotionProps> = ({
  setVoucherCode,
  setDiscounted
}) => {
  const [selected, setSelected] = useState<IVoucherList | null>(null);
  const { data: getVoucher } = useGetVouchers();
  return (
    <Card className="shadow-lg border-0 bg-white/90 backdrop-blur-md">
      <CardHeader className="pb-2 border-b">
        <CardTitle className="text-lg md:text-xl flex items-center gap-3 font-semibold text-gray-800">
          <span className="flex items-center justify-center w-8 h-8 rounded-full bg-primary text-primary-foreground font-bold shadow">
            4
          </span>
          Khuyến mãi
        </CardTitle>
      </CardHeader>
      <CardContent className="pt-4">
        {Array.isArray(getVoucher?.data) && getVoucher.data.length > 0 && (
          <div className="space-y-2">
            <h4 className="text-sm font-semibold text-gray-700">Mã khuyến mãi có sẵn:</h4>
            <ul className="space-y-4">
              {getVoucher?.data?.map((voucher: IVoucherList) => (
                <li
                  key={voucher._id}
                  onClick={() => {
                    setSelected(voucher)
                    setVoucherCode(voucher.code)
                    setDiscounted(voucher.discountAmount)
                  }}
                  className={`flex justify-between items-center border px-3 py-2 rounded-[16px] cursor-pointer transition ${selected?._id === voucher._id ? "ring-2 ring-primary" : "hover:ring-2 hover:ring-gray-700"} `}
                >
                  <span className="text-sm font-medium text-gray-800 flex items-center space-x-2">
                    <div>
                      <Image src={'/assets/images/voucher.png'} alt="voucher" width={52} height={52} />
                    </div>
                    <div>
                      {voucher.code}
                    </div>
                  </span>
                  <div>
                    <span className="text-green-500">Giảm giá: {formatCurrency(voucher.discountAmount)}</span>
                    <div>
                      <p>Đến: {formatDate(voucher.validTo)}</p>
                    </div>
                  </div>
                </li>
              ))}
            </ul>
          </div>
        )}

      </CardContent>
    </Card>
  );
};

export default PromotionSection;
