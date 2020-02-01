using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace DICOMcloud.Wado.WebApi.Controllers
{
    public class FramesController : ApiController
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
        public HttpResponseMessage GetFrames
        (
            [ModelBinder(typeof(RsFrameRequestModelBinder))]
            IWadoRsFramesRequest request
        )
        {
            return WadoService.RetrieveFrames(request);
        }

    }
}
