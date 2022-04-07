using fo = Dicom;
using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DICOMcloud.Pacs;
using DICOMcloud.Media;

using DICOMcloud.DataAccess;
using DICOMcloud;
using DICOMcloud.Messaging;
using Dicom;

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

        public virtual async Task<HttpResponseMessage> Store
        (
            WebStoreRequest request, 
            IStudyId studyId = null
        )
        {
            GetDicomHandler getDicomDelegate = CreateDatasetParser(request);

            if (null != getDicomDelegate)
            {
                var storeResult = await StoreStudy        ( request, studyId, getDicomDelegate ) ;
                var result      = new HttpResponseMessage ( storeResult.HttpStatus ) ;

                
                if (!string.IsNullOrWhiteSpace(storeResult.StatusMessage))
                {
                    result.ReasonPhrase =
                        storeResult.StatusMessage.Length > 512 ?
                        storeResult.StatusMessage.Substring(0, 512) :
                        storeResult.StatusMessage;
                }

                result.Content = CreateResponseContent (request, storeResult);

                return result;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public virtual Task<HttpResponseMessage> Delete  ( WebDeleteRequest request )
        {
            _storageService.Delete ( request.Dataset, request.DeleteLevel )  ;
                
            return Task.FromResult( new HttpResponseMessage ( HttpStatusCode.OK ) ) ;
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

        protected virtual WadoStoreResponse CreateWadoStoreResponseModel(IStudyId studyId)
        {
            return new WadoStoreResponse(studyId, _urlProvider);
        }

        private async Task<WadoStoreResponse> StoreStudy 
        ( 
            WebStoreRequest request, 
            IStudyId studyId, 
            GetDicomHandler getDicom 
        )
        {
            WadoStoreResponse response = CreateWadoStoreResponseModel (studyId);

            foreach (var mediaContent in request.Contents)
            {
                Stream dicomStream = await mediaContent.ReadAsStreamAsync();
                var dicomDs = getDicom(dicomStream);


                PublisherSubscriberFactory.Instance.Publish(this,
                                                              new WebStoreDatasetProcessingMessage(request, dicomDs));

                try
                {
                    var result = _storageService.StoreDicom(dicomDs, CreateObjectMetadata(dicomDs, request));


                    response.AddResult(dicomDs);

                    PublisherSubscriberFactory.Instance.Publish(this,
                                                                  new WebStoreDatasetProcessedMessage(request, dicomDs));
                }
                catch (Exception ex)
                {
                    response.AddResult(dicomDs, ex);

                    PublisherSubscriberFactory.Instance.Publish(this,
                                                                   new WebStoreDatasetProcessingFailureMessage(request, dicomDs, ex));
                }
            }

            return response;
        }

        private GetDicomHandler CreateDatasetParser(WebStoreRequest request)
        {
            GetDicomHandler getDicomDelegate = null ;


            switch (request.MediaType)
            {
                case MimeMediaTypes.DICOM:
                {
                    getDicomDelegate = GetDicom;
                }
                break;

                case MimeMediaTypes.xmlDicom:
                {
                    getDicomDelegate = new GetDicomHandler(delegate (Stream stream)
                    {
                        StreamReader reader = new StreamReader(stream);
                        string xmlString = reader.ReadToEnd();

                        
                        return GetXmlConverter().Convert(xmlString);
                    });
                }
                break;

                case MimeMediaTypes.Json:
                {
                    getDicomDelegate = new GetDicomHandler(delegate (Stream stream)
                    {
                        StreamReader reader = new StreamReader(stream);
                        string jsonString = reader.ReadToEnd();


                        return GetJsonConverter().Convert(jsonString);
                    });
                }
                break;

                default:
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
            }

            return getDicomDelegate;
        }        

        private HttpContent CreateResponseContent
        (
            WebStoreRequest request, 
            WadoStoreResponse storeResult
        )
        {
            HttpContent content;
            //this is not taking the "q" parameter
            if (new MimeMediaType(MimeMediaTypes.Json).IsIn(request.AcceptHeader))
            {
                IJsonDicomConverter converter = GetJsonConverter();

                content = new StringContent(converter.Convert(storeResult.GetResponseContent()),
                                                      System.Text.Encoding.UTF8,
                                                      MimeMediaTypes.Json);
            }
            else
            {
                IXmlDicomConverter xmlConverter = GetXmlConverter();

                content = new StringContent(xmlConverter.Convert(storeResult.GetResponseContent()),
                                                    System.Text.Encoding.UTF8,
                                                    MimeMediaTypes.xmlDicom);
            }

            return content;
        }

        private delegate DicomDataset GetDicomHandler ( Stream stream ) ;
    }
}
