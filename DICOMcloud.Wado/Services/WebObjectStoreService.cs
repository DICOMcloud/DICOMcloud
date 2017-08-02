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

namespace DICOMcloud.Wado
{
    public class WebObjectStoreService : IWebObjectStoreService
    {
        private IObjectStoreService _storageService;

        //public WebObjectStoreService ( ) : this ( new ObjectStoreDataService ( ) ) {}
        public WebObjectStoreService ( IObjectStoreService storage ) 
        {
            _storageService = storage ;
        }

        public virtual async Task<HttpResponseMessage> Store
        (
            IWebStoreRequest request, 
            string studyInstanceUID 
        )
        {
            WadoStoreResponse storeResult = null  ;

            switch ( request.MediaType )
            {
                //TODO: build the response here, { Successes.Add(objectMetadata), Failures.Add(objectMetadata), Create
                case MimeMediaTypes.DICOM:
                {
                    storeResult = await GetResponseDataset (request, studyInstanceUID );
                }
                break ;

                case MimeMediaTypes.xmlDicom:
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                
                case MimeMediaTypes.Json:
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                
                default:
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
            }

            if ( null != storeResult )
            {
                var result = new HttpResponseMessage ( storeResult.HttpStatus) ;

                
                if ( new MimeMediaType ( MimeMediaTypes.Json ).IsIn ( request.AcceptHeader ) ) //this is not taking the "q" parameter
                {
                    JsonDicomConverter converter = new JsonDicomConverter ( ) ;
                    
                    
                    result.Content = new StringContent (  converter.Convert ( storeResult.GetResponseContent ( ) ), 
                                                          System.Text.Encoding.UTF8, MimeMediaTypes.Json ) ;
                }
                else
                {
                    XmlDicomConverter xmlConverter = new XmlDicomConverter ( ) ;
                    
                    result.Content = new StringContent (  xmlConverter.Convert ( storeResult.GetResponseContent ( ) ), 
                                                          System.Text.Encoding.UTF8, MimeMediaTypes.xmlDicom ) ;
                }

                return result ;    
            }
            else
            {
                return new HttpResponseMessage ( HttpStatusCode.BadRequest ) ;
            }
        }

        public virtual Task<HttpResponseMessage> Delete  ( IWebDeleteRequest request )
        {
            var result = _storageService.Delete ( request.Dataset, request.DeleteLevel )  ;

            if ( result.Status == Pacs.Commands.CommandStatus.Failed )
            {
                //TODO: inspect exception type and return proper error
                return Task.FromResult( new HttpResponseMessage ( HttpStatusCode.InternalServerError ) ) ;
            }
            
            return Task.FromResult( new HttpResponseMessage ( HttpStatusCode.OK ) ) ;
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

        protected virtual InstanceMetadata CreateObjectMetadata ( fo.DicomDataset dataset, IWebStoreRequest request )
        {
            return new InstanceMetadata ( ) { } ;
        }

        private async Task<WadoStoreResponse> GetResponseDataset ( IWebStoreRequest request, string studyInstanceUID )
        {
            WadoStoreResponse response = new WadoStoreResponse(studyInstanceUID);

            foreach (var mediaContent in request.Contents)
            {
                Stream dicomStream = await mediaContent.ReadAsStreamAsync();
                var    dicomDs     = GetDicom ( dicomStream ) ;

                PublisherSubscriberFactory.Instance.Publish ( this, new WebStoreDatasetProcessingMessage ( request, dicomDs ) ) ;
                
                try
                {
                    var result = _storageService.StoreDicom ( dicomDs, CreateObjectMetadata ( dicomDs, request ) ) ;

                    response.AddResult(result);
                }
                catch (Exception ex)
                {
                    response.AddResult(ex, dicomStream);
                }

            }

            return response;
        }
    }
}
