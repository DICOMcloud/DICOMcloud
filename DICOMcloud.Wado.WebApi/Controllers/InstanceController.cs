using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace DICOMcloud.Wado.WebApi.Controllers
{
    public class InstanceController : ControllerBase
    {
        protected IQidoRsService QidoService { get; set; }
        protected IWebObjectStoreService StorageService { get; set; }
        protected IWadoRsService WadoService { get; set; }

        public InstanceController
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
        /// Look up instances
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("qidors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances")]
        [Route("qidors/studies/{StudyInstanceUID}/instances")]
        [Route("qidors/instances")]
        [Route("api/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances")]
        [Route("api/studies/{StudyInstanceUID}/instances")]
        [Route("api/instances")]
        public HttpResponseMessage SearchForInstances
        (
            [ModelBinder(typeof(QidoRequestModelBinder))]
            IQidoRequestModel request
        )
        {
            return QidoService.SearchForInstances(request);
        }

        [HttpGet]
        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances/{SOPInstanceUID}")]
        [Route("api/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances/{SOPInstanceUID}")]
        public async Task<HttpResponseMessage> GetInstance
        (
            [ModelBinder(typeof(RsObjectRequestModelBinder))]
            IWadoRsInstanceRequest request
        )
        {
            return await WadoService.RetrieveInstance(request);
        }

        [HttpDelete]
        [Route("delowrs/studies/{studyInstanceUID}/series/{seriesInstanceUID}/instances/{sopInstanceUID}")]
        [Route("api/studies/{studyInstanceUID}/series/{seriesInstanceUID}/instances/{sopInstanceUID}")]
        public async Task<HttpResponseMessage> DeleteInstance
        (
            [ModelBinder(typeof(RsDeleteRequestModelBinder))]
            WebDeleteRequest request
        )
        {
            return await StorageService.Delete(request);
        }

    }
}
