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

namespace DICOMcloud.DataAccess.Database.UnitTest
{
    [TestClass]
    public class ArchieveDataAccess
    {
        [TestInitialize]
        public void Initialize ( ) 
        {
            Helper = new DicomHelpers ( ) ;
            DataAccessHelper = new DataAccessHelpers ( ) ;
        }

        [TestMethod]
        [TestCategory("Data Access Layer")]
        public void DAL_StoreDelete_Simple ( ) 
        {
            DAL_StoreInstance  ( ) ;
            DAL_DeleteStudy    ( ) ;
            DAL_DeleteSeries   ( ) ;
            DAL_DeleteInstance ( ) ;

            ValidateDatabaseEmpty ( ) ;
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
            var factory = new DicomStoreParameterFactory();
            DicomDataset ds = Helper.GetDicomDataset(0);
            DicomDataset ds2 = Helper.GetDicomDataset(1);
            DicomDataset ds3 = Helper.GetDicomDataset(2);


            DataAccessHelper.DataAccess.StoreInstance(new ObjectId(ds), factory.ProcessDataSet(ds), null);
            DataAccessHelper.DataAccess.StoreInstance(new ObjectId(ds2), factory.ProcessDataSet(ds2), null);
            DataAccessHelper.DataAccess.StoreInstance(new ObjectId(ds3), factory.ProcessDataSet(ds3), null);

            ValidateSopInstanceExists ( ds ) ;
            ValidateSopInstanceExists ( ds2 ) ;
            ValidateSopInstanceExists ( ds3 ) ;
        }

        private void ValidateSopInstanceExists ( DicomDataset ds )
        {
            var queryFactory = new Matching.ConditionFactory();

            queryFactory.BeginProcessingElements();

            queryFactory.ProcessElement(ds.First(n => n.Tag == DicomTag.SOPInstanceUID));

            IEnumerable<DicomDataset> queryDs = DataAccessHelper.DataAccess.Search ( queryFactory.EndProcessingElements ( ),
                                                                                     new QueryOptions ( ), Enum.GetName ( typeof(ObjectQueryLevel),
                                                                                     ObjectQueryLevel.Instance ) ) ;

            Assert.IsNotNull ( queryDs);
            Assert.AreEqual  ( queryDs.Count ( ), 1 ) ;
            Assert.AreEqual  ( queryDs.First ( ).Get<string>(DicomTag.SOPInstanceUID), ds.Get<string>(DicomTag.SOPInstanceUID));
        }

        private void ValidateDatabaseEmpty ( ) 
        {
            IEnumerable<DicomDataset> queryDs = DataAccessHelper.DataAccess.Search ( new List<IMatchingCondition> ( ),
                                                                                     new QueryOptions ( ), Enum.GetName ( typeof(ObjectQueryLevel),
                                                                                     ObjectQueryLevel.Instance ) ) ;
        

            Assert.IsNotNull ( queryDs ) ;
            Assert.AreEqual ( queryDs.Count ( ), 0 );
        }

        DicomHelpers Helper { get; set; }
        DataAccessHelpers  DataAccessHelper { get; set; }
    }
}
