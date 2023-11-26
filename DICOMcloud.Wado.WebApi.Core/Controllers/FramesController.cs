using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DICOMcloud.Wado.WebApi.Controllers
{
    [ApiController]
    public class FramesController : ControllerBase
    {
        protected IQidoRsService QidoService { get; set; }
        protected IWadoRsService WadoService { get; set; }

        public FramesController
        (
            IQidoRsService qidoService,
            IWadoRsService wadoService
        )
        {
            QidoService = qidoService;
            WadoService = wadoService;
        }

        [HttpGet]
        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances/{SOPInstanceUID}/frames/{FrameList}")]
        [Route("api/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances/{SOPInstanceUID}/frames/{FrameList}")]
        public IActionResult GetFrames
        (
            [ModelBinder(typeof(RsFrameRequestModelBinder))]
            IWadoRsFramesRequest request
        )
        {
            return new WadoRsResult(WadoService.RetrieveFrames(request));
        }

    }
}
