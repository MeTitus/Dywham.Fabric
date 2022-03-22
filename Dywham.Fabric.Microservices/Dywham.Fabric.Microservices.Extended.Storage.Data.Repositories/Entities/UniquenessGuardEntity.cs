using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities
{
    [Table("UniquenessGuard")]
    public class UniquenessGuardEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateTime { get; set; }

        public string UniqueKey { get; set; }
    }
}