using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Dywham.Fabric.Data.Repositories.EntityFramework
{
    public record QueryProperty<T> : IRepository where T : class, new()
    {
        public Func<DbSet<T>, IQueryable<T>> Where { get; set; }

        public QueryResultOrdering OrderedSet { get; set; }

        public int? Limit { get; set; }

        public int? StartIndex { get; set; }
    }
}