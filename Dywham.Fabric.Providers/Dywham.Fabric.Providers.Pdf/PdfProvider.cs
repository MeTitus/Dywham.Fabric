using System;
using System.Diagnostics;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Dywham.Fabric.Providers.Pdf
{
    public class PdfProvider : IPdfProvider
    {
        public string WkHtmlToPdfExecutablePath { get; set; }


        public byte[] ConvertHtml(string htmlContent)
        {
            return GeneratePdf(htmlContent);
        }

        public void ConvertHtml(string htmlContent, string filename)
        {
            File.WriteAllBytes(filename, GeneratePdf(htmlContent));
        }

        public void ConvertFile(string htmlFilename, string filename)
        {
            var bytes = GeneratePdf(File.ReadAllText(htmlFilename));

            File.WriteAllBytes(filename, bytes);
        }

        private byte[] GeneratePdf(string htmlContent)
        {
            var tempPdfFile = Guid.NewGuid() + ".pdf";
            var htmlFile = string.Empty;

            try
            {
                // wkhtmltopdf --load-error-handling ignore file:///C:/Program%20Files/wkhtmltopdf/bin/test.html google.pdf  

                if (string.IsNullOrWhiteSpace(WkHtmlToPdfExecutablePath))
                {
                    throw new InvalidOperationException($"'{nameof(WkHtmlToPdfExecutablePath)}' property is not specified");
                }

                if (!File.Exists(WkHtmlToPdfExecutablePath))
                {
                    throw new FileNotFoundException($"'{WkHtmlToPdfExecutablePath}' not found");
                }

                htmlFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".html");

                var htmlFileUri = new Uri(htmlFile).AbsoluteUri;

                File.WriteAllText(htmlFile, htmlContent);

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo(WkHtmlToPdfExecutablePath, $"--load-error-handling ignore {htmlFileUri} {tempPdfFile}"),
                };

                using (process)
                {
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.Start();
                    process.WaitForExit();
                }

                if (!File.Exists(tempPdfFile))
                {
                    throw new InvalidOperationException("Couldn't generate the pdf file");
                }

                return File.ReadAllBytes(tempPdfFile);
            }
            finally
            {
                SafeFileDelete(tempPdfFile);
                SafeFileDelete(htmlFile);
            }
        }

        private static void SafeFileDelete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
                // ignored
            }
        }

        public void Merge(string targetPath, params string[] pdfs)
        {
            using (var targetDoc = new PdfDocument())
            {
                foreach (var pdf in pdfs)
                {
                    using (var pdfDoc = PdfReader.Open(pdf, PdfDocumentOpenMode.Import))
                    {
                        for (var i = 0; i < pdfDoc.PageCount; i++)
                        {
                            targetDoc.AddPage(pdfDoc.Pages[i]);
                        }
                    }
                }

                targetDoc.Save(targetPath);
            }
        }
    }
}
