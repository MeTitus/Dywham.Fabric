using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities
{
    [Table("EventAuditing")]
    public class EventAuditingEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string TypeName { get; set; }

        public string Identities { get; set; }

        public string Payload { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateTime { get; set; }

        public Guid? TrackingId { get; set; }

        public string OriginatedInTheContextOfTypeName { get; set; }

        public string OriginatedInTheContextOfPayload { get; set; }
    }
}