using DICOMcloud.DataAccess.Database.Schema;

namespace DICOMcloud.DataAccess.Database
{
    public interface ISortingStrategy
    {
        string Sort        (QueryBuilder queryText, IQueryOptions options, TableKey queryLevel) ;
        bool   CanPaginate (QueryBuilder queryText, IQueryOptions options, TableKey queryLevel);
        
        string           SortBy          { get; }
        string           CountColumn     { get; }
        SortingDirection Direction       { get; }
        bool             ApplyPagination { get; set; }
    }

    public enum SortingDirection
    {
        ASC,
        DESC
    }
}
