using DICOMcloud.IO;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System;

namespace DICOMcloud.Wado.Models
{
    public interface IWebStoreRequest : IWadoRequestHeader
    {
        string MediaType { get; set; }

        Collection<HttpContent> Contents { get; }
    }

    public class WebStoreRequest : MultipartStreamProvider, IWebStoreRequest
    {
        private Stream _tempStreamReference ;

        public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptCharsetHeader
        {
            get ;

            set ;
        }

        public HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> AcceptHeader
        {
            get ;

            set ;
        }

        public string MediaType
        {
            get; set;
        }

        //TODO: return a storage stream
        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            NameValueHeaderValue dicomType = parent.Headers.ContentType.Parameters.Where ( n=>n.Name ==  "type" ).FirstOrDefault ( ) ;

            MediaType = dicomType.Value.Trim(new char[] {'"'}) ;

            //IStorageService service = new WebStorageService ( ) ;

            //IStorageContainer storage = service.GetStorageContainer ( "DicomStorage" ) ; 

            //storage.GetNewLocation ( ).GetWriteStream ( out _tempStreamReference, false ) ;//TODO: this needs to be closed deleted, it is not working so far with all options I tried due to async calls
            // =  ; //keeping a reference within the object so it doesn't get disposed immediately as this method is called async/thread

            _tempStreamReference = new MemoryStream ( );
            return _tempStreamReference ;
        }
    }
}
