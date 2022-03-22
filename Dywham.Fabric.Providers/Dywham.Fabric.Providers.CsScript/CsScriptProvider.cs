using CSScriptLib;

namespace Dywham.Fabric.Providers.CsScript
{
    public class CsScriptProvider : ICsScriptProvider
    {
        public T LoadClass<T>(string code) where T : class
        {
            return CSScript.Evaluator.LoadCode<T>(code);
        }
    }
}
