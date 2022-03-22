using System.Threading;
using System.Threading.Tasks;

namespace Dywham.Fabric.Providers.Network.Email
{
    public interface IEmailProvider
    {
        Task SendEmailAsync(string subject, string body, bool isBodyHtml, string destination, CancellationToken token);

        Task SendEmailAsync(SenderDetails details, string subject, string body, bool isBodyHtml, string destination, CancellationToken token);
    }
}
