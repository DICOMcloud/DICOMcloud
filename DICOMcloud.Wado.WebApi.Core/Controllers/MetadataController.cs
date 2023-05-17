using DICOMcloud.Extensions;
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
        public HttpResponseMessage GetStudiesMetadata
        (
            [ModelBinder(typeof(RsStudiesRequestModelBinder))]
            IWadoRsStudiesRequest request
        )
        {
            return WadoService.RetrieveStudyMetadata(request);
        }

        [HttpGet]
        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/metadata")]
        [Route("api/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/metadata")]
        public HttpResponseMessage GetSeriesMetadata
        (
            [ModelBinder(typeof(RsSeriesRequestModelBinder))]
            IWadoRsSeriesRequest request
        )
        {
            return WadoService.RetrieveSeriesMetadata(request);
        }

        [HttpGet]
        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances/{SOPInstanceUID}/metadata")]
        [Route("api/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances/{SOPInstanceUID}/metadata")]
        public HttpResponseMessage GetInstanceMetadata
        (
            [ModelBinder(typeof(RsObjectRequestModelBinder))]
            IWadoRsInstanceRequest request
        )
        {
            try
            {
                return WadoService.RetrieveInstanceMetadata(request);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { Content = new StringContent(ex.ToJson()) };
            }
        }
    }
}
