using System.Data;
using System.Data.SqlClient;

namespace DICOMcloud.DataAccess.Database
{
    public class SqlDatabaseFactory : IDatabaseFactory
    {
        public SqlDatabaseFactory ( IConnectionStringProvider connectionStringProvider )
        {
            ConnectionString = connectionStringProvider.ConnectionString ;
        }

        public SqlDatabaseFactory ( string connectionString )
        {
            ConnectionString = connectionString ;
        }

        public virtual IDbConnection CreateConnection ( )
        {
            return new SqlConnection ( ConnectionString ) ;
        }
        
        public string ConnectionString {  get; protected set ; }

        public virtual IDbCommand CreateCommand ( )
        { 
            return new System.Data.SqlClient.SqlCommand ( ) ;
        }
        
        public virtual IDbDataParameter CreateParameter ( string parameterName, object value )
        {
            return new SqlParameter (  parameterName, value?? System.DBNull.Value ) ;
        }
    }
}