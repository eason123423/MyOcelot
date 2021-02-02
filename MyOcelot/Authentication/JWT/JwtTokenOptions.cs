using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyOcelot.Authentication.JWT
{
    public class JwtTokenOptions
    {
        public string Issuer { get; set; }
        public string ValidateIssuer { get; set; }
        public string Audience { get; set; }
        public string ValidateAudience { get; set; }
        public string ValidateIssuerSigningKey { get; set; }
        public string ValidateLifetime { get; set; }
        public string RequireExpirationTime { get; set; }
        public string JwtExpiresInMinutes { get; set; }
        public string ValidateIntervaltime { get; set; }
        public string IntervalExpiresInMinutes { get; set; }
        public string SigningKey { get; set; }
    }
}
