using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Web.Http.Results;
using DICOMcloud.Media;
using System.Text;

namespace DICOMcloud.Wado.WebApi.Controllers
{
    [ApiController]
    public class StudiesController : ControllerBase
    {
        protected IQidoRsService QidoService { get; set; }
        protected IWebObjectStoreService StorageService { get; set; }
        protected IWadoRsService WadoService { get; set; }

        public StudiesController
        (
            IQidoRsService qidoService, 
            IWebObjectStoreService storageService, 
            IWadoRsService wadoService
        )
        {
            QidoService = qidoService;
            StorageService = storageService;
            WadoService = wadoService;
        }

        /// <summary>
        /// Look up studies (i.e., for a particular patient)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("qidors/studies")]
        [Route("api/studies")]
        [HttpGet]
        public ActionResult<QidoResponse> SearchForStudies
        (
            [ModelBinder(typeof(QidoRequestModelBinder))]
            IQidoRequestModel request
        )
        {
            return QidoService.SearchForStudies(request);
        }


        [HttpGet]
        [Route("wadors/studies/{StudyInstanceUID}")]
        [Route("api/studies/{StudyInstanceUID}")]
        public IActionResult GetStudies
        (
            [ModelBinder(typeof(RsStudiesRequestModelBinder))]
            IWadoRsStudiesRequest request
        )
        {
            return new WadoRsResult(WadoService.RetrieveStudy(request));
        }

        [HttpPost]
        [Route("stowrs/studies/{studyInstanceUID}")]
        [Route("stowrs")]
        [Route("api/studies/{studyInstanceUID}")]
        [Route("api/studies/")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Post(string? studyInstanceUID = null)
        {
            WebStoreRequest webStoreRequest = new WebStoreRequest(Request);
            IStudyId? studyId = null;


            if (!string.IsNullOrWhiteSpace(studyInstanceUID))
            {
                studyId = new ObjectId() { StudyInstanceUID = studyInstanceUID };
            }

            if (Request.GetMultipartBoundary() == null)
            {
                return StatusCode((int)HttpStatusCode.UnsupportedMediaType);
            }

            return new WebStoreResult(await StorageService.Store(webStoreRequest, studyId));
        }

        [HttpDelete]
        [Route("delowrs/studies/{studyInstanceUID}")]
        [Route("api/studies/{studyInstanceUID}")]
        public async Task<IActionResult> DeleteStudy
        (
            [ModelBinder(typeof(RsDeleteRequestModelBinder))]
            WebDeleteRequest request
        )
        {
            await StorageService.Delete(request);

            return Ok();
        }
    }
}
