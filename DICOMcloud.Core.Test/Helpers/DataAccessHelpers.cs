
using DICOMcloud.DataAccess.Database;
using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.DataAccess.Matching;
using DICOMcloud.DataAccess;
using FellowOakDicom;

namespace DICOMcloud.Core.Test.Helpers
{
    public class DataAccessHelpers
    {
        private string DbName;

        public DataAccessHelpers()
            : this("DICOMcloud.mdf")
        { }
        
        private DataAccessHelpers ( string dbName ) 
        {
            DbName = dbName;

            string dbPath = Path.Combine(DicomHelpers.GetSampleDatabaseFolder(), dbName);

            DbSchemaProvider schemaProvider = new StorageDbSchemaProvider ( ) ;
            string connectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=" + dbPath + ";Initial Catalog=" + dbName + ";Integrated Security=True" ;
           
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
