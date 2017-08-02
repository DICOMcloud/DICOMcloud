using DICOMcloud.DataAccess.Database.Schema;

namespace DICOMcloud.DataAccess.Database.Sql
{
    public class SqlObjectArchieveDataAccess : ObjectArchieveDataAccess
    {
        public SqlObjectArchieveDataAccess ( ){}

        public SqlObjectArchieveDataAccess ( string connectionString ) : base ( connectionString ) 
        {}

        protected override ObjectArchieveDataAdapter CreateDataAdapter ( )
        {
            return new SqlObjectArchieveDataAdapter ( ConnectionString, SchemaProvider ) ;
        }

        protected override DbSchemaProvider CreateSchemaProvider()
        {
            return new StorageDbSchemaProvider ( ) ;
        }
    }
}
