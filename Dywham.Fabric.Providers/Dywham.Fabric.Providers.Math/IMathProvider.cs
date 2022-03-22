namespace Dywham.Fabric.Providers.Math
{
    public interface IMathProvider : IProvider
    {
        decimal TruncateWithoutRounding(decimal value, byte decimals);

        decimal TruncateWithoutRounding(double value, byte decimals);
    }
}