using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom;
using DICOMcloud.DataAccess.Database.Schema;

namespace DICOMcloud.DataAccess.Database
{
    public class ObjectArchieveSortingStrategy : ISortingStrategy
    {
        public DbSchemaProvider SchemaProvider { get; protected set; }

        public ObjectArchieveSortingStrategy ( DbSchemaProvider schemaProvider )
        {
            SchemaProvider  = schemaProvider ;
            ApplyPagination = true;
        }

        public virtual string Sort (QueryBuilder queryBuilder, IQueryOptions options, TableKey queryLevelTable )
        {
            IEnumerable<ColumnInfo> orderByColumns = null;
            

            Direction = SortingDirection.ASC;

            if (queryLevelTable == StorageDbSchemaProvider.StudyTableName)
            {
                var studyTable = SchemaProvider.GetTableInfo (StorageDbSchemaProvider.StudyTableName) ;

                orderByColumns = new ColumnInfo [] { studyTable.KeyColumn } ;

                Direction = SortingDirection.DESC ;
            }
            else if (queryLevelTable == StorageDbSchemaProvider.SeriesTableName)
            {
                orderByColumns = SchemaProvider.GetColumnInfo((uint)DicomTag.SeriesNumber);
            }

            if (queryLevelTable == StorageDbSchemaProvider.ObjectInstanceTableName)
            {
                orderByColumns = SchemaProvider.GetColumnInfo((uint)DicomTag.InstanceNumber);
            }

            
            if (null != orderByColumns)
            {
                
                SortBy = string.Join(",", orderByColumns.Select ( column => (string)column ));
            
                foreach ( var column in orderByColumns )
                {
                    if ( !queryBuilder.ProcessedColumns.ContainsKey ( queryLevelTable ) ||
                         ( queryBuilder.ProcessedColumns.ContainsKey ( queryLevelTable ) &&
                         !queryBuilder.ProcessedColumns[queryLevelTable].Contains ( column )) )
                    {
                        queryBuilder.ProcessColumn ( queryLevelTable, column ) ;
                    }
                }

                string queryText = queryBuilder.GetQueryText(queryLevelTable, options ) ;


                if ( ApplyPagination && CanPaginate (queryBuilder, options, queryLevelTable) )
                {
                    CountColumn = "TotalRows" ;

                    return string.Format ( Sorting_Template, 
                                           queryText,
                                           CountColumn,
                                           string.Format ( OrderBy_Template,  SortBy, GetDirection()),
                                           string.Format(Pagination_Template, options.Offset, options.Limit)) ;
                }
                else
                {
                    return queryText + string.Format ( OrderBy_Template,  
                                                       SortBy, 
                                                       GetDirection ( ));
                }
            }
            else
            {
                return queryBuilder.GetQueryText(queryLevelTable, options ) ;
            }
        }

        public virtual bool CanPaginate
        (
            QueryBuilder queryBuilder,
            IQueryOptions options,
            TableKey queryLeveTable
        )
        {
            if (null == options || !options.Limit.HasValue || options.Limit <= 0) return false;

            foreach ( var tableColumns in queryBuilder.ProcessedColumns )
            { 
                // If a child table/columns are returned in the query then the sorting strategy will fail
                // Github: Issue #25
                if (tableColumns.Key.Parent == queryLeveTable)
                { 
                    return false ;
                }
            }

            return true;
        }

        public virtual string SortBy { get; protected set; }

        public virtual SortingDirection Direction { get; protected set ;}

        public virtual string CountColumn { get; private set; }

        public virtual bool ApplyPagination { get; set; }

        private string GetDirection ( )
        {
            return ((Direction == SortingDirection.DESC) ? "DESC" : "ASC");
        }

        private static string OrderBy_Template = " ORDER BY {0} {1}" ;
//http://andreyzavadskiy.com/2016/12/03/pagination-and-total-number-of-rows-from-one-select/
        private static string Sorting_Template = 
@"
WITH Data_CTE 
AS
(
{0}
), 
Count_CTE 
AS 
(
    SELECT COUNT(*) AS {1} FROM Data_CTE
)
SELECT *
FROM Data_CTE
CROSS JOIN Count_CTE
{2}
{3}
";

        private static string Pagination_Template = 
@"
OFFSET {0} ROWS
FETCH NEXT {1} ROWS ONLY;";
    }
}
