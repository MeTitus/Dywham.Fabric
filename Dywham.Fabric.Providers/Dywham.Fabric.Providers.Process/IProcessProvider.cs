using System.Collections.Generic;
using System.Diagnostics;

namespace Dywham.Fabric.Providers.Process
{
    public interface IProcessProvider : IProvider
    {
        bool Exists(int id);

        bool Exists(string name);

        void Kill(int id);

        void KillByWindowTitle(string title);

        void SetWindowTitle(int id, string title);

        IList<System.Diagnostics.Process> GetByName(string name);

        System.Diagnostics.Process GetById(int id);

        int? Start(string path, StartProcessOptions options);

        int? Start(ProcessStartInfo info, StartProcessOptions options);
    }
}