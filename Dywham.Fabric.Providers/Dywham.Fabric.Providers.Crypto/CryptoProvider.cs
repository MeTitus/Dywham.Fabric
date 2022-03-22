using System;
using System.Security.Cryptography;
using System.Text;

namespace Dywham.Fabric.Providers.Crypto
{
    public class CryptoProvider : ICryptoProvider
    {
        private const int SaltSize = 32;
        private const int HashSize = 32;
        private const int IterationCount = 10000;


        public string CreateSha256Hash(string input)
        {
            var sha256 = SHA256.Create();

            sha256.Initialize();

            return CreateSha256Hash(input, sha256);
        }

        public string CreateSha256Hash(string input, SHA256 sha256)
        {
            var data = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public Tuple<string, string> GenerateRfc2898Hash(string password)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, SaltSize))
            {
                rfc2898DeriveBytes.IterationCount = IterationCount;

                var hashData = rfc2898DeriveBytes.GetBytes(HashSize);
                var saltData = rfc2898DeriveBytes.Salt;

                return new Tuple<string, string>(Convert.ToBase64String(hashData), Convert.ToBase64String(saltData));
            }
        }

        public bool ValidateRfc2898Hash(string password, string passwordHash, string salt)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, SaltSize))
            {
                rfc2898DeriveBytes.IterationCount = IterationCount;
                rfc2898DeriveBytes.Salt = Convert.FromBase64String(salt);

                var hashData = rfc2898DeriveBytes.GetBytes(HashSize);

                return Convert.ToBase64String(hashData) == passwordHash;
            }
        }
    }
}