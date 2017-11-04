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

        public virtual void Sort (IQueryOptions options, TableKey queryLeveTable )
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
            }
        }

        public string SortBy { get; protected set; }

        public SortingDirection Direction { get; protected set ;}
    }
}
