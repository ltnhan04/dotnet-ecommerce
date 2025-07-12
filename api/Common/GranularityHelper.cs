// api.Common/GranularityHelper.cs
using api.Enums; // Đảm bảo bạn đã tạo Enums/Granularity.cs

namespace api.Common
{
    public interface IGranularityHelper
    {
        Granularity GetRecommendedGranularity(DateTime fromDate, DateTime toDate);
    }

    public class GranularityHelper : IGranularityHelper
    {
        public Granularity GetRecommendedGranularity(DateTime fromDate, DateTime toDate)
        {
            TimeSpan duration = toDate - fromDate;

            if (duration.TotalDays <= 2)
            {
                return Granularity.Hourly;
            }
            else if (duration.TotalDays <= 60) // Khoảng 2 tháng
            {
                return Granularity.Daily;
            }
            else if (duration.TotalDays <= 180) // Khoảng 6 tháng
            {
                return Granularity.Weekly;
            }
            else if (duration.TotalDays <= 730) // Khoảng 2 năm
            {
                return Granularity.Monthly;
            }
            else
            {
                return Granularity.Yearly;
            }
        }
    }
}