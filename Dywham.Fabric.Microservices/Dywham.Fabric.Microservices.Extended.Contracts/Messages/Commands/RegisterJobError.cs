using System;

namespace Dywham.Fabric.Microservices.Extended.Contracts.Messages.Commands
{
    public class RegisterJobError : ExtendedCommand
    {
        public string Name { get; set; }

        public string Error { get; set; }

        public Guid? Id { get; set; }
    }
}