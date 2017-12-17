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
            SchemaProvider = schemaProvider ;
        }

        public virtual string Sort (QueryBuilder queryBuilder, IQueryOptions options, TableKey queryLeveTable )
        {
            IEnumerable<ColumnInfo> orderByColumns = null;
            

            Direction = SortingDirection.ASC;

            if (queryLeveTable == StorageDbSchemaProvider.StudyTableName)
            {
                var studyTable = SchemaProvider.GetTableInfo (StorageDbSchemaProvider.StudyTableName) ;

                orderByColumns = new ColumnInfo [] { studyTable.KeyColumn } ;

                Direction = SortingDirection.DESC ;
            }
            else if (queryLeveTable == StorageDbSchemaProvider.SeriesTableName)
            {
                orderByColumns = SchemaProvider.GetColumnInfo((uint)DicomTag.SeriesNumber);
            }

            if (queryLeveTable == StorageDbSchemaProvider.ObjectInstanceTableName)
            {
                orderByColumns = SchemaProvider.GetColumnInfo((uint)DicomTag.InstanceNumber);
            }

            
            if (null != orderByColumns)
            {
                
                SortBy = string.Join(",", orderByColumns.Select ( column => (string)column ));
            
                foreach ( var column in orderByColumns )
                {
                    if ( !queryBuilder.ProcessedColumns.ContainsKey ( queryLeveTable ) ||
                         ( queryBuilder.ProcessedColumns.ContainsKey ( queryLeveTable ) &&
                         !queryBuilder.ProcessedColumns[queryLeveTable].Contains ( column )) )
                    {
                        queryBuilder.ProcessColumn ( queryLeveTable, column ) ;
                    }
                }

                string queryText = queryBuilder.GetQueryText(queryLeveTable, options ) ;


                if ( null != options && options.Limit > 0 )
                {
                    CountColumn = "TotalRows" ;
                    return string.Format ( Sorting_Template, 
                                           queryText,
                                           CountColumn,
                                           string.Format ( OrderBy_Template,  SortBy, (( Direction == SortingDirection.DESC ) ? "DESC" : "ASC" )),
                                           options.Offset,
                                           options.Limit ) ;
                }
                else
                {
                    return queryText + string.Format ( OrderBy_Template,  SortBy, (( Direction == SortingDirection.DESC ) ? "DESC" : "ASC" ) );
                }
            }
            else
            {
                return queryBuilder.GetQueryText(queryLeveTable, options ) ;
            }
        }

        public string SortBy { get; protected set; }

        public SortingDirection Direction { get; protected set ;}
        public string CountColumn { get; private set; }

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
OFFSET {3} ROWS
FETCH NEXT {4} ROWS ONLY;
";
    }
}
