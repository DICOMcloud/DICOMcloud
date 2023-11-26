using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud.Media;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public static class MultipartResponseHelper
    {
        static MultipartResponseHelper ( ) 
        {
            DicomDataBoundary =  "DICOM DATA BOUNDARY" ;
        }

        public static string DicomDataBoundary
        {
            get;
            set;
        }


        public static bool IsMultiPart(MediaTypeHeaderValue header)
        {
            return header.MediaType == MimeMediaTypes.MultipartRelated;
        }

        public static void AddMultipartContent ( MultipartContent multiContent, IWadoRsResponse wadoResponse )
        {
            StreamContent sContent = new StreamContent ( wadoResponse.Content );

            sContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue ( wadoResponse.MimeType );

            multiContent.Add ( sContent );
        }

        public static IEnumerable<string> GetRequestedTransferSyntax  (  MediaTypeHeaderValue mediaTypeHeader, string defaultTransfer )
        {
            //TODO: this should be extended to include query parameters in the request?
            List<string> transferSyntaxes ;
            IEnumerable<NameValueHeaderValue> transferSyntaxHeader ;

            
            transferSyntaxes     = new List<string> ( ) ;
            transferSyntaxHeader = mediaTypeHeader.Parameters.Where ( n => n.Name == "transfer-syntax" );

            if ( 0 == transferSyntaxHeader.Count ( ) )
            {
                transferSyntaxes.Add ( defaultTransfer );
            }
            else
            {
                transferSyntaxes.AddRange ( transferSyntaxHeader.Select ( n => n.Value.Value ) );
            }

            return transferSyntaxes ;
        }

        public static string GetSubMediaType (MediaTypeHeaderValue mediaTypeHeader )
        {
        
            var subMediaTypeHeader = mediaTypeHeader.Parameters.Where ( n => n.Name == "type" ).FirstOrDefault ( );

            return (subMediaTypeHeader != null) ? subMediaTypeHeader.Value.Value.Trim ( '"' ) : "";
        }

    }
}
