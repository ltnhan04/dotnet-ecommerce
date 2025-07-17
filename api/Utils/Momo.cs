using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace api.Utils
{
    public class Momo
    {
        public static string CreateSignature(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLower();
        }

        public static long RefundByPayType(string payType)
        {
            var refundLimit = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
            {
                {"momo_wallet", 30_000_000},
                {"qr", 30_000_000},
                {"napas", 50_000_000},
                {"atm", 50_000_000},
                {"visa", 50_000_000},
                {"master", 50_000_000},
                {"jcb", 50_000_000},
            };
            return refundLimit.GetValueOrDefault(payType, 50_000_000);
        }
    }
}