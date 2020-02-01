
using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using DICOMcloud.Pacs;
using fo = Dicom;
using DICOMcloud.IO;
using DICOMcloud.Media;

namespace DICOMcloud.Wado
{
    //TODO: this service should be unified with the RetrieveService
    public class WadoUriService : IWadoUriService
    {
        public IObjectRetrieveService RetrieveService { get; private set ; }
        
        public WadoUriService ( IObjectRetrieveService retrieveService )
        {
            RetrieveService = retrieveService ;
        }

        public virtual HttpResponseMessage GetInstance ( IWadoUriRequest request )
        {
            //validation code should go in here
            if (null == request || string.Compare(request.RequestType, "WADO", true ) != 0 )
            {
                throw new DCloudException("Request Type must be set to WADO");
            }

            List<MediaTypeHeaderValue> mediaTypeHeader = GetRequestedMimeType ( request );
            string  currentTransfer = null ;
            

            foreach (MediaTypeHeaderValue mediaType in mediaTypeHeader)
            {

                IStorageLocation dcmLocation;


                currentTransfer = GetRequestedMediaTransferSyntax ( request, mediaType );

                dcmLocation = GetLocation ( request, currentTransfer, mediaType );

                if ( null != dcmLocation && dcmLocation.Exists ( ) )
                {
                    if (DicomWebServerSettings.Instance.SupportPreSignedUrls && dcmLocation is IPreSignedUrlStorageLocation)
                    {
                        var expiry = DateTime.Now.AddHours(DicomWebServerSettings.Instance.PreSignedUrlReadExpiryTimeInHours);
                        Uri locationUrl = ((IPreSignedUrlStorageLocation)dcmLocation).GetReadUrl (null, expiry);
                        HttpResponseMessage msg = new HttpResponseMessage(HttpStatusCode.Redirect);

                        msg.Headers.Location = locationUrl;

                        return msg;
                    }
                    else
                    {
                        StreamContent sc = new StreamContent ( dcmLocation.GetReadStream ( ) );
                        sc.Headers.ContentType = new MediaTypeHeaderValue ( mediaType.MediaType );
                        HttpResponseMessage msg = new HttpResponseMessage ( HttpStatusCode.OK );

                        msg.Content = sc;

                        return msg;
                    }
                }
            }

            HttpResponseMessage responseMessage ;

            if ( TryOnDemandTransform ( request, mediaTypeHeader, out responseMessage ) )
            {
                return responseMessage ;
            }


            if ( mediaTypeHeader.Where ( n=>n.MediaType == MimeMediaTypes.DICOM ).FirstOrDefault ( ) != null )
            {
                return new HttpResponseMessage ( HttpStatusCode.NotFound ) { Content = new StringContent ( "Image not found" ) } ;
            }
            else
            {
                //6.3.2 Body of Non-DICOM Media Type Response
                //The HTTP behavior is that an error (406 - Not Acceptable) is returned if the required media type cannot be served.
                return new HttpResponseMessage ( HttpStatusCode.NotAcceptable ) ;
            }
        }

        protected virtual IStorageLocation GetLocation ( IWadoUriRequest request, string currentTransfer, MediaTypeHeaderValue mediaType )
        {
            return RetrieveService.RetrieveSopInstance ( request,
                                                         new DicomMediaProperties ( mediaType.MediaType, currentTransfer ) );
        }

        private static string GetRequestedMediaTransferSyntax ( IWadoUriRequest request, MediaTypeHeaderValue mediaType )
        {
            string anyTransfer = "*" ;
            string currentTransfer ;


            if ( null != request.ImageRequestInfo && 
                 !string.IsNullOrWhiteSpace ( request.ImageRequestInfo.TransferSyntax ) &&
                 anyTransfer != request.ImageRequestInfo.TransferSyntax)
            {
                currentTransfer = request.ImageRequestInfo.TransferSyntax;
            }
            else
            {
                string transferString;


                DefaultMediaTransferSyntax.Instance.TryGetValue ( mediaType.MediaType, out transferString );

                currentTransfer = !string.IsNullOrWhiteSpace ( transferString ) ? transferString : "";
            }

            return currentTransfer;
        }

        protected virtual bool TryOnDemandTransform 
        ( 
            IWadoUriRequest request, 
            List<MediaTypeHeaderValue> mediaTypeHeaderList, 
            out HttpResponseMessage responseMessage 
        )
        {
            string defaultDicomTransfer ;


            DefaultMediaTransferSyntax.Instance.TryGetValue ( MimeMediaTypes.DICOM, out defaultDicomTransfer ) ; 
                
            foreach ( var mediaTypeHeader in mediaTypeHeaderList )
            {
                string transferSyntax ;
                
                
                transferSyntax = GetRequestedMediaTransferSyntax ( request, mediaTypeHeader );

                //should return only one for URI service
                foreach ( var result in RetrieveService.GetTransformedSopInstances ( request, MimeMediaTypes.DICOM, defaultDicomTransfer, mediaTypeHeader.MediaType, transferSyntax ) )
                {
                    StreamContent sc        = new StreamContent        (  result.Location.GetReadStream ( ) ) ;
                    sc.Headers.ContentType  = new MediaTypeHeaderValue ( mediaTypeHeader.MediaType ) ;
                    responseMessage         = new HttpResponseMessage  ( HttpStatusCode.OK ) ;

                    responseMessage.Content = sc;

                    return true ;
                }
            }

            responseMessage = null ;

            return false ;
        }

        //TODO: there is more into this now in part 18 2016 version, reference section 6.1.1.5 and 6.1.1.6
        //exact method to tp determine "SelectedMediaType" is detailed in 6.1.1.7
        protected virtual List<MediaTypeHeaderValue> GetRequestedMimeType(IWadoUriRequest request)
        {
            List<MediaTypeHeaderValue> acceptTypes = new List<MediaTypeHeaderValue>();
            bool acceptAll = request.AcceptHeader.Contains(AllMimeType, new MediaTypeHeaderComparer ( ) ); 

            if (!string.IsNullOrEmpty(request.ContentType))
            {
                string[] mimeTypes = request.ContentType.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string mime in mimeTypes)
                {
                    MediaTypeWithQualityHeaderValue mediaType;

                    if (MediaTypeWithQualityHeaderValue.TryParse(mime, out mediaType))
                    {
                        if (acceptAll || request.AcceptHeader.Contains(mediaType, new MediaTypeHeaderComparer()))
                        {
                            acceptTypes.Add(mediaType);
                        }
                    }
                    else
                    { 
                        //TODO: throw excpetion?
                    }
                }
            }

            return acceptTypes;
        }

        private readonly MediaTypeHeaderValue AllMimeType = MediaTypeHeaderValue.Parse ("*/*");
    }

    public class MediaTypeHeaderComparer : IEqualityComparer<MediaTypeHeaderValue>
    {
        public bool Equals(MediaTypeHeaderValue x, MediaTypeHeaderValue y)
        {
            return string.Compare(x.MediaType, y.MediaType, true) == 0;
        }

        public int GetHashCode(MediaTypeHeaderValue obj)
        {
            return obj.MediaType.GetHashCode();
        }
    }
}
