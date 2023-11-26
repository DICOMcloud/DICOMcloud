using DICOMcloud.Wado.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using DICOMcloud.Pacs;
using DICOMcloud.Media;
using Microsoft.Net.Http.Headers;
using DICOMcloud.IO;
using DICOMcloud;
using fo = Dicom;
using System.IO;
using System;

namespace DICOMcloud.Wado
{
    public class WadoRsService : IWadoRsService
    {
        IObjectRetrieveService RetrieveService { get; set;  }
        
        public WadoRsService ( IObjectRetrieveService retrieveService )
        {
            RetrieveService   = retrieveService ;
        }

        //DICOM Instances are returned in either DICOM or Bulk data format
        //DICOM format is part10 native, Bulk data is based on the accept:
        //octet-stream, jpeg, jp2....
        public virtual WadoRsResponse RetrieveStudy ( IWadoRsStudiesRequest request )
        {
            return RetrieveMultipartInstance ( request, new WadoRsInstanceRequest ( request ) ) ;
        }

        public virtual WadoRsResponse RetrieveSeries ( IWadoRsSeriesRequest request )
        {
            return RetrieveMultipartInstance ( request, new WadoRsInstanceRequest ( request ) ) ;
        }

        public virtual WadoRsResponse RetrieveInstance ( IWadoRsInstanceRequest request )
        {
            return RetrieveMultipartInstance ( request, request ) ;
        }

        public virtual WadoRsResponse RetrieveFrames ( IWadoRsFramesRequest request )
        {
            return RetrieveMultipartInstance ( request, request ) ;
        }

        public virtual WadoRsResponse RetrieveBulkData ( IWadoRsInstanceRequest request )
        {
            //TODO: validation accept header is not dicom...

            return RetrieveMultipartInstance ( request, request ) ;
        }
        
        public virtual WadoRsResponse RetrieveBulkData ( IWadoRsFramesRequest request )
        {
            //TODO: validation accept header is not dicom...

            return RetrieveMultipartInstance ( request, request ) ;
        }
        
        //Metadata can be XML (Required) or Json (optional) only. DICOM Instances are returned with no bulk data
        //Bulk data URL can be returned (which we should) 
        public virtual WadoRsResponse RetrieveStudyMetadata(IWadoRsStudiesRequest request)
        {
            return RetrieveInstanceMetadata ( new WadoRsInstanceRequest ( request ) );
        }

        public virtual WadoRsResponse RetrieveSeriesMetadata(IWadoRsSeriesRequest request)
        {
            return RetrieveInstanceMetadata ( new WadoRsInstanceRequest ( request ) );
        }

        public virtual WadoRsResponse RetrieveInstanceMetadata(IWadoRsInstanceRequest request)
        {
            foreach(var header in request.AcceptHeader )
            {
                if (MultipartResponseHelper.IsMultiPart(header))
                { 
                    var subMediaHeader = MultipartResponseHelper.GetSubMediaType(header);

                    if (null == subMediaHeader || subMediaHeader != MimeMediaTypes.XmlDicom)
                    {
                        return new WadoRsResponse() { StatusCode = System.Net.HttpStatusCode.BadRequest };
                    }

                    return RetrieveMultipartInstance(request, request); //should be an XML request!
                }
                //must be json, or just return json anyway (defualt) or (*/*)
                else if (header.MediaType == MimeMediaTypes.JsonDicom || header.MediaType == MimeMediaTypes.Json || header.MediaType == MimeMediaTypes.Any)
                {
                    return ProcessJsonRequest(request, request);
                }
            }


            return ProcessJsonRequest(request, request);
        }

        public virtual WadoRsResponse RetrieveMultipartInstance ( IWadoRequestHeader header, IObjectId request )
        {
            MultipartContent multiContent ;
            MediaTypeHeaderValue selectedMediaTypeHeader ;
            WadoRsResponse response;


            multiContent            = new MultipartContent ( "related", MultipartResponseHelper.DicomDataBoundary ) ;           
            response                = new WadoRsResponse(header, request, multiContent) {  StatusCode = System.Net.HttpStatusCode.OK };
            selectedMediaTypeHeader = null;


            foreach ( var mediaTypeHeader in header.AcceptHeader )
            {
                if (!MultipartResponseHelper.IsMultiPart(mediaTypeHeader)) continue;

                selectedMediaTypeHeader = mediaTypeHeader;

                if ( request is IWadoRsFramesRequest )
                {
                    var frames = ((IWadoRsFramesRequest) request ).Frames ;
                    foreach ( int frame in frames )
                    {
                        request.Frame = frame ;

                        foreach ( var wadoResponse in ProcessMultipartRequest ( request, mediaTypeHeader ) )
                        {
                            MultipartResponseHelper.AddMultipartContent ( multiContent, wadoResponse );                            
                        }
                    }
                }
                else
                {
                    foreach ( var wadoResponse in ProcessMultipartRequest ( request, mediaTypeHeader ) )
                    { 
                        MultipartResponseHelper.AddMultipartContent ( multiContent, wadoResponse );
                    }
                }

                if (multiContent.Count() > 0) { break ; }
            }

            if (selectedMediaTypeHeader == null)
            {
                return new WadoRsResponse() { StatusCode = System.Net.HttpStatusCode.NotAcceptable };
            }

            if (multiContent.Count() == 0)
            {
                return new WadoRsResponse() { StatusCode = System.Net.HttpStatusCode.NotFound };
            }

            multiContent.Headers.ContentType.Parameters.Add ( 
                new System.Net.Http.Headers.NameValueHeaderValue ( 
                    "type", "\"" + MultipartResponseHelper.GetSubMediaType (selectedMediaTypeHeader) + "\"" ) ) ;

            return response ;
        }

        protected virtual WadoRsResponse ProcessJsonRequest 
        ( 
            IWadoRequestHeader header, 
            IObjectId objectID
        )
        {
            List<IWadoRsResponse> responses = new List<IWadoRsResponse> ( ) ;
            WadoRsResponse response = new WadoRsResponse( ) ;
            StringBuilder fullJsonResponse = new StringBuilder ("[") ;
            StringBuilder jsonArray = new StringBuilder ( ) ;
            string selectedTransfer = "" ;
            bool exists = false ;
            var mediaTypeHeader = header.AcceptHeader.FirstOrDefault ( ) ;

            IEnumerable<NameValueHeaderValue> transferSyntaxHeader = null ;
            List<string> transferSyntaxes = new List<string> ( ) ;
            var defaultTransfer = "" ;

            
            if ( null != mediaTypeHeader )
            {
                transferSyntaxHeader = mediaTypeHeader.Parameters.Where (n=>n.Name == "transfer-syntax") ;
            }

            if ( null == transferSyntaxHeader || 0 == transferSyntaxHeader.Count ( ) )
            {
                transferSyntaxes.Add ( defaultTransfer ) ;
            }
            else
            {
                transferSyntaxes.AddRange ( transferSyntaxHeader.Select ( n=>n.Value.Value ) ) ;
            }

            foreach ( var transfer in transferSyntaxes )
            {
                selectedTransfer = transfer == "*" ? defaultTransfer : transfer ;

                foreach ( IStorageLocation storage in GetLocations (objectID, new DicomMediaProperties ( MimeMediaTypes.JsonDicom, selectedTransfer ) ) )
                {
                    exists = true ;
                    
                    using (var memoryStream = new MemoryStream())
                    {
                        storage.Download ( memoryStream ) ;
                        jsonArray.Append ( System.Text.Encoding.UTF8.GetString(memoryStream.ToArray ( ) ) ) ;
                        jsonArray.Append (",") ;
                    }
                }

                if ( exists ) { break ; }
            }

            fullJsonResponse.Append(jsonArray.ToString().TrimEnd(','));
            fullJsonResponse.Append ("]") ;

            if ( exists ) 
            {
                var content  = new StreamContent ( new MemoryStream (System.Text.Encoding.UTF8.GetBytes(fullJsonResponse.ToString())) ) ;
            
                content.Headers.ContentType= new System.Net.Http.Headers.MediaTypeHeaderValue (MimeMediaTypes.JsonDicom);
            
                if ( !string.IsNullOrWhiteSpace ( selectedTransfer ) )
                {
                    content.Headers.ContentType.Parameters.Add ( new System.Net.Http.Headers.NameValueHeaderValue ( "transfer-syntax", "\"" + selectedTransfer + "\""));
                }

                response.Content =  content ;
            }
            else
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
            }
            
            return response ;
        }


        /// <Examples>
        /// Accept: multipart/related; type="image/jpx"; transfer-syntax=1.2.840.10008.1.2.4.92,
        /// Accept: multipart/related; type="image/jpx"; transfer-syntax=1.2.840.10008.1.2.4.93
        /// Accept: multipart/related; type="image/jpeg"
        /// </Examples>
        protected virtual IEnumerable<IWadoRsResponse> ProcessMultipartRequest
        (
            IObjectId objectID,
            MediaTypeHeaderValue mediaTypeHeader
            
        )
        {
            string              subMediaType;
            IEnumerable<string> transferSyntaxes ;
            string              defaultTransfer = null;
            bool                instancesFound = false ;

            subMediaType = MultipartResponseHelper.GetSubMediaType(mediaTypeHeader) ;

            DefaultMediaTransferSyntax.Instance.TryGetValue ( subMediaType, out defaultTransfer );

            transferSyntaxes = MultipartResponseHelper.GetRequestedTransferSyntax ( mediaTypeHeader, defaultTransfer );

            foreach ( var result in FindLocations ( objectID, subMediaType, transferSyntaxes, defaultTransfer ) )
            {
                instancesFound = true ;

                yield return new WadoResponse ( result.Location.GetReadStream ( ), subMediaType ) { TransferSyntax = result.TransferSyntax };
            }

            if ( !instancesFound )
            {
                string defaultDicomTransfer ;


                DefaultMediaTransferSyntax.Instance.TryGetValue ( MimeMediaTypes.DICOM, out defaultDicomTransfer ) ; 
                

                foreach ( var result in RetrieveService.GetTransformedSopInstances ( objectID, MimeMediaTypes.DICOM, defaultDicomTransfer, subMediaType, transferSyntaxes.FirstOrDefault ( ) ) )
                {
                    yield return new WadoResponse ( result.Location.GetReadStream ( ), subMediaType ) { TransferSyntax = result.TransferSyntax };
                }
            }
        }

        protected virtual IEnumerable<ObjectRetrieveResult> FindLocations ( IObjectId objectID, string subMediaType, IEnumerable<string> transferSyntaxes, string defaultTransfer )
        {
            return RetrieveService.FindSopInstances ( objectID, subMediaType, transferSyntaxes, defaultTransfer ) ;
        }

        protected virtual IEnumerable<IStorageLocation> GetLocations ( IObjectId request, DicomMediaProperties mediaInfo )
        {
            if ( null != request.Frame )
            {
                List<IStorageLocation> result = new List<IStorageLocation> ( ) ;

                
                result.Add ( RetrieveService.RetrieveSopInstance ( request, mediaInfo ) ) ;

                return result ;
            }
            else
            {
                return RetrieveService.RetrieveSopInstances ( request, mediaInfo ) ;
            }
        }
    }
}
