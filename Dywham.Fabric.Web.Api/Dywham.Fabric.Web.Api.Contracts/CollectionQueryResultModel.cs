using System.Collections.Generic;

namespace Dywham.Fabric.Web.Api.Contracts
{
    public class CollectionQueryResultModel<T>
    {
        public IList<T> Set { get; set; }

        public long TotalCount { get; set; }

        public long SetSize { get; set; }
    }
}