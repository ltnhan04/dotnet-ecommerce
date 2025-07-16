import { Clock, Truck, PackageCheck, CheckCircle, XCircle } from "lucide-react";

interface OrderTimelineProps {
  status: "pending" | "processing" | "shipped" | "delivered" | "cancel";
}

const steps = [
  { step: "pending", label: "Chờ xử lý", icon: Clock },
  { step: "processing", label: "Đang xử lý", icon: Truck },
  { step: "shipped", label: "Đã gửi hàng", icon: PackageCheck },
  { step: "delivered", label: "Đã giao", icon: CheckCircle },
];

const OrderTimeline = ({ status }: OrderTimelineProps) => {
  const currentIndex = steps.findIndex((s) => s.step === status);
  console.log("Status gửi vào OrderTimeline:", status);
  return (
    <div className="relative py-8 px-2 sm:px-6">
      {status === "cancel" ? (
        <div className="flex flex-col items-center justify-center text-center text-red-600 space-y-2">
          <XCircle className="w-10 h-10 animate-pulse text-destructive" />
          <p className="text-lg font-semibold">Đơn hàng đã bị hủy</p>
        </div>
      ) : (
        <>
          <div className="absolute top-1/2 left-6 right-6 h-1 bg-gray-200 -translate-y-1/2 z-0 rounded" />

          <div className="relative z-10 flex justify-between items-center">
            {steps.map((step, index) => {
              const Icon = step.icon;
              const isCompleted = index < currentIndex;
              const isCurrent = index === currentIndex;

              return (
                <div
                  key={step.step}
                  className="flex flex-col items-center text-center w-20"
                >
                  <div
                    className={`w-12 h-12 rounded-full flex items-center justify-center border-4 mb-2 transition-all
                      ${
                        isCompleted
                          ? "bg-emerald-100 border-emerald-400 text-emerald-600"
                          : isCurrent
                          ? "bg-blue-100 border-blue-400 text-blue-600 animate-pulse"
                          : "bg-gray-50 border-gray-200 text-gray-300"
                      }
                    `}
                  >
                    <Icon className="w-6 h-6" />
                  </div>
                  <p
                    className={`text-xs font-medium ${
                      isCompleted
                        ? "text-emerald-600"
                        : isCurrent
                        ? "text-blue-600"
                        : "text-gray-400"
                    }`}
                  >
                    {step.label}
                  </p>
                </div>
              );
            })}
          </div>
        </>
      )}
    </div>
  );
};

export default OrderTimeline;
