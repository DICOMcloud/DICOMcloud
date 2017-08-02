using DICOMcloud.DataAccess.Database.Sql;

namespace DICOMcloud.DataAccess.UnitTest
{
    public class DataAccessHelpers
    {
        public DataAccessHelpers ( string dbName ) 
        {
            //throw new NotImplementedException ( "specify a connection string below" ) ;
            //TODO: To run the test against a database, uncomment the line below and pass the connection string to your database
            DataAccess = new SqlObjectArchieveDataAccess ( "Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\DB\\" + dbName + ";Initial Catalog=" + dbName + ";Integrated Security=True" ) ;
        }
        
        public IObjectArchieveDataAccess DataAccess { get; set; }
    }
}
