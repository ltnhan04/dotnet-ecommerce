import { CheckCircle, Clock, Package } from "lucide-react";

interface OrderTimelineProps {
  status: string;
}

const OrderTimeline = ({ status }: OrderTimelineProps) => {
  const getTimelineStatus = (orderStatus: string, currentStep: string) => {
    const steps = ["pending", "processing", "delivered"];
    const currentIndex = steps.indexOf(orderStatus);
    const stepIndex = steps.indexOf(currentStep);

    if (stepIndex < currentIndex) {
      return "completed";
    } else if (stepIndex === currentIndex) {
      return "current";
    } else {
      return "upcoming";
    }
  };

  const getTimelineIcon = (status: string, step: string) => {
    const timelineStatus = getTimelineStatus(status, step);

    if (timelineStatus === "completed") {
      return <CheckCircle className="w-7 h-7 text-emerald-500 drop-shadow" />;
    } else if (timelineStatus === "current") {
      return (
        <Clock className="w-7 h-7 text-blue-500 animate-pulse drop-shadow-lg" />
      );
    } else {
      return <Package className="w-7 h-7 text-gray-300" />;
    }
  };

  const steps = [
    { step: "pending", label: "Chờ xử lý" },
    { step: "processing", label: "Đang giao" },
    { step: "delivered", label: "Đã giao" },
  ];

  return (
    <div className="relative py-6">
      {/* Animated line */}
      <div className="absolute top-1/2 left-8 right-8 h-1 bg-gradient-to-r from-yellow-200 via-blue-200 to-emerald-200 -translate-y-1/2 z-0" />
      <div className="relative flex justify-between z-10">
        {steps.map(({ step, label }) => {
          const timelineStatus = getTimelineStatus(status, step);
          const isCompleted = timelineStatus === "completed";
          const isCurrent = timelineStatus === "current";
          return (
            <div key={step} className="flex flex-col items-center flex-1">
              <div
                className={`w-16 h-16 rounded-full flex items-center justify-center mb-2 border-4 transition-all duration-300
                  ${isCompleted ? "bg-emerald-50 border-emerald-300" : ""}
                  ${
                    isCurrent
                      ? "bg-blue-50 border-blue-400 shadow-lg scale-110"
                      : ""
                  }
                  ${
                    !isCompleted && !isCurrent
                      ? "bg-gray-50 border-gray-200"
                      : ""
                  }
                `}
              >
                {getTimelineIcon(status, step)}
              </div>
              <div className="text-center">
                <p
                  className={`text-base font-bold mb-1 transition-colors
                    ${isCompleted ? "text-emerald-600" : ""}
                    ${isCurrent ? "text-blue-600" : ""}
                    ${!isCompleted && !isCurrent ? "text-gray-400" : ""}
                  `}
                >
                  {label}
                </p>
                {isCurrent && (
                  <p className="text-xs text-blue-500 mt-1 animate-pulse font-semibold">
                    Đang xử lý
                  </p>
                )}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default OrderTimeline;
