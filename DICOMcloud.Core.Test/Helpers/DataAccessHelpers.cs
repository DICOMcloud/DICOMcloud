
using DICOMcloud.DataAccess.Database;
using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.DataAccess.Matching;
using DICOMcloud.DataAccess;
using FellowOakDicom;

namespace DICOMcloud.Core.Test.Helpers
{
    public class DataAccessHelpers
    {
        public DataAccessHelpers()
            : this("DICOMcloud.mdf")
        { }
        
        private DataAccessHelpers ( string dbName ) 
        {
             string dbPath = "L:\\Projects\\DICOMcloud.Core\\DICOMcloud.Wado.WebApi.Core\\App_Data\\DB\\" + dbName;
             // string dbPath = "C:\\source\\repos\\DICOMcloud-git\\DICOMcloud.Wado.WebApi\\App_Data\\DB\\" + dbName;
            DbSchemaProvider schemaProvider = new StorageDbSchemaProvider ( ) ;
            string connectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=" + dbPath + ";Initial Catalog=" + dbName + ";Integrated Security=True" ;
           //throw new NotImplementedException ( "specify a connection string below" ) ;
            //TODO: To run the test against a database, uncomment the line below and pass the connection string to your database
            DataAccess = new ObjectArchieveDataAccess ( schemaProvider,
                                                        new  ObjectArchieveDataAdapter ( schemaProvider, new SqlDatabaseFactory (connectionString))) ;
        }

        internal void EmptyDatabase()
        {
            IEnumerable<DicomDataset> queryDs = DataAccess.Search ( new List<IMatchingCondition> ( ),
                                                                    new QueryOptions ( ), 
                                                                    Enum.GetName ( typeof(ObjectQueryLevel), ObjectQueryLevel.Study ) ) ;
        
            foreach ( var ds in queryDs )
            {
                var study = DicomObjectIdFactory.Instance.CreateObjectId (ds) ;
            
                
                DataAccess.DeleteStudy ( study ) ;
            }
        }

        public IObjectArchieveDataAccess DataAccess { get; set; }
    }
}
