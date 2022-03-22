using System.Drawing;
using QRCoder;

namespace Dywham.Fabric.Providers.QRCoder
{
    public class QrCoderGenerationSettings
    {
        public int PixelsPerModule { get; set; } = 20;

        public Color DarkColor { get; set; } = Color.Black;

        public Color LightColor { get; set; } = Color.White;

        public bool DrawQuietZones { get; set; } = true;

        public QRCodeGenerator.ECCLevel ECCLevel { get; set; } = QRCodeGenerator.ECCLevel.L;
    }
}