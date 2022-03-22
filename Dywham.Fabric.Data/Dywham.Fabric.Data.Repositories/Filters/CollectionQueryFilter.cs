namespace Dywham.Fabric.Data.Repositories.Filters
{
    public class CollectionQueryFilter
    {
        public int? Limit { get; set; }

        public int? StartIndex { get; set; }

        public bool Asc { get; set; } = true;

        public string ColumnName { get; set; }
    }
}