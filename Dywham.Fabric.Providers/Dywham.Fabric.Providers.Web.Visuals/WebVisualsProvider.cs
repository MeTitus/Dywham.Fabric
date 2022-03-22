using System;
using System.Diagnostics;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;

namespace Dywham.Fabric.Providers.Web.Visuals
{
    public class WebVisualsProvider : IWebVisualsProvider
    {
        public string WkHtmlToImageExecutablePath { get; set; }


        public string RenderHtml(string templateSource, object model)
        {
            return RenderHtml(templateSource, Guid.NewGuid().ToString(), null, model);
        }

        public string RenderHtml(string templateSource, Type modelType, object model)
        {
            return RenderHtml(templateSource, Guid.NewGuid().ToString(), modelType, model);
        }

        public string RenderHtml(string templateSource, string templateKey, Type modelType, object model)
        {
            return Engine.Razor.RunCompile(templateSource, templateKey, modelType, model);
        }

        public byte[] ConvertHtml(string htmlContent)
        {
            return GenerateImage(htmlContent);
        }

        public void ConvertHtml(string htmlContent, string filename)
        {
            File.WriteAllBytes(filename, GenerateImage(htmlContent));
        }

        public void ConvertFile(string htmlFilename, string filename)
        {
            var bytes = GenerateImage(File.ReadAllText(htmlFilename));

            File.WriteAllBytes(filename, bytes);
        }

        private byte[] GenerateImage(string htmlContent)
        {
            var tempPdfFile = Guid.NewGuid() + ".png";
            var htmlFile = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(WkHtmlToImageExecutablePath))
                {
                    throw new InvalidOperationException($"'{nameof(WkHtmlToImageExecutablePath)}' property is not specified");
                }

                if (!File.Exists(WkHtmlToImageExecutablePath))
                {
                    throw new FileNotFoundException($"'{WkHtmlToImageExecutablePath}' not found");
                }

                htmlFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".html");

                var htmlFileUri = new Uri(htmlFile).AbsoluteUri;

                File.WriteAllText(htmlFile, htmlContent);

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo(WkHtmlToImageExecutablePath, $"--load-error-handling ignore {htmlFileUri} {tempPdfFile}"),
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
                    throw new InvalidOperationException("Couldn't generate the image file");
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
    }
}
