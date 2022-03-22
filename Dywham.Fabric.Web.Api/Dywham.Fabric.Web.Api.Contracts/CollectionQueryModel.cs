namespace Dywham.Fabric.Web.Api.Contracts
{
    public class CollectionQueryModel
    {
        public int? Limit { get; set; }

        public int? StartIndex { get; set; }

        public bool? Asc { get; set; } = true;

        public string ColumnName { get; set; }
    }
}
