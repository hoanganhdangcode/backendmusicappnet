using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Net.MusicApp.Services.Common
{
    public static class JWTHelper
    {
        public static RSA LoadRsaPrivateKeyFromPem(string pemPath)
        {
            var text = File.ReadAllText(pemPath);

            var rsa = RSA.Create();

            if (text.Contains("BEGIN RSA PRIVATE KEY"))
            {
                // PKCS#1
                var key = DecodePem(text, "RSA PRIVATE KEY");
                rsa.ImportRSAPrivateKey(key, out _);
            }
            else if (text.Contains("BEGIN PRIVATE KEY"))
            {
                // PKCS#8 (Common nowadays)
                var key = DecodePem(text, "PRIVATE KEY");
                rsa.ImportPkcs8PrivateKey(key, out _);
            }
            else
            {
                throw new Exception("Unsupported key format");
            }

            return rsa;
        }

        private static byte[] DecodePem(string pem, string section)
        {
            var header = $"-----BEGIN {section}-----";
            var footer = $"-----END {section}-----";

            var body = pem.Replace(header, "")
                          .Replace(footer, "")
                          .Replace("\n", "")
                          .Replace("\r", "")
                          .Trim();

            return Convert.FromBase64String(body);
        }

        public static string GenerateToken(IEnumerable<Claim> claim)
        {
            var rsa = RSA.Create();
             rsa = LoadRsaPrivateKeyFromPem("Keys/rsa_private_key.pem");

            var credentials = new SigningCredentials(
                new RsaSecurityKey(rsa),
                SecurityAlgorithms.RsaSha256
            );


            var token = new JwtSecurityToken(
                claims: claim,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
