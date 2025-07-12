using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Utils
{
    public static class GetNotificationForOrderStatus
    {
        public static (string userTitle, string userMessage, string adminTitle, string adminMessage) GetNotifications(string status, string orderId)
        {
            string userTitle = "";
            string userMessage = "";
            string adminTitle = "";
            string adminMessage = "";

            switch (status.ToLower())
            {

                case "processing":
                    userTitle = "✅ Đơn hàng đã được xác nhận";
                    userMessage = $"Đơn hàng #{orderId} đã được xác nhận và đang chuẩn bị giao.";
                    adminTitle = "🔄 Đơn hàng đã được xác nhận";
                    adminMessage = $"Bạn đã xác nhận đơn hàng #{orderId}.";
                    break;

                case
                    "shipped":
                    userTitle = "🚚 Đơn hàng đang được giao";
                    userMessage = $"Đơn hàng #{orderId} của bạn đang trên đường giao đến.";
                    adminTitle = "🚚 Đã bàn giao đơn hàng cho vận chuyển";
                    adminMessage = $"Đơn hàng #{orderId} đã được giao cho đơn vị vận chuyển.";
                    break;

                case
                    "delivered":
                    userTitle = "🎉 Đơn hàng đã giao thành công";
                    userMessage = $"Đơn hàng #{orderId} đã được giao thành công. Cảm ơn bạn đã mua hàng!";
                    adminTitle = "✅ Đơn hàng đã hoàn tất";
                    adminMessage = $"Đơn hàng #{orderId} đã được giao đến khách.";
                    break;

                case
                    "cancel":
                    userTitle = "❌ Đơn hàng đã bị hủy";
                    userMessage = $"Đơn hàng #{orderId} của bạn đã bị hủy.";
                    adminTitle = "⚠️ Khách hủy đơn hàng";
                    adminMessage = $"Khách hàng đã hủy đơn hàng #{orderId}.";
                    break;

                default:
                    userTitle = "🔄 Đơn hàng cập nhật trạng thái";
                    userMessage = $"Đơn hàng #{orderId} đã được cập nhật trạng thái: {status}.";
                    adminTitle = "🔄 Cập nhật đơn hàng";
                    adminMessage = $"Đơn hàng #{orderId} chuyển trạng thái: {status}.";
                    break;
            }
            return (userTitle, userMessage, adminTitle, adminMessage);

        }
    }
}