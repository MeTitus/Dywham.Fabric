namespace Dywham.Fabric.Data.Repositories.Filters
{
    public class QueryFilter
    {
        public bool Asc { get; set; } = true;

        public string ColumnName { get; set; }

        public int? Limit { get; set; }

        public int? StartIndex { get; set; }
    }
}