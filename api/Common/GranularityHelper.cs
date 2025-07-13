using api.Enums; 

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
            else if (duration.TotalDays <= 60) // About 2 months
            {
                return Granularity.Daily;
            }
            else if (duration.TotalDays <= 180) // About 6 months
            {
                return Granularity.Weekly;
            }
            else if (duration.TotalDays <= 730) // About 2 years
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