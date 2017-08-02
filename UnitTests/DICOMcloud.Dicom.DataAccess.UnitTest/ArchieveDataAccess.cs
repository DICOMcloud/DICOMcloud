using System;
using DICOMcloud;
using fo = Dicom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DICOMcloud.UnitTest;
using DICOMcloud.DataAccess.UnitTest;

namespace DICOMcloud.DataAccess.UnitTest
{
    [TestClass]
    public class ArchieveDataAccess
    {
        [TestInitialize]
        public void Initialize ( ) 
        {
            Helper = new DicomHelpers ( ) ;
            DataAccessHelper = new DataAccessHelpers ( "DICOMcloud_DAL_UnitTest.mdf" ) ;
        }

        [TestMethod]
        [TestCategory("Data Access Layer")]
        public void DAL_StoreDelete_Simple ( ) 
        {
            DAL_StoreInstance  ( ) ;
            DAL_DeleteStudy    ( ) ;
            DAL_DeleteSeries   ( ) ;
            DAL_DeleteInstance ( ) ;
        }

        private void DAL_DeleteStudy ( )
        {
            DataAccessHelper.DataAccess.DeleteStudy ( new ObjectId ( ) { StudyInstanceUID = Helper.Study1UID } ) ;    
        }

        private void DAL_DeleteSeries ( )
        {
            DataAccessHelper.DataAccess.DeleteSeries ( new ObjectId ( ) { StudyInstanceUID = Helper.Study2UID, SeriesInstanceUID = Helper.Series2UID } ) ;    
        }
        
        private void DAL_DeleteInstance ( )
        {
            DataAccessHelper.DataAccess.DeleteInstance ( new ObjectId ( ) { StudyInstanceUID = Helper.Study1UID, SeriesInstanceUID = Helper.Series1UID, SOPInstanceUID = Helper.Instance3UID } ) ;    
        }

        private void DAL_StoreInstance ( )
        {
            var factory = new DicomStoreParameterFactory ( ) ;
            var ds  = Helper.GetDicomDataset ( 0 ) ;
            var ds2 = Helper.GetDicomDataset ( 1 ) ;
            var ds3 = Helper.GetDicomDataset ( 2 ) ;


            DataAccessHelper.DataAccess.StoreInstance ( new ObjectId (ds), factory.ProcessDataSet ( ds ), null ) ;    
            DataAccessHelper.DataAccess.StoreInstance ( new ObjectId (ds2), factory.ProcessDataSet ( ds2 ), null ) ;    
            DataAccessHelper.DataAccess.StoreInstance ( new ObjectId (ds3), factory.ProcessDataSet ( ds3 ), null ) ;    
        }
        
        
        DicomHelpers Helper { get; set; }
        DataAccessHelpers  DataAccessHelper { get; set; }
    }
}
