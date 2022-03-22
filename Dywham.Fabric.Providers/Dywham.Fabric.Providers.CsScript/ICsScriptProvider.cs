namespace Dywham.Fabric.Providers.CsScript
{
    public interface ICsScriptProvider : IProvider
    {
        T LoadClass<T>(string code) where T : class;
    }
}