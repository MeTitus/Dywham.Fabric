using System;

namespace Dywham.Fabric.Providers
{
    public class EnvironmentProvider : IEnvironmentProvider
    {
        public bool TryGetAsDecimal(string name, out decimal value)
        {
            return decimal.TryParse(Environment.GetEnvironmentVariable(name), out value);
        }

        public bool TryGetAsInt(string name, out int value)
        {
            return int.TryParse(Environment.GetEnvironmentVariable(name), out value);
        }

        public bool TryGetAsString(string name, out string value)
        {
            value = Environment.GetEnvironmentVariable(name);

            return !string.IsNullOrEmpty(value);
        }

        public bool VariableExists(string name)
        {
            return Environment.GetEnvironmentVariables().Contains(name);
        }
    }
}