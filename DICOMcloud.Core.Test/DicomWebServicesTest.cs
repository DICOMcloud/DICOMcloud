using System.Net;
using System.Net.Http.Headers;
using Dicom;
using DICOMcloud.Core.Test.Helpers;
using DICOMcloud.DataAccess;
using DICOMcloud.IO;
using DICOMcloud.Media;
using DICOMcloud.Pacs;
using DICOMcloud.Wado;
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Http;
using Moq;
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
        public async Task Web_Storage_Simple()
        {
            DicomDataset[] storeDs = new DicomDataset[]
            {
                DicomHelper.GetDicomDataset(0),
                DicomHelper.GetDicomDataset(1),
                DicomHelper.GetDicomDataset(2)
            };

            // Setting up the mock HttpContext and HttpRequest
            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();

            var headers = new HeaderDictionary();
            var bodyStream = new MemoryStream();
            var response = new Mock<HttpResponse>();
            
            response.Setup(res => res.Body).Returns(new MemoryStream());
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            request.SetupGet(r => r.Headers).Returns(headers);
            request.SetupGet(r => r.Body).Returns(bodyStream);
            request.SetupGet(r => r.ContentType).Returns("multipart/related");
            request.Object.Headers.Add("Accept",   MimeMediaTypes.Json);
            request.Object.Headers.Add("ContentType",  "multipart/related");
            var mimeType = "application/dicom";
            var multiContent = new MultipartContent("related", "DICOM DATA BOUNDARY");
            multiContent.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("type", "\"" + mimeType + "\""));
            
            foreach (var ds in storeDs)
            {
                var dicomFile = new DicomFile(ds);
                var ms = new MemoryStream();

                dicomFile.Save(ms);
                ms.Position = 0;

                var sContent = new StreamContent(ms);
                sContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

                multiContent.Add(sContent);
            }

            // Assigning multipart content to the mock request
            bodyStream = new MemoryStream();
            multiContent.CopyToAsync(bodyStream).Wait();
            bodyStream.Position = 0;
            request.SetupGet(r => r.Body).Returns(bodyStream);
            request.Object.GetTypedHeaders().ContentType = new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("multipart/related");
            request.Object.GetTypedHeaders().ContentType.Parameters.Add(new Microsoft.Net.Http.Headers.NameValueHeaderValue("type", "\"" + mimeType + "\""));
            // Creating WebStoreRequest with the mock HttpRequest
            WebStoreRequest webStoreRequest = new WebStoreRequest(request.Object);
            webStoreRequest.MediaType = MimeMediaTypes.DICOM;
            // Perform the test
            var storeResult = await WebStoreService.Store(webStoreRequest);

            Assert.IsNotNull(storeResult);
            Assert.IsTrue(storeResult.HttpStatus is >= HttpStatusCode.OK and < HttpStatusCode.MultipleChoices);

            ValidateStoredMatchQuery(storeDs);
            Pacs_Delete_Simple();
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
            Assert.IsTrue(result.Result.TotalCount > 0);
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
            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();

            var headers = new HeaderDictionary();
            var bodyStream = new MemoryStream();
            var response = new Mock<HttpResponse>();
            response.Setup(res => res.Body).Returns(new MemoryStream());

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            request.SetupGet(r => r.Headers).Returns(headers);
            request.SetupGet(r => r.Body).Returns(bodyStream);
            request.SetupGet(r => r.ContentType).Returns("multipart/related");
            request.SetupGet(r => r.Host).Returns(new HostString("https://localhost:3000"));
            
            QidoRequestModel requestModel = new QidoRequestModel();
            context.Object.Request.Headers["Accept"] = MimeMediaTypes.Json;
            requestModel.AcceptHeader = request.Object.GetTypedHeaders().Accept;
            requestModel.Headers = request.Object.GetTypedHeaders();
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
