using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using DICOMcloud.Wado;
using DICOMcloud.Wado.Models;


namespace DICOMcloud.Wado.Controllers
{
    [Authorize]
    public class DelowRSController : ApiController
    {
        public IWebObjectStoreService StorageService { get; set; }

        public DelowRSController ( IWebObjectStoreService storageService )
        {
            StorageService = storageService ;
        }

        [HttpDelete]
        [Route("delowrs/studies/{studyInstanceUID}")]
        [Route("delowrs/studies/{studyInstanceUID}/series/{seriesInstanceUID}")]
        [Route("delowrs/studies/{studyInstanceUID}/series/{seriesInstanceUID}/instances/{sopInstanceUID}")]
        public async Task<HttpResponseMessage> DeleteStudy
        (
            [ModelBinder(typeof(RsDeleteRequestModelBinder))] 
            WebDeleteRequest request 
        )
        {
            return await StorageService.Delete ( request );
        }
    }
}