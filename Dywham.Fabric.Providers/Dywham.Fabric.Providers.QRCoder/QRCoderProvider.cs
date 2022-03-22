using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Text;
using QRCoder;

namespace Dywham.Fabric.Providers.QRCoder
{
    [SupportedOSPlatform("windows")]
    public class QrCoderProvider : IQrCoderProvider
    {
        public Bitmap Generate(string data)
        {
            return Generate(data, new QrCoderGenerationSettings());
        }

        public Bitmap Generate(string data, QrCoderGenerationSettings settings)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(data, settings.ECCLevel))
            using (var qrCode = new QRCode(qrCodeData))
            {
                return qrCode.GetGraphic(settings.PixelsPerModule, settings.DarkColor,
                    settings.LightColor, settings.DrawQuietZones);
            }
        }

        public string GeneratePngAsBase64(string data)
        {
            return GeneratePngAsBase64(data, new QrCoderGenerationSettings());
        }

        public string GeneratePngAsBase64(string data, QrCoderGenerationSettings settings)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(data, settings.ECCLevel))
            using (var qrCode = new Base64QRCode(qrCodeData))
            {
                return qrCode.GetGraphic(settings.PixelsPerModule, settings.DarkColor,
                    settings.LightColor, settings.DrawQuietZones);
            }
        }

        public string GenerateAsSvg(string data)
        {
            return GenerateAsSvg(data, new QrCoderGenerationSettings());
        }

        public string GenerateAsSvg(string data, QrCoderGenerationSettings settings)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(data, settings.ECCLevel))
            using (var qrCode = new SvgQRCode(qrCodeData))
            {
                return qrCode.GetGraphic(settings.PixelsPerModule, settings.DarkColor,
                    settings.LightColor, settings.DrawQuietZones);
            }
        }

        public string GenerateSvgAsBase64(string data)
        {
            return GenerateSvgAsBase64(data, new QrCoderGenerationSettings());
        }

        public string GenerateSvgAsBase64(string data, QrCoderGenerationSettings settings)
        {
            var svg = GenerateAsSvg(data, settings);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(svg));
        }

        public void GenerateAndSave(string data, string location)
        {
            GenerateAndSave(data, location, new QrCoderGenerationSettings());
        }

        public void GenerateAndSave(string data, string location, QrCoderGenerationSettings settings)
        {
            var bitmap = Generate(data, settings);

            bitmap.Save(location);
        }
    }
}