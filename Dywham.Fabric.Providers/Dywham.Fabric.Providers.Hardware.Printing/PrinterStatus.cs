namespace Dywham.Fabric.Providers.Hardware.Printing
{
    public class PrinterStatus
    {
        public string Name { get; set; }

        public bool IsOk { get; set; }

        public bool IsOffLine { get; set; } = true;

        public bool? IsPaperJammed { get; set; }

        public bool? NeedUserIntervention { get; set; }

        public bool? HasPaperProblem { get; set; }

        public bool? IsOutOfPaper { get; set; }

        public bool? IsBusy { get; set; }
    }
}
