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

        [HttpPost]
        [Route("stowrs/studies/{StudyInstanceUID}")]
        [Route("stowrs")]
        public async Task<HttpResponseMessage> Post(string StudyInstanceUID = null)
        {

            WebStoreRequest webStoreRequest = new WebStoreRequest ( Request) ;

            if ( !Request.Content.IsMimeMultipartContent("related") )
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            await Request.Content.ReadAsMultipartAsync ( webStoreRequest ) ;

            return await StorageService.Store (webStoreRequest, StudyInstanceUID);
        }
    }
}