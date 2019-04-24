using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataTransferObject
{
    public class JWTSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
