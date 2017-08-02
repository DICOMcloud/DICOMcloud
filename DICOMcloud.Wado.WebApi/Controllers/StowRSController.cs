using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DICOMcloud.Wado;
using DICOMcloud.Wado.Models;


namespace DICOMcloud.Wado.Controllers
{
    public class StowRSController : ApiController
    {
        public IWebObjectStoreService StorageService { get; set; }

        public StowRSController ( ) : this (null) {}
        public StowRSController ( IWebObjectStoreService storageService )
        {
            StorageService = storageService ;
        }

        // POST api/<controller>
        [HttpPost]
        [Route("stowrs/studies/{StudyInstanceUID}")]
        public async Task<HttpResponseMessage> Post(string StudyInstanceUID = null)
        {

            WebStoreRequest webStoreRequest = new WebStoreRequest ( ) ;

            if ( !Request.Content.IsMimeMultipartContent("related") )
            {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            await Request.Content.ReadAsMultipartAsync ( webStoreRequest ) ;

            webStoreRequest.AcceptCharsetHeader = Request.Headers.AcceptCharset ;
            webStoreRequest.AcceptHeader        = Request.Headers.Accept ;

            return await StorageService.Store (webStoreRequest, StudyInstanceUID);
        }
    }

    public class DicomStoreRequest 
    {
        public string ContentTypeHeader
        {
            get; set;
        }

        public MultiPartContent[] Body
        {
            get; set;
        }
    }

    public class MultiPartContent
    {
        public string ContentTypeHeader
        {
            get; set;
        }

        public System.IO.Stream Content
        {
            get; set; 
        }
    }
}