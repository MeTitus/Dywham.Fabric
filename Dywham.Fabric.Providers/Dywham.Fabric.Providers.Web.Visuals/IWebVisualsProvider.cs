using System;

namespace Dywham.Fabric.Providers.Web.Visuals
{
    public interface IWebVisualsProvider : IProvider
    {
        string RenderHtml(string templateSource, object model);

        string RenderHtml(string templateSource, Type modelType, object model);

        string RenderHtml(string templateSource, string templateKey, Type modelType = null, object model = null);

        byte[] ConvertHtml(string htmlContent);

        void ConvertHtml(string htmlContent, string imageFilename);

        void ConvertFile(string htmlFilename, string imageFilename);
    }
}
