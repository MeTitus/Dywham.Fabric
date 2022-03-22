using System;

namespace Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories
{
    [Serializable]
    public class UnexpectedNoOfUpdatedItemsException : Exception
    {
        public UnexpectedNoOfUpdatedItemsException()
        { }

        public UnexpectedNoOfUpdatedItemsException(int expected, int actual)
            : base($"Expected: {expected} actual: {actual}")
        { }
    }
}