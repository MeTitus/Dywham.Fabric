using System;

namespace Dywham.Fabric.Providers.Process
{
    public class StartProcessOptions
    {
        public bool SingleInstance { get; set; }

        public Action<string> OnStandardOutputDataReceived { get; set; }

        public Action<object, EventArgs> OnExit { get; set; }
        
        public bool? WaitUntilAvailable { get; set; }

        public int? MaxTimeToWait { get; set; }

        public Func<bool> IsAvailable { get; set; }

        public bool BindToCurrentProcess { get; set; }
    }
}
