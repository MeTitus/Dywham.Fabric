using System;
using System.Printing;
using System.Drawing;
using System.Drawing.Printing;

namespace Dywham.Fabric.Providers.Hardware.Printing.Generic
{
#pragma warning disable CA1416 // Validate platform compatibility

    public class GenericPrinterProvider : IGenericPrinterProvider
    {
        public PrinterStatus CheckPrinterStatus(string address)
        {
            PrintQueue printQueue = null;

            // ReSharper disable once ConvertToUsingDeclaration
            using (var printServer = new PrintServer())
            {
                foreach (var item in printServer.GetPrintQueues())
                {
                    if (!item.FullName.ToUpperInvariant().Contains(address.ToUpper())) continue;

                    printQueue = item;

                    break;
                }

                if (printQueue == null) return new PrinterStatus();

                return new PrinterStatus
                {
                    Name = address,
                    HasPaperProblem = printQueue.HasPaperProblem,
                    IsOutOfPaper = printQueue.IsOutOfPaper,
                    IsBusy = printQueue.IsBusy,
                    IsOffLine = printQueue.IsOffline,
                    IsPaperJammed = printQueue.IsPaperJammed,
                    NeedUserIntervention = printQueue.NeedUserIntervention,
                    IsOk = printQueue.HasPaperProblem == false
                           && printQueue.IsOutOfPaper == false
                           && printQueue.IsBusy == false
                           && printQueue.IsPaperJammed == false
                           && printQueue.NeedUserIntervention == false
                };
            }
        }

        public void Print(string printerName, string imageFile, PrintingOptions options)
        {
            var image = (Bitmap) Image.FromFile(imageFile);

            using (var printDocument = new PrintDocument())
            {
                var paperSize = new PaperSize(Guid.NewGuid().ToString(), options.PaperWidth, options.PaperHeight);

                printDocument.PrinterSettings.PrinterName = printerName;
                printDocument.DefaultPageSettings.Margins = options.Margins;
                printDocument.DefaultPageSettings.PaperSize = paperSize;
                printDocument.OriginAtMargins = true;
                
                if (options.Resolution != null)
                {
                    printDocument.PrinterSettings.DefaultPageSettings.PrinterResolution = new PrinterResolution
                    {
                        Kind = PrinterResolutionKind.High,
                        X = options.Resolution.X,
                        Y = options.Resolution.Y
                    };
                }

                printDocument.PrintPage += (sender, eventArgs) =>
                {
                    var g = eventArgs.Graphics;

                    g.DrawImageUnscaled(image, 0, 0);
                };

                printDocument.Print();
            }
        }
    }
}

#pragma warning restore CA1416 // Validate platform compatibility