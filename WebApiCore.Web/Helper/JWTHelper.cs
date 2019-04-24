using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApiCore.DataTransferObject;

namespace WebApiCore.Web.Helper
{
    public static class JWTHelper
    {
        public static string GetIdToken(JWTSettings _options, Credentials credential)
        {
            var payload = new Dictionary<string, object>
              {
                { "id", credential.Id },
                { "sub", credential.Username },
                { "email", credential.Email }
              };
            return GetToken(_options, payload);
        }

        public static string GetAccessToken(JWTSettings _options, Credentials credential)
        {
            var payload = new Dictionary<string, object>
              {
                { "id", credential.Id },
                { "sub", credential.Username },
                { "email", credential.Email },
                { "scope", "admin" }
              };
            return GetToken(_options, payload);
        }

        private static string GetToken(JWTSettings _options, Dictionary<string, object> payload)
        {
            var secret = _options.SecretKey;
            //byte[] key = Convert.FromBase64String(secret);

            payload.Add("iss", _options.Issuer);
            payload.Add("aud", _options.Audience);
            payload.Add("nbf", ConvertToUnixTimestamp(DateTime.Now));
            payload.Add("iat", ConvertToUnixTimestamp(DateTime.Now));
            payload.Add("exp", ConvertToUnixTimestamp(DateTime.Now.AddDays(1)));
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(payload, secret);
        }

        private static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static string Decode(string token, string secret)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var json = decoder.Decode(token, secret, verify: true);
                return json;
            }
            catch (TokenExpiredException)
            {
                return "Token has expired";
            }
            catch (SignatureVerificationException)
            {
                return "Token has invalid signature";
            }
        }        
    }
}
