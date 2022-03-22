using System.Drawing;

namespace Dywham.Fabric.Providers.QRCoder
{
    public interface IQrCoderProvider : IProvider
    {
        Bitmap Generate(string data);

        Bitmap Generate(string data, QrCoderGenerationSettings settings);
        
        string GeneratePngAsBase64(string data);
        
        string GeneratePngAsBase64(string data, QrCoderGenerationSettings settings);

        string GenerateAsSvg(string data);

        string GenerateAsSvg(string data, QrCoderGenerationSettings settings);

        string GenerateSvgAsBase64(string data);

        string GenerateSvgAsBase64(string data, QrCoderGenerationSettings settings);

        void GenerateAndSave(string data, string location);

        void GenerateAndSave(string data, string location, QrCoderGenerationSettings settings);
    }
}