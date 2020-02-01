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
using DICOMcloud.Pacs;
using Dicom;
using DICOMcloud.DataAccess;
using System.Collections.Generic;
using DICOMcloud.Wado;
using DICOMcloud.Wado.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Threading.Tasks;

namespace DICOMcloud.UnitTest
{
    [TestClass]
    public class DICOMwebServicesTest
    {
        [TestInitialize]
        public void Initialize ( ) 
        {
            DicomHelper        = new DicomHelpers ( ) ;
            DataAccessHelper   = new DataAccessHelpers ( ) ;
            var storagePath    = DicomHelpers.GetTestDataFolder ( "storage", true ) ;
            var mediaIdFactory = new DicomMediaIdFactory ( ) ;


            MediaStorageService storageService = new FileStorageService ( storagePath ) ;
            IObjectArchieveQueryService queryService = new ObjectArchieveQueryService(DataAccessHelper.DataAccess);

            var factory = new Pacs.Commands.DCloudCommandFactory( storageService,
                                                             DataAccessHelper.DataAccess,
                                                             new DicomMediaWriterFactory ( storageService, 
                                                                                           mediaIdFactory  ),
                                                             mediaIdFactory ) ;
            
            StoreService = new ObjectStoreService ( factory ) ;

            var urlProvider = new MockRetrieveUrlProvider();
            WebStoreService = new WebObjectStoreService(StoreService,urlProvider);
            WebQueryService = new QidoRsService(queryService, mediaIdFactory, storageService);
        }

        [TestCleanup]
        public void Cleanup ( )
        {
            DataAccessHelper.EmptyDatabase ( ) ;
        }

        [TestMethod]
        public async Task Web_Storage_Simple ( )
        {
            DicomDataset[] storeDs = new DicomDataset[] 
            { 
                DicomHelper.GetDicomDataset (0),
                DicomHelper.GetDicomDataset (1),
                DicomHelper.GetDicomDataset (2)
            };

            var request = new HttpRequestMessage();
            WebStoreRequest webStoreRequest = new WebStoreRequest(request);

            request.Headers.Accept.Add (new MediaTypeWithQualityHeaderValue(MimeMediaTypes.Json));
            
            webStoreRequest.MediaType = MimeMediaTypes.DICOM;

            var mimeType = "application/dicom";
            var multiContent = new MultipartContent("related", "DICOM DATA BOUNDARY");

            multiContent.Headers.ContentType.Parameters.Add(new System.Net.Http.Headers.NameValueHeaderValue("type", "\"" + mimeType + "\""));

            foreach (var ds in storeDs)
            {
                DicomFile dicomFile = new DicomFile(ds);
                MemoryStream ms = new MemoryStream ();
            
                dicomFile.Save(ms);
                ms.Position = 0;
            
                StreamContent sContent = new StreamContent(ms);

                sContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);

                multiContent.Add(sContent);
                webStoreRequest.Request.Content = multiContent;
                webStoreRequest.Contents.Add(sContent);
            }

            var storeResult = await WebStoreService.Store(webStoreRequest);

            Assert.IsNotNull(storeResult);
            Assert.IsTrue(storeResult.IsSuccessStatusCode);

            ValidateStoredMatchQuery (storeDs);

            Pacs_Delete_Simple ( ) ;
        }

        [TestMethod]
        public void Web_SearchForStudies_SequenceTest( )
        {
            QidoRequestModel requestModel = GetQueryRequest();


            //Include a sequence on its own "PerformedProtocolCodeSequence"
            requestModel.Query.IncludeElements.Add("00400260");
                                             //RequestAttributeSequence
            //Include a sequence with element "RequestAttributesSequence.ScheduledProcedureStepID"
            requestModel.Query.IncludeElements.Add("00400275.00400009");
            var result = WebQueryService.SearchForStudies(requestModel);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessStatusCode);
        }

        private static QidoRequestModel GetQueryRequest()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "https://localhost:3000", ""),
                new HttpResponse(new StringWriter())
            );

            QidoRequestModel requestModel = new QidoRequestModel();
            var headers = new HttpClient().DefaultRequestHeaders;

            headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MimeMediaTypes.Json));
            requestModel.AcceptHeader = headers.Accept;
            requestModel.Headers = headers;
            requestModel.Query = new QidoQuery();
            return requestModel;
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

        private void Pacs_Delete_Simple ()
        {
            var study1    = GetUidElement ( fo.DicomTag.StudyInstanceUID, DicomHelper.Study1UID) ;
            var study2    = GetUidElement ( fo.DicomTag.StudyInstanceUID, DicomHelper.Study2UID) ;
            var study3    = GetUidElement ( fo.DicomTag.StudyInstanceUID, DicomHelper.Study3UID) ;
            var series2   = GetUidElement ( fo.DicomTag.SeriesInstanceUID, DicomHelper.Series2UID) ;
            var series3   = GetUidElement ( fo.DicomTag.SeriesInstanceUID, DicomHelper.Series3UID) ;
            var instance3 = GetUidElement ( fo.DicomTag.SOPInstanceUID, DicomHelper.Instance3UID) ;

            StoreService.Delete ( new fo.DicomDataset ( study1 ), ObjectQueryLevel.Study ) ;
            StoreService.Delete ( new fo.DicomDataset ( study2, series2 ), ObjectQueryLevel.Series ) ;
            StoreService.Delete ( new fo.DicomDataset ( study3, series3, instance3 ), ObjectQueryLevel.Instance ) ;
        }

        private fo.DicomUniqueIdentifier GetUidElement (fo.DicomTag tag, string uid )
        {
            return new fo.DicomUniqueIdentifier ( tag, uid ) ;
        }


        private DicomHelpers        DicomHelper      { get; set; }
        private DataAccessHelpers   DataAccessHelper { get; set; }
        private IObjectStoreService StoreService     { get; set; } 

        private IWebObjectStoreService WebStoreService;
        private IQidoRsService  WebQueryService;

    }
}
