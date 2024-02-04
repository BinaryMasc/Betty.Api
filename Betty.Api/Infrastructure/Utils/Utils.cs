using BettyApi.Models;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Betty.Api.Infrastructure.Utils
{
    public static class Utils
    {
        public static User? GetUserFromContext(ClaimsPrincipal User) => 
            JsonConvert.DeserializeObject<User>(User.Claims.FirstOrDefault(c => c.Type == typeof(User).ToString())?.Value ?? throw new Exception("Deserialization error: Invalid Token."));

        public static string GetHash(string str)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
