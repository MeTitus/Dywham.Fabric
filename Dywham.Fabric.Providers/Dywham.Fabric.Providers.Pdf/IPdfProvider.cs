namespace Dywham.Fabric.Providers.Pdf
{
    public interface IPdfProvider : IProvider
    {
        byte[] ConvertHtml(string htmlContent);

        void ConvertHtml(string htmlContent, string pdfFilename);

        void ConvertFile(string htmlFilename, string pdfFilename);

        void Merge(string targetPath, params string[] pdfs);
    }
}