using System.Drawing.Printing;

namespace Dywham.Fabric.Providers.Hardware.Printing
{
    public class PrintingOptions
    {
        public int PaperWidth { get; set; } = 327;

        public int PaperHeight { get; set; } = 48;

        public PrintingResolution Resolution { get; set; }

#pragma warning disable CA1416 // Validate platform compatibility
        public Margins Margins { get; set; } = new Margins(23, 0, 19, 0);
#pragma warning restore CA1416 // Validate platform compatibility
    }
}
