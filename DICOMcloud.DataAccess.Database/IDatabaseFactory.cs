using System.Data;

namespace DICOMcloud.DataAccess.Database
{
    public interface IDatabaseFactory
    {
        IDbConnection CreateConnection ( ) ;
        
        string ConnectionString {  get; }

        IDbCommand CreateCommand ( ) ;
        
        IDbDataParameter CreateParameter ( string parameterName, object value ) ;
    }
}