using System.Net.Http;
using System.Threading.Tasks;
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc;

namespace DICOMcloud.Wado.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
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
        public async Task<HttpResponseMessage> GetFrames
        (
            [ModelBinder(typeof(RsFrameRequestModelBinder))]
            IWadoRsFramesRequest request
        )
        {
            return await WadoService.RetrieveFrames(request);
        }
    }
}
