using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Leaderboard
{
    class AuthenticatedHttpClientHandler : HttpClientHandler
    {
        private string _key;
        public AuthenticatedHttpClientHandler(string key)
        {
            _key = key;

        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            request.Headers.Add("x-token", GenerateToken());
            return base.SendAsync(request, cancellationToken);
        }

        private string GenerateToken()
        {

            return CreateToken(new { Issued = DateTime.UtcNow, Expiration = DateTime.UtcNow.AddSeconds(60) });

        }

        public static string Base64Encode(string data)
        {
            try
            {
                var byte_data = System.Text.Encoding.UTF8.GetBytes(data);
                string encodedData = Convert.ToBase64String(byte_data);
                return encodedData;
            }
            catch (Exception e)
            {
                throw new Exception("An error occured during base 64 encoding.", e);
            }
        }

        private string CreateToken(object data)
        {
            var str = Base64Encode(JsonConvert.SerializeObject(data));
            return string.Format("{0}.{1}", str, ComputeSignature(str, _key));
        }

        private static string ComputeSignature(string data, string key)
        {
            using (var sha = SHA256CryptoServiceProvider.Create())
            {
                sha.Initialize();
                var bytes = System.Text.Encoding.UTF8.GetBytes(data + key);
                return Convert.ToBase64String(sha.ComputeHash(bytes));
            }
        }
    }
}
