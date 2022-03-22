using System;

namespace Dywham.Fabric.Microservices.Extended.Contracts.Messages.Commands
{
    public abstract class ToggleOperationalJobState : ExtendedCommand, IRequiresUserIdentification
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public bool? Enable { get; set; }
    }
}