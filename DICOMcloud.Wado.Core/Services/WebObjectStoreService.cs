using fo = Dicom;
using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DICOMcloud.Pacs;
using DICOMcloud.Media;

using DICOMcloud.DataAccess;
using DICOMcloud;
using DICOMcloud.Messaging;
using Dicom;
using System.Web.Http;
using DICOMcloud.Wado.Core.WadoResponse;
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;


namespace DICOMcloud.Wado
{
    public class WebObjectStoreService : IWebObjectStoreService
    {
        private IObjectStoreService _storageService;
        private IRetrieveUrlProvider _urlProvider ;

        public WebObjectStoreService ( IObjectStoreService storage, IRetrieveUrlProvider urlProvider = null ) 
        {
            _storageService = storage ;
            _urlProvider    = urlProvider ;
        }

        public virtual async Task<WebStoreResponse> Store
        (
            WebStoreRequest request, 
            IStudyId studyId = null
        )
        {
            return await StoreStudy ( request, studyId) ;
        }

        public async virtual Task Delete  ( WebDeleteRequest request )
        {
            await Task.Run( () => { 
                _storageService.Delete ( request.Dataset, request.DeleteLevel )  ;
            });
        }

        protected virtual fo.DicomDataset GetDicom ( Stream dicomStream )
        {
            fo.DicomFile dicom ;


            dicom = fo.DicomFile.Open ( dicomStream, FileReadOption.ReadLargeOnDemand ) ;

            dicom.Dataset.NotValidated();

            return dicom.Dataset ;
        }

        protected virtual InstanceMetadata CreateObjectMetadata ( fo.DicomDataset dataset, WebStoreRequest request )
        {
            return new InstanceMetadata ( ) { } ;
        }
        
        protected virtual IXmlDicomConverter GetXmlConverter()
        {
            return new XmlDicomConverter();
        }

        protected virtual IJsonDicomConverter GetJsonConverter()
        {
            return new JsonDicomConverter();
        }

        protected virtual WebStoreResponse CreateWadoStoreResponseModel(IStudyId studyId)
        {
            return new WebStoreResponse(studyId, _urlProvider);
        }

        private async Task<WebStoreResponse> StoreStudy 
        ( 
            WebStoreRequest request, 
            IStudyId studyId
        )
        {
            WebStoreResponse response = CreateWadoStoreResponseModel (studyId);


            await foreach (var mediaContent in request.GetContents())
            {
                var dicomDs = await CreateDatasetAsync(request.MediaType, mediaContent);
                

                PublisherSubscriberFactory.Instance.Publish(new WebStoreDatasetProcessingMessage(request, dicomDs));

                try
                {
                    var result = _storageService.StoreDicom(dicomDs, CreateObjectMetadata(dicomDs, request));


                    response.AddResult(dicomDs);

                    PublisherSubscriberFactory.Instance.Publish(new WebStoreDatasetProcessedMessage(request, dicomDs));
                }
                catch (Exception ex)
                {
                    response.AddResult(dicomDs, ex);

                    PublisherSubscriberFactory.Instance.Publish(new WebStoreDatasetProcessingFailureMessage(request, dicomDs, ex));
                }
            }

            return response;
        }

        async private Task<DicomDataset> CreateDatasetAsync
        (
            string mediaType, MultipartSection multipartSection
        )
        {
            switch (mediaType)
            {
                case MimeMediaTypes.DICOM:
                {
                    var stream = new MemoryStream();

                    const int chunkSize = 1024;
                    var buffer = new byte[chunkSize];
                    var bytesRead = 0;

                    do
                    {
                        bytesRead = await multipartSection.Body.ReadAsync(buffer, 0, buffer.Length);
                        stream.Write(buffer, 0, bytesRead);

                    } while (bytesRead > 0);

                    stream.Position = 0;
                    return GetDicom(stream);
                }

                //TODO: Zade - need to handle reading the buik data for XML
                case MimeMediaTypes.XmlDicom:
                {
                    var xmlString = await multipartSection.ReadAsStringAsync();
                        
                    return GetXmlConverter().Convert(xmlString);;
                }

                //TODO: Zade - there is no JSON format defined for store yet.
                case MimeMediaTypes.JsonDicom:
                {
                    string jsonString = await multipartSection.ReadAsStringAsync();

                    return GetJsonConverter().Convert(jsonString);
                    
                }

                default:
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
            }
        }

        private delegate DicomDataset GetDicomHandler ( Stream stream ) ;
    }
}
