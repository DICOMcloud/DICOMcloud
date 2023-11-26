using DICOMcloud.Extensions;
using DICOMcloud.Media;
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc;

namespace DICOMcloud.Wado.WebApi.Controllers
{
    [ApiController]
    public class MetadataController : ControllerBase
    {
        protected IWadoRsService WadoService { get; set; }

        public MetadataController
        (
            IWadoRsService wadoService
        )
        {
            WadoService = wadoService;
        }

        [HttpGet]
        [Route("wadors/studies/{StudyInstanceUID}/metadata")]
        [Route("api/studies/{StudyInstanceUID}/metadata")]
        public IActionResult GetStudiesMetadata
        (
            [ModelBinder(typeof(RsStudiesRequestModelBinder))]
            IWadoRsStudiesRequest request
        )
        {
            return new WadoRsResult(WadoService.RetrieveStudyMetadata(request));
        }

        [HttpGet]
        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/metadata")]
        [Route("api/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/metadata")]
        public IActionResult GetSeriesMetadata
        (
            [ModelBinder(typeof(RsSeriesRequestModelBinder))]
            IWadoRsSeriesRequest request
        )
        {
            return new WadoRsResult(WadoService.RetrieveSeriesMetadata(request));
        }

        [HttpGet]
        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances/{SOPInstanceUID}/metadata")]
        [Route("api/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances/{SOPInstanceUID}/metadata")]
        public IActionResult GetInstanceMetadata
        (
            [ModelBinder(typeof(RsObjectRequestModelBinder))]
            IWadoRsInstanceRequest request
        )
        {
            return new WadoRsResult(WadoService.RetrieveInstanceMetadata(request));
        }
    }
}
