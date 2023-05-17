using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Media
{
    public abstract class MimeMediaTypes
    {
        private MimeMediaTypes() { }

        public const string DICOM = "application/dicom";
        public const string xmlDicom = "application/dicom+xml" ;
        public const string Json = "application/dicom+json"; //TODO: this is now updated to DICOM 2016, no more application/json
        public const string UncompressedData = "application/octet-stream" ; //uncompressed bulk data
        public const string Jpeg  = "image/jpeg";
        public const string WebP  = "image/webp";
        
        public const string PlainText = "text/plain" ;
        public const string MultipartRelated = "multipart/related" ;
        
        public const string Rle = "image/x-dicom-rle";
        public const string Jls = "image/x-jls" ;
        public const string Jp2 = "image/jp2" ;
        public const string Jpx = "image/jpx" ;

        public const string Mpeg2 = "video/mpeg2" ;
        public const string Mp4   = "video/mp4" ;
    }

    public class MimeMediaType
    { 
        public MimeMediaType ( string mimeType )
        {
            MimeType = mimeType ;
        }

        public override bool Equals(object obj)
        {
            if ( obj is string )
            { 
                return string.Equals ( MimeType, (string)obj, StringComparison.InvariantCultureIgnoreCase ) ;
            }

            if ( obj is MimeMediaType )
            {
                return string.Equals ( MimeType, ((MimeMediaType) obj).MimeType ) ;
            }

            return false ;
        }

        public override int GetHashCode()
        {
            if ( string.IsNullOrWhiteSpace ( MimeType ) )
            { 
                return base.GetHashCode();
            }

            return MimeType.GetHashCode ( ) ;
        }

        public override string ToString()
        {
            if ( string.IsNullOrWhiteSpace ( MimeType ) )
            { 
                return "" ;
            }

            return MimeType ;
        }
        public string MimeType { get; set; }
    
        public static implicit operator string(MimeMediaType mime)
        {
            return mime.MimeType;
        }


        public static implicit operator MimeMediaType(string mime)
        {
            return new MimeMediaType(mime);
        }

        public bool IsIn
        (
            HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> httpHeaderValueCollection
        )
        {
            MediaTypeWithQualityHeaderValue mediaType = null ;

            return IsIn (httpHeaderValueCollection, out mediaType) ;
        }

        public bool IsIn
        (
            HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> httpHeaderValueCollection,
            out MediaTypeWithQualityHeaderValue mediaType
        )
        {
            mediaType = httpHeaderValueCollection.FirstOrDefault(n=>n.MediaType.Equals(MimeType, StringComparison.InvariantCultureIgnoreCase )) ;

            return  mediaType != null ;
        }
    }
}
