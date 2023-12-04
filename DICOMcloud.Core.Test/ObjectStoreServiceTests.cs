
using DICOMcloud.Core.Test.Helpers;
using DICOMcloud.DataAccess;
using DICOMcloud.IO;
using DICOMcloud.Media;
using DICOMcloud.Pacs;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.NativeCodec;
using Microsoft.Extensions.DependencyInjection;

namespace DICOMcloud.Core.Test
{
    [TestClass]
    public class ObjectStoreServiceTests
    {
        [TestInitialize]
        public void Initialize ( ) 
        {
            DicomHelper        = new DicomHelpers ( ) ;
            DataAccessHelper   = new DataAccessHelpers ( ) ;
            var storagePath    = DicomHelpers.GetTestDataFolder ( "storage", true ) ;
            var mediaIdFactory = new DicomMediaIdFactory ( ) ;


            MediaStorageService storageService = new FileStorageService ( storagePath ) ;
            
            var factory = new Pacs.Commands.DCloudCommandFactory( storageService,
                                                             DataAccessHelper.DataAccess,
                                                             new DicomMediaWriterFactory ( storageService, 
                                                                                           mediaIdFactory  ),
                                                             mediaIdFactory ) ;
            
            StoreService = new ObjectStoreService ( factory ) ;
        }

        [TestCleanup]
        public void Cleanup ( )
        {
            DataAccessHelper.EmptyDatabase ( ) ;
        }

        [TestMethod]
        public void Pacs_Storage_Simple ( )
        {
            DicomDataset[] storeDs = new DicomDataset[] 
            { 
                DicomHelper.GetDicomDataset (0),
                DicomHelper.GetDicomDataset (1),
                DicomHelper.GetDicomDataset (2)
            };

            StoreService.StoreDicom (storeDs[0], new DataAccess.InstanceMetadata ( ) ) ;
            StoreService.StoreDicom (storeDs[1], new DataAccess.InstanceMetadata ( ) ) ;
            StoreService.StoreDicom (storeDs[2], new DataAccess.InstanceMetadata ( ) ) ;

            ValidateStoredMatchQuery (storeDs);

            Pacs_Delete_Simple ( ) ;
        }

        private void ValidateStoredMatchQuery(DicomDataset[] storedDs)
        {
            var queryDs = DicomHelper.GetQueryDataset ( ) ;
            var queryFactory = new DataAccess.Matching.ConditionFactory();
            var matchingElements = queryFactory.ProcessDataSet (queryDs);

            var results = DataAccessHelper.DataAccess.Search ( matchingElements, 
                                                               new QueryOptions ( ), 
                                                               DICOMcloud.ObjectQueryLevelConstants.Instance );
        
            foreach ( var ds in results)
            { 
                var sopUid = ds.GetSingleValue<string> (DicomTag.SOPInstanceUID);

                var stored = storedDs.FirstOrDefault (n => n.GetSingleValue<string> (DicomTag.SOPInstanceUID) == sopUid);
            
                Assert.IsNotNull (stored);

                foreach ( var element in stored)
                { 
                    Assert.AreEqual ( stored.GetSingleValue<string> (element.Tag), ds.GetSingleValue<string> (element.Tag));
                }
            }
        }

        protected virtual void EnsureCodecsLoaded ( ) 
        {
            var path = Environment.CurrentDirectory ; //System.IO.Path.Combine ( System.Web.Hosting.HostingEnvironment.MapPath ( "~/" ), "bin" );

            System.Diagnostics.Trace.TraceInformation ( "Path: " + path );

            new DicomSetupBuilder().RegisterServices(s =>
                s.AddFellowOakDicom()
                .AddLogging()
                .AddTranscoderManager<NativeTranscoderManager >()
                .AddImageManager<ImageSharpImageManager>())
                .SkipValidation()
                .Build();

             //FellowOakDicom.Imaging.Codec.TranscoderManager. Imaging.Codec.TranscoderManager.LoadCodecs ( path ) ;
        }

        [TestMethod]
        public void Pacs_Storage_Images ( )
        {
            EnsureCodecsLoaded ( ) ;

            StoreService.StoreDicom ( DicomHelper.GetDicomDataset (2), new DataAccess.InstanceMetadata ( ) ) ;

            int counter = 0 ;
            
            foreach ( string file in Directory.GetFiles (DicomHelpers.GetSampleImagesFolder ( ) ) )
            {
                var dataset =  DicomFile.Open ( file ).Dataset ;

                //reason is to shorten the path where the DS is stored. 
                //location include the UIDs, so make sure your storage
                // folder is close to the root when keeping the original UIDs
                dataset.AddOrUpdate (  DicomTag.PatientID, "Patient_" + counter ) ;
                dataset.AddOrUpdate (  DicomTag.StudyInstanceUID, "1112." + counter ) ;
                dataset.AddOrUpdate (  DicomTag.SeriesInstanceUID, "1113." + counter ) ;
                dataset.AddOrUpdate (  DicomTag.SOPInstanceUID, "1114." + counter ) ;
                
                StoreService.StoreDicom ( dataset, new DataAccess.InstanceMetadata ( ) ) ;

                counter++ ;    
            }
        }

        private void Pacs_Delete_Simple ()
        {
            var study1    = GetUidElement (  DicomTag.StudyInstanceUID, DicomHelper.Study1UID) ;
            var study2    = GetUidElement (  DicomTag.StudyInstanceUID, DicomHelper.Study2UID) ;
            var study3    = GetUidElement (  DicomTag.StudyInstanceUID, DicomHelper.Study3UID) ;
            var series2   = GetUidElement (  DicomTag.SeriesInstanceUID, DicomHelper.Series2UID) ;
            var series3   = GetUidElement (  DicomTag.SeriesInstanceUID, DicomHelper.Series3UID) ;
            var instance3 = GetUidElement (  DicomTag.SOPInstanceUID, DicomHelper.Instance3UID) ;

            StoreService.Delete ( new  DicomDataset ( study1 ), ObjectQueryLevel.Study ) ;
            StoreService.Delete ( new  DicomDataset ( study2, series2 ), ObjectQueryLevel.Series ) ;
            StoreService.Delete ( new  DicomDataset ( study3, series3, instance3 ), ObjectQueryLevel.Instance ) ;
        }

        private  DicomUniqueIdentifier GetUidElement ( DicomTag tag, string uid )
        {
            return new  DicomUniqueIdentifier ( tag, uid ) ;
        }


        private DicomHelpers        DicomHelper      { get; set; }
        private DataAccessHelpers   DataAccessHelper { get; set; }
        private IObjectStoreService StoreService     { get; set; } 
    }
}
