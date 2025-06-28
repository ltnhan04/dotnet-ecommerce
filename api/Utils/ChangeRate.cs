using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;

namespace api.Utils
{
    public static class ChangeRate
    {
        public const int vndLimit = 99999999;

        public static int priceInUSD(int price)
        {
            var exchangeRate = 26000;
            return price / exchangeRate;
        }
    }
}