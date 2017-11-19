using DICOMcloud.DataAccess.Database.Schema;

namespace DICOMcloud.DataAccess.Database
{
    public interface ISortingStrategy
    {
        void Sort (IQueryOptions options, TableKey queryLevel) ;

        string SortBy { get; }

        SortingDirection Direction { get; }
    }

    public enum SortingDirection
    {
        ASC,
        DESC
    }
}
