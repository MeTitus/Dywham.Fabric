using System.ComponentModel.DataAnnotations.Schema;

namespace Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities
{
    [Table("Operational")]
    public class OperationalEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Jobs { get; set; }

        public string Settings { get; set; }

        public string Execution { get; set; }

        public int Version { get; set; }
    }
}