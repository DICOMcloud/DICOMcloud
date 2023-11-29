using System.Net.Http.Headers;

using DICOMcloud.Core.Test.Helpers;
using DICOMcloud.DataAccess;
using DICOMcloud.IO;
using DICOMcloud.Media;
using DICOMcloud.Pacs;
using DICOMcloud.Wado;
using DICOMcloud.Wado.Models;
using FellowOakDicom;
using Microsoft.AspNetCore.Http;
using fo = Dicom;

namespace DICOMcloud.Core.Test
{
    [TestClass]
    public class DicomWebServicesTest
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

            var request = new HttpRequest();
            WebStoreRequest webStoreRequest = new WebStoreRequest(request);

            request.Headers.Accept.Add (new MediaTypeWithQualityHeaderValue(MimeMediaTypes.JsonDicom));
            
            webStoreRequest.MediaType = MimeMediaTypes.DICOM;

            var mimeType = "application/dicom";
            var multiContent = new MultipartContent("related", "DICOM DATA BOUNDARY");

            multiContent.Headers.ContentType?.Parameters.Add(new System.Net.Http.Headers.NameValueHeaderValue("type", "\"" + mimeType + "\""));

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

        // private static QidoRequestModel GetQueryRequest()
        // {
        //     HttpContext.Current = new HttpContext(
        //         new HttpRequest("", "https://localhost:3000", ""),
        //         new HttpResponse(new StringWriter())
        //     );
        //
        //     QidoRequestModel requestModel = new QidoRequestModel();
        //     var headers = new HttpClient().DefaultRequestHeaders;
        //
        //     headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MimeMediaTypes.Json));
        //     requestModel.AcceptHeader = headers.Accept;
        //     requestModel.Headers = headers;
        //     requestModel.Query = new QidoQuery();
        //     return requestModel;
        // }
         
        private static QidoRequestModel GetQueryRequest()
        {
            var context = new DefaultHttpContext
            {
                Request =
                {
                    Scheme = "https",
                    Host = new HostString("localhost:3000"),
                    PathBase = "",
                    Path = "/"
                },
                Response =
                {
                    Body = new MemoryStream()
                }
            };

            // If you need to simulate other parts of the HttpContext, you can modify the context object here

            QidoRequestModel requestModel = new QidoRequestModel();
            //var headers = new HttpClient().DefaultRequestHeaders;
            HeaderDictionary headersDic = new HeaderDictionary
            {
                { "Accept", new Microsoft.Extensions.Primitives.StringValues(MimeMediaTypes.JsonDicom) }
            };
            var headers = new Microsoft.AspNetCore.Http.Headers.RequestHeaders(headersDic);

            requestModel.AcceptHeader = headers.Accept;
            requestModel.Headers = headers;
            requestModel.Query = new QidoQuery();

            // If you need to use the simulated HttpContext in your tests, you can set it to the requestModel or any other object
            // requestModel.HttpContext = context; 

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
            var study1    = GetUidElement ( DicomTag.StudyInstanceUID, DicomHelper.Study1UID) ;
            var study2    = GetUidElement ( DicomTag.StudyInstanceUID, DicomHelper.Study2UID) ;
            var study3    = GetUidElement ( DicomTag.StudyInstanceUID, DicomHelper.Study3UID) ;
            var series2   = GetUidElement ( DicomTag.SeriesInstanceUID, DicomHelper.Series2UID) ;
            var series3   = GetUidElement ( DicomTag.SeriesInstanceUID, DicomHelper.Series3UID) ;
            var instance3 = GetUidElement ( DicomTag.SOPInstanceUID, DicomHelper.Instance3UID) ;

            StoreService.Delete ( new DicomDataset ( study1 ), ObjectQueryLevel.Study ) ;
            StoreService.Delete ( new DicomDataset ( study2, series2 ), ObjectQueryLevel.Series ) ;
            StoreService.Delete ( new DicomDataset ( study3, series3, instance3 ), ObjectQueryLevel.Instance ) ;
        }

        private DicomUniqueIdentifier GetUidElement (DicomTag tag, string uid )
        {
            return new DicomUniqueIdentifier ( tag, uid ) ;
        }


        private DicomHelpers        DicomHelper      { get; set; }
        private DataAccessHelpers   DataAccessHelper { get; set; }
        private IObjectStoreService StoreService     { get; set; } 

        private IWebObjectStoreService WebStoreService;
        private IQidoRsService  WebQueryService;

    }
}
