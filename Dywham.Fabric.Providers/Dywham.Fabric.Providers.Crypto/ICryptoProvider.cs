using System;
using System.Security.Cryptography;
using Dywham.Fabric.Providers;

namespace Dywham.Fabric.Providers.Crypto
{
    public interface ICryptoProvider : IProvider
    {
        string CreateSha256Hash(string input);

        string CreateSha256Hash(string input, SHA256 sha256);

        Tuple<string, string> GenerateRfc2898Hash(string text);

        bool ValidateRfc2898Hash(string text, string textHash, string salt);
    }
}