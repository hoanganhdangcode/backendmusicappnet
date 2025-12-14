using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Net.MusicApp.Services
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
        public static RSA LoadRsaPublicKeyFromPem(string pemPath)
        {
            var text = File.ReadAllText(pemPath);
            var rsa = RSA.Create();
            if (text.Contains("BEGIN RSA PUBLIC KEY"))
            {
                // PKCS#1
                var key = DecodePem(text, "RSA PUBLIC KEY");
                rsa.ImportRSAPublicKey(key, out _);
            }
            else if (text.Contains("BEGIN PUBLIC KEY"))
            {
                // X.509 SubjectPublicKeyInfo
                var key = DecodePem(text, "PUBLIC KEY");
                rsa.ImportSubjectPublicKeyInfo(key, out _);
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

            var start = pem.IndexOf(header, StringComparison.Ordinal);
            if (start < 0) throw new Exception("PEM header not found");
            start += header.Length;

            var end = pem.IndexOf(footer, start, StringComparison.Ordinal);
            if (end < 0) throw new Exception("PEM footer not found");

            var base64 = pem[start..end].Replace("\r", "").Replace("\n", "").Trim();
            return Convert.FromBase64String(base64);
        }


        public static string GenerateToken(IEnumerable<Claim> claim, TimeSpan timetoexp)
        {
            var rsa = RSA.Create();
             rsa = LoadRsaPrivateKeyFromPem("Keys/rsa_private_key.pem");

            var credentials = new SigningCredentials(
                new RsaSecurityKey(rsa),
                SecurityAlgorithms.RsaSha256
            );


            var token = new JwtSecurityToken(
                claims: claim,
                expires: DateTime.UtcNow.Add(timetoexp),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
