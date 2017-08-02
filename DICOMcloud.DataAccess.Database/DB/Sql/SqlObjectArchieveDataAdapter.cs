using System.Data;
using System.Data.SqlClient;
using DICOMcloud.DataAccess.Database.Schema;

namespace DICOMcloud.DataAccess.Database.Sql
{
    public class SqlObjectArchieveDataAdapter : ObjectArchieveDataAdapter
    {
        public SqlObjectArchieveDataAdapter ( string connectionString ) 
        :  this ( connectionString, new DbSchemaProvider ( ) )
        { 
        }

        public SqlObjectArchieveDataAdapter 
        (   
            string connectionString, 
            DbSchemaProvider schemaProvider 
        ) : base ( schemaProvider )
        { 
            ConnectionString = connectionString ;
        }

        public override IDbConnection CreateConnection ( )
        {
            return new SqlConnection ( ConnectionString ) ;
        }
        
        public string ConnectionString {  get; protected set ; }

        protected override IDbCommand CreateCommand ( )
        { 
            return new System.Data.SqlClient.SqlCommand ( ) ;
        }
        
        protected override IDbDataParameter CreateParameter ( string parameterName, object value )
        {
            return new SqlParameter (  parameterName, value?? System.DBNull.Value ) ;
        }
    }
}
