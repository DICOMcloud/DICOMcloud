using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DICOMcloud.Wado;
using DICOMcloud.Wado.Models;


namespace DICOMcloud.Wado.Controllers
{ 
        [LogAction]

        public class StowRSController : ApiController
    {
        public IWebObjectStoreService StorageService { get; set; }

        public StowRSController ( ) : this (null) {}
        public StowRSController ( IWebObjectStoreService storageService )
        {
            StorageService = storageService ;
        }
        [HttpPost]
        [Route("stowrs/studies/{studyInstanceUID}")]
        [Route("stowrs")]
        public async Task<HttpResponseMessage> Post(string studyInstanceUID = null)
        {
            WebStoreRequest webStoreRequest = new WebStoreRequest ( Request) ;
            IStudyId studyId = null;


            if ( !string.IsNullOrWhiteSpace (studyInstanceUID))
            { 
                studyId = new ObjectId ( ) {StudyInstanceUID = studyInstanceUID};
            }

            if ( !Request.Content.IsMimeMultipartContent("related") )
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            await Request.Content.ReadAsMultipartAsync ( webStoreRequest ) ;

            return await StorageService.Store (webStoreRequest, studyId);
        }
    }
}