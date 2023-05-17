using fo = Dicom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database.Schema
{
    
    public class DbSchemaProvider
    {
        public DbSchemaProvider ( ) : this ( new DbSchemaSource ( ) )
        {}

        public DbSchemaProvider ( DbSchemaSource source )
        {
            SchemaSource  = source ;
        }

        public  string GetTableName ( fo.DicomTag tag )
        {
                IList<ColumnInfo> columns = null ;
            if ( SchemaSource.Tags.TryGetValue ( (uint)tag, out columns ) )
            {
                return columns.FirstOrDefault ( ).Table.Name ;
            }

            return null ;
        }

        public  TableKey GetTableInfo ( string tableName )
        {
            return SchemaSource.Tables [ tableName ] ;
        }

        public  ColumnInfo GetColumn ( string tableName, string columnName )
        {
            string     columnKey = SchemaSource.GetColumnKey ( tableName, columnName ) ;
            ColumnInfo column    = null ;

            
            SchemaSource.Columns.TryGetValue ( columnKey, out column ) ;
            
            return column ;
        }

        public  IEnumerable<ColumnInfo> GetColumnInfo ( uint tag  )
        {
            IList<ColumnInfo> result = null ;

            if ( !SchemaSource.Tags.TryGetValue ( tag, out result ) )
            { 
                result = new List<ColumnInfo> ( ) ;
            }

            return result ;
        }

        public  IEnumerable<uint> GetDicomTag ( string tableName, string columnName )
        {
            string columnKey = SchemaSource.GetColumnKey ( tableName, columnName ) ;
            ColumnInfo column = null ;

            if ( SchemaSource.Columns.TryGetValue (columnKey, out column) )
            {
                return column.Tags ;
            }

            
            return null ;
        }

        public  PersonNameParts GetPNColumnPart ( string columnName )
        { 
            string[] parts = columnName.Split ( '_' ) ;

            if (parts.Length != 2) {  throw new ArgumentException ( "Invalid person name column: " + columnName ) ; }

            return (PersonNameParts) Enum.Parse ( typeof(PersonNameParts), parts[1] ) ;
        }

        public DbSchemaSource SchemaSource
        {
            get;
            protected set;
        }


        public virtual string GetQueryTable ( string queryLevel )
        {
            return queryLevel ; //assume that query level matches database table name (e.g. Study, Series, Instance...)
        }


        abstract class DbConstants
        {
            public abstract class PersonName
            { 
                public const string Family =  "Family" ; //Enum.GetName ( typeof(PersonNameParts) , PersonNameParts.Family ) ;
                public const string Given =  "Given" ; 
                public const string Middle =  "Middle" ; 
                public const string Prefix =  "Prefix" ; 
                public const string Suffix =  "Suffix" ; 
            }
        }
    }
}
