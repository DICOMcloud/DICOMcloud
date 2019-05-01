using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DICOMcloud.DataAccess.Database
{
    public class MySQLDatabaseFactory : IDatabaseFactory
    {
        public MySQLDatabaseFactory( IConnectionStringProvider connectionStringProvider )
        {
            ConnectionString = connectionStringProvider.ConnectionString ;
        }

        public MySQLDatabaseFactory( string connectionString )
        {
            ConnectionString = connectionString ;
        }

        public virtual IDbConnection CreateConnection ( )
        {
            return new MySqlConnection ( ConnectionString ) ;
        }
        
        public string ConnectionString {  get; protected set ; }

        public virtual IDbCommand CreateCommand ( )
        { 
            return new MySqlCommand ( ) ;
        }
        
        public virtual IDbDataParameter CreateParameter ( string parameterName, object value )
        {
            return new MySqlParameter (  parameterName, value?? System.DBNull.Value ) ;
        }
    }
}