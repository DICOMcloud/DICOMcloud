using System;
using System.IO;
using System.Linq;
using DICOMcloud.DataAccess.UnitTest;
using DICOMcloud.UnitTest;
using fo = Dicom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DICOMcloud.Pacs.Commands;
using DICOMcloud.IO;
using DICOMcloud.Media;

namespace DICOMcloud.Pacs.UnitTest
{
    [TestClass]
    public class ObjectStoreServiceTest
    {
        [TestInitialize]
        public void Initialize ( ) 
        {
            DicomHelper        = new DicomHelpers ( ) ;
            DataAccessHelper   = new DataAccessHelpers ( "DICOMcloud_PACS_UnitTest.mdf" ) ;
            var storagePath    = DicomHelpers.GetTestDataFolder ( "storage", true ) ;
            var mediaIdFactory = new DicomMediaIdFactory ( ) ;


            MediaStorageService storageService = new FileStorageService ( storagePath ) ;
            
            var factory = new Commands.DCloudCommandFactory( storageService,
                                                             DataAccessHelper.DataAccess,
                                                             new DicomMediaWriterFactory ( storageService, 
                                                                                           mediaIdFactory  ),
                                                             mediaIdFactory ) ;
            
            StoreService = new ObjectStoreService ( factory ) ;
        }

        [TestMethod]
        public void Pacs_Storage_Simple ( )
        {
            Assert.AreEqual ( StoreService.StoreDicom ( DicomHelper.GetDicomDataset (0), new DataAccess.InstanceMetadata ( ) ).Status, CommandStatus.Success ) ;
            Assert.AreEqual ( StoreService.StoreDicom ( DicomHelper.GetDicomDataset (1), new DataAccess.InstanceMetadata ( ) ).Status, CommandStatus.Success ) ;
            Assert.AreEqual ( StoreService.StoreDicom ( DicomHelper.GetDicomDataset (2), new DataAccess.InstanceMetadata ( ) ).Status, CommandStatus.Success ) ;

            Pacs_Delete_Simple ( ) ;
        }
        protected virtual void EnsureCodecsLoaded ( ) 
        {
            var path = Environment.CurrentDirectory ; //System.IO.Path.Combine ( System.Web.Hosting.HostingEnvironment.MapPath ( "~/" ), "bin" );

            System.Diagnostics.Trace.TraceInformation ( "Path: " + path );

            fo.Imaging.Codec.TranscoderManager.LoadCodecs ( path ) ;
        }

        [TestMethod]
        public void Pacs_Storage_Images ( )
        {
            EnsureCodecsLoaded ( ) ;

            Assert.AreEqual ( StoreService.StoreDicom ( DicomHelper.GetDicomDataset (2), new DataAccess.InstanceMetadata ( ) ).Status, CommandStatus.Success ) ;

            int counter = 0 ;
            
            foreach ( string file in Directory.GetFiles (DicomHelpers.GetSampleImagesFolder ( ) ) )
            {
                var dataset = fo.DicomFile.Open ( file ).Dataset ;

                //reason is to shorten the path where the DS is stored. 
                //location include the UIDs, so make sure your storage
                // folder is close to the root when keeping the original UIDs
                dataset.AddOrUpdate ( fo.DicomTag.PatientID, "Patient_" + counter ) ;
                dataset.AddOrUpdate ( fo.DicomTag.StudyInstanceUID, "Study_" + counter ) ;
                dataset.AddOrUpdate ( fo.DicomTag.SeriesInstanceUID, "Series_" + counter ) ;
                dataset.AddOrUpdate ( fo.DicomTag.SOPInstanceUID, "Instance_" + counter ) ;
                
                Assert.AreEqual ( StoreService.StoreDicom ( dataset, new DataAccess.InstanceMetadata ( ) ).Status, CommandStatus.Success ) ;

                //Assert.AreEqual ( StoreService.Delete ( dataset, Dicom.ObjectLevel.Instance ).Status, CommandStatus.Success ) ;
            
                counter++ ;    
            }
        }

        private void Pacs_Delete_Simple ( )
        {
            var study1    = GetUidElement ( fo.DicomTag.StudyInstanceUID, DicomHelper.Study1UID) ;
            var study2    = GetUidElement ( fo.DicomTag.StudyInstanceUID, DicomHelper.Study2UID) ;
            var study3    = GetUidElement ( fo.DicomTag.StudyInstanceUID, DicomHelper.Study3UID) ;
            var series2   = GetUidElement ( fo.DicomTag.SeriesInstanceUID, DicomHelper.Series2UID) ;
            var series3   = GetUidElement ( fo.DicomTag.SeriesInstanceUID, DicomHelper.Series3UID) ;
            var instance3 = GetUidElement ( fo.DicomTag.SOPInstanceUID, DicomHelper.Instance3UID) ;

            var deleteStudyResult    = StoreService.Delete ( new fo.DicomDataset ( study1 ), ObjectLevel.Study ) ;
            var deleteSeriesResult   = StoreService.Delete ( new fo.DicomDataset ( study2, series2 ), ObjectLevel.Series ) ;
            var deleteInstanceResult = StoreService.Delete ( new fo.DicomDataset ( study3, series3, instance3 ), ObjectLevel.Instance ) ;
            
            Assert.AreEqual ( CommandStatus.Success, deleteStudyResult.Status, deleteStudyResult.Message ) ;
            Assert.AreEqual ( CommandStatus.Success, deleteSeriesResult.Status, deleteSeriesResult.Message ) ;
            Assert.AreEqual ( CommandStatus.Success, deleteInstanceResult.Status, deleteInstanceResult.Message ) ;
        }

        private fo.DicomUniqueIdentifier GetUidElement (fo.DicomTag tag, string uid )
        {
            return new fo.DicomUniqueIdentifier ( tag, uid ) ;
        }


        private DicomHelpers        DicomHelper      { get; set; }
        private DataAccessHelpers   DataAccessHelper { get; set; }
        private IObjectStoreService StoreService     { get; set; } 
    }
}
