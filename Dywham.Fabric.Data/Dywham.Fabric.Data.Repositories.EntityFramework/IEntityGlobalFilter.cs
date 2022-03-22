using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Dywham.Fabric.Data.Repositories.EntityFramework
{
    public interface IEntityGlobalFilter<T>
    {
        [NotMapped]
        Expression<Func<T, bool>> Filter { get; set; }
    }
}