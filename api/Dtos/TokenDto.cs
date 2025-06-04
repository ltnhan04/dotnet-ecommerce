using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class TokenDto
    {
        public string accessToken { get; set; } = string.Empty;
        public string refreshToken { get; set; } = string.Empty;
    }
}