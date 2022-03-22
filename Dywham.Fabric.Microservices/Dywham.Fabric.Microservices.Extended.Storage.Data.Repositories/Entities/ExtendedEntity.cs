using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Dywham.Fabric.Data.Repositories.EntityFramework;

namespace Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities
{
    public abstract class ExtendedEntity
    {
        public DateTime CreatedOn { get; set; }

        public DateTime LastChangedOn { get; set; }

        public bool IsArchived { get; set; }
    }

    public abstract class ExtendedEntity<T> : ExtendedEntity, IEntityGlobalFilter<T> where T : ExtendedEntity
    {
        [NotMapped]
        public Expression<Func<T, bool>> Filter { get; set; } = x => !x.IsArchived;
    }
}