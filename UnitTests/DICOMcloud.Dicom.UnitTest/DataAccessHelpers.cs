using System;
using System.Collections.Generic;
using Dicom;
using DICOMcloud.DataAccess.Database.Sql;
using DICOMcloud.DataAccess.Matching;

namespace DICOMcloud.DataAccess.UnitTest
{
    public class DataAccessHelpers
    {
        public DataAccessHelpers ( ) 
        : this ( "DICOMcloud_UnitTest.mdf" )
        {}

        private DataAccessHelpers ( string dbName ) 
        {
            //throw new NotImplementedException ( "specify a connection string below" ) ;
            //TODO: To run the test against a database, uncomment the line below and pass the connection string to your database
            DataAccess = new SqlObjectArchieveDataAccess ( "Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\DB\\" + dbName + ";Initial Catalog=" + dbName + ";Integrated Security=True" ) ;
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
