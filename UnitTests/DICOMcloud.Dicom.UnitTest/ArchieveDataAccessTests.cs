using System;
using DICOMcloud;
using fo = Dicom;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DICOMcloud.UnitTest;
using DICOMcloud.DataAccess.UnitTest;
using Dicom;
using System.Collections.Generic;
using DICOMcloud.DataAccess.Matching;
using DICOMcloud.DataAccess;

namespace DICOMcloud.UnitTest
{
    [TestClass]
    public class ArchieveDataAccessTests
    {
        [TestInitialize]
        public void Initialize ( ) 
        {
            Helper = new DicomHelpers ( ) ;
            DataAccessHelper = new DataAccessHelpers ( ) ;
        }

        [TestCleanup]
        public void Cleanup ( )
        {
            DataAccessHelper.EmptyDatabase ( ) ;
        }

        [TestMethod]
        [TestCategory("Data Access Layer")]
        public void DAL_StoreDelete_Simple ( ) 
        {
            DAL_StoreInstance  (   ) ; //we store 2 studies, 3 series and 3 instances
            ValidateStudyCount ( 2 ) ; 
            
            DAL_DeleteInstance ( ) ;
            ValidateStudyCount ( 2 ) ;

            DAL_DeleteSeries ( ) ;
            ValidateStudyCount ( 1 ) ;
            
            DAL_DeleteStudy ( );
            ValidateStudyCount ( 0 );
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
            DataAccessHelper.DataAccess.DeleteInstance ( new ObjectId ( ) { StudyInstanceUID = Helper.Study3UID, SeriesInstanceUID = Helper.Series3UID, SOPInstanceUID = Helper.Instance3UID } ) ;    
        }

        private void DAL_StoreInstance ( )
        {
            var factory = new DicomStoreParameterFactory();
            DicomDataset ds = Helper.GetDicomDataset(0);
            DicomDataset ds2 = Helper.GetDicomDataset(1);
            DicomDataset ds3 = Helper.GetDicomDataset(2);


            DataAccessHelper.DataAccess.StoreInstance(DicomObjectIdFactory.Instance.CreateObjectId (ds), factory.ProcessDataSet(ds), null);
            DataAccessHelper.DataAccess.StoreInstance(DicomObjectIdFactory.Instance.CreateObjectId (ds2), factory.ProcessDataSet(ds2), null);
            DataAccessHelper.DataAccess.StoreInstance(DicomObjectIdFactory.Instance.CreateObjectId (ds3), factory.ProcessDataSet(ds3), null);

            ValidateSopInstanceExists ( ds ) ;
            ValidateSopInstanceExists ( ds2 ) ;
            ValidateSopInstanceExists ( ds3 ) ;
        }

        private void ValidateSopInstanceExists ( DicomDataset ds )
        {
            var queryFactory = new DataAccess.Matching.ConditionFactory();

            queryFactory.BeginProcessingElements();

            queryFactory.ProcessElement(ds.First(n => n.Tag == DicomTag.SOPInstanceUID));

            IEnumerable<DicomDataset> queryDs = DataAccessHelper.DataAccess.Search ( queryFactory.EndProcessingElements ( ),
                                                                                     new QueryOptions ( ), Enum.GetName ( typeof(ObjectQueryLevel),
                                                                                     ObjectQueryLevel.Instance ) ) ;

            Assert.IsNotNull ( queryDs);
            Assert.AreEqual  ( queryDs.Count ( ), 1 ) ;
            Assert.AreEqual  ( ds.GetSingleValue<string>(DicomTag.SOPInstanceUID), queryDs.First ( ).GetSingleValue<string>(DicomTag.SOPInstanceUID));
        }

        private void ValidateStudyCount ( int studies ) 
        {
            IEnumerable<DicomDataset> queryDs = DataAccessHelper.DataAccess.Search ( new List<IMatchingCondition> ( ),
                                                                                     new QueryOptions ( ), 
                                                                                     Enum.GetName ( typeof(ObjectQueryLevel), ObjectQueryLevel.Study ) ) ;
        
            Assert.IsNotNull ( queryDs ) ;
            Assert.AreEqual ( studies, queryDs.Count ( ) );
        }

        DicomHelpers Helper { get; set; }
        DataAccessHelpers  DataAccessHelper { get; set; }
    }
}
