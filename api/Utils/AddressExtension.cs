using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.models;

namespace api.Utils
{
    public static class AddressExtension
    {
        public static string ToFullAddress(this Address address) 
        {
            return $"{address.street}, {address.ward}, {address.district}, {address.city}, {address.country}";
        }
    }
}