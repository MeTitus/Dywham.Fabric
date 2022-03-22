using Dywham.Fabric.Providers;

namespace Dywham.Fabric.Providers.Hardware.Printing
{
    public interface IPrintingProvider : IProvider
    {
        PrinterStatus CheckPrinterStatus(string address);

        void Print(string printerName, string imageFile, PrintingOptions options);
    }
}
