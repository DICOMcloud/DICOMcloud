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
        private IRetieveUrlProvider _urlProvider ;
        //public WebObjectStoreService ( ) : this ( new ObjectStoreDataService ( ) ) {}
        public WebObjectStoreService ( IObjectStoreService storage, IRetieveUrlProvider urlProvider = null ) 
        {
            _storageService = storage ;
            _urlProvider    = urlProvider ;
        }

        public virtual async Task<HttpResponseMessage> Store
        (
            WebStoreRequest request, 
            string studyInstanceUID 
        )
        {
            GetDicomHandler getDicomDelegate = CreateDatasetParser(request);

            if (null != getDicomDelegate)
            {
                var storeResult = await StoreStudy(request, studyInstanceUID, getDicomDelegate);
                var result      = new HttpResponseMessage(storeResult.HttpStatus);


                //this is not taking the "q" parameter
                if (new MimeMediaType(MimeMediaTypes.Json).IsIn(request.AcceptHeader))
                {
                    IJsonDicomConverter converter = GetJsonConverter();

                    result.Content = new StringContent(converter.Convert(storeResult.GetResponseContent()),
                                                          System.Text.Encoding.UTF8,
                                                          MimeMediaTypes.Json);
                }
                else
                {
                    IXmlDicomConverter xmlConverter = GetXmlConverter();

                    result.Content = new StringContent(xmlConverter.Convert(storeResult.GetResponseContent()),
                                                        System.Text.Encoding.UTF8,
                                                        MimeMediaTypes.xmlDicom);
                }

                return result;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public virtual Task<HttpResponseMessage> Delete  ( WebDeleteRequest request )
        {
            try
            {
                _storageService.Delete ( request.Dataset, request.DeleteLevel )  ;
                
                return Task.FromResult( new HttpResponseMessage ( HttpStatusCode.OK ) ) ;
            }
            catch ( Exception ex )
            {
                return Task.FromResult( new HttpResponseMessage ( HttpStatusCode.InternalServerError ) ) ;
            }
        }

        //TODO: uncomment
        //public virtual async Task<HttpResponseMessage> ProcessRequest ( WebDeleteRequest request )
        //{
        //}

        protected virtual fo.DicomDataset GetDicom ( Stream dicomStream )
        {
            fo.DicomFile dicom ;


            dicom = fo.DicomFile.Open ( dicomStream ) ;

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

        private async Task<WadoStoreResponse> StoreStudy ( WebStoreRequest request, string studyInstanceUID, GetDicomHandler getDicom )
        {
            WadoStoreResponse response = new WadoStoreResponse(studyInstanceUID, _urlProvider);

            foreach (var mediaContent in request.Contents)
            {
                Stream dicomStream = await mediaContent.ReadAsStreamAsync();
                var    dicomDs     = getDicom ( dicomStream ) ; 

                PublisherSubscriberFactory.Instance.Publish ( this, new WebStoreDatasetProcessingMessage ( request, dicomDs ) ) ;
                
                try
                {
                    var result = _storageService.StoreDicom(dicomDs, CreateObjectMetadata (dicomDs, request));

                    
                    response.AddResult(dicomDs);

                    PublisherSubscriberFactory.Instance.Publish ( this, new WebStoreDatasetProcessedMessage ( request, dicomDs ) ) ;
                }
                catch (Exception ex)
                {
                    response.AddResult(dicomDs, ex);

                    PublisherSubscriberFactory.Instance.Publish ( this, new WebStoreDatasetProcessingFailureMessage ( request, dicomDs, ex ) ) ;
                }
            }

            return response;
        }
    
        private delegate DicomDataset GetDicomHandler ( Stream stream ) ;

        private GetDicomHandler CreateDatasetParser(WebStoreRequest request)
        {
            GetDicomHandler getDicomDelegate = null ;


            switch (request.MediaType)
            {
                //TODO: build the response here, { Successes.Add(objectMetadata), Failures.Add(objectMetadata), Create
                case MimeMediaTypes.DICOM:
                {
                    getDicomDelegate = GetDicom;
                }
                break;

                case MimeMediaTypes.xmlDicom:
                {
                    GetDicomHandler xmlHandler = new GetDicomHandler(delegate (Stream stream)
                    {
                        StreamReader reader = new StreamReader(stream);
                        string xmlString = reader.ReadToEnd();

                        return GetXmlConverter().Convert(xmlString);
                    });
                }
                break;

                case MimeMediaTypes.Json:
                {
                    GetDicomHandler xmlHandler = new GetDicomHandler(delegate (Stream stream)
                    {
                        StreamReader reader = new StreamReader(stream);
                        string xmlString = reader.ReadToEnd();

                        return GetJsonConverter().Convert(xmlString);
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
    }
}
