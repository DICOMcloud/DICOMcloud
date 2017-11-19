
using fo = Dicom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database.Schema
{
    public class StorageDbSchemaProvider : DbSchemaProvider
    {
        public StorageDbSchemaProvider ( )
        {
            Init ( ) ;
        }

        public StorageDbSchemaProvider ( DbSchemaSource source ) 
        : base ( source )
        {
            Init ( ) ;
        }

        public override string GetQueryTable ( string queryLevel )
        {
            if ( queryLevel == Enum.GetName ( typeof(ObjectQueryLevel), ObjectQueryLevel.Instance ) )
            {
                return ObjectInstanceTableName ;
            }

            return base.GetQueryTable ( queryLevel ) ;
        }

        public  TableKey PatientTable { get; private set; }
        public  TableKey StudyTable { get; private set; }
        public  TableKey SeriesTable { get; private set; }
        public  TableKey ObjectInstanceTable { get; private set; }


        //TODO: replace access to this with a QueryBase Key that corresponds to a table defined in the schema (key=name or new attribute)
        public const string PatientTableName        = "Patient" ;
        public const string StudyTableName          = "Study" ;
        public const string SeriesTableName         = "Series" ;
        public const string ObjectInstanceTableName ="ObjectInstance" ;

        public class MetadataTable
        {
            public static string TableName         = "ObjectInstance" ;
            public static string SopInstanceColumn = "SopInstanceUid" ;
            public static string MetadataColumn    = "Metadata" ;
            public static string OwnerColumn       = "Owner" ;
        }

        private void Init ( ) 
        {
            PatientTable        = GetTableInfo ( PatientTableName) ;
            StudyTable          = GetTableInfo ( StudyTableName ) ;
            SeriesTable         = GetTableInfo ( SeriesTableName ) ;
            ObjectInstanceTable = GetTableInfo ( ObjectInstanceTableName ) ;
        }
        
    }
}
