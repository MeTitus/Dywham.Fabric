using System;

namespace Dywham.Fabric.Microservices.Extended.Contracts.Messages.Events
{
    public interface ILongRunningTaskCompleted
    {
        Guid Id { get; set; }
    }
}