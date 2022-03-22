namespace Dywham.Fabric.Providers
{
    public interface IEnvironmentProvider : IProvider
    {
        bool VariableExists(string name);

        bool TryGetAsString(string name, out string value);

        bool TryGetAsInt(string name, out int value);

        bool TryGetAsDecimal(string name, out decimal value);
    }
}