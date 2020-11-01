using DICOMcloud.Extensions;
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DICOMcloud.Wado.WebApi.Controllers
{
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
        public async Task<HttpResponseMessage> GetStudiesMetadata
        (
            [ModelBinder(typeof(RsStudiesRequestModelBinder))]
            IWadoRsStudiesRequest request
        )
        {
            return await WadoService.RetrieveStudyMetadata(request);
        }

        [HttpGet]
        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/metadata")]
        [Route("api/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/metadata")]
        public async Task<HttpResponseMessage> GetSeriesMetadata
        (
            [ModelBinder(typeof(RsSeriesRequestModelBinder))]
            IWadoRsSeriesRequest request
        )
        {
            return await WadoService.RetrieveSeriesMetadata(request);
        }

        [HttpGet]
        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances/{SOPInstanceUID}/metadata")]
        [Route("api/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances/{SOPInstanceUID}/metadata")]
        public async Task<HttpResponseMessage> GetInstanceMetadata
        (
            [ModelBinder(typeof(RsObjectRequestModelBinder))]
            IWadoRsInstanceRequest request
        )
        {
            try
            {
                return await WadoService.RetrieveInstanceMetadata(request);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { Content = new StringContent(ex.ToJson()) };
            }
        }
    }
}