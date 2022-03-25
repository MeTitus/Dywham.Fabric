using System.Collections.Generic;

namespace Dywham.Fabric.Data.Repositories
{
    public class QueryResult<T>
    {
        public IList<T> Data { get; set; }

        public long TotalCount { get; set; }

        public long SetSize { get; set; }
    }
}