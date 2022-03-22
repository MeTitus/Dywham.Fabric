using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Microservices.Contracts.Messages;
using NServiceBus;

namespace Dywham.Fabric.Microservices.Endpoint.Handlers;

public abstract class DywhamMessageHandler<T> : IHandleMessages<T> where T : DywhamMessage
{
    // ReSharper disable once PossibleNullReferenceException
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once StaticMemberInGenericType
    protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected IMessageHandlerContext Context;
    protected T Message;


    public IDywhamEndpointInstance DywhamEndpointInstance { get; set; }


    public Task Handle(T message, IMessageHandlerContext context)
    {
        Message = message;

        Context = context;

        return HandleAsync(CancellationToken.None);
    }

    protected abstract Task HandleAsync(CancellationToken token);
}