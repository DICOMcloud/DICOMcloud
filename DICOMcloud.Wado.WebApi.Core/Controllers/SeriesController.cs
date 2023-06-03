﻿using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc;

namespace DICOMcloud.Wado.WebApi.Controllers
{
    [ApiController]
    public class SeriesController : ControllerBase
    {
        protected IQidoRsService QidoService { get; set; }
        protected IWebObjectStoreService StorageService { get; set; }
        protected IWadoRsService WadoService { get; set; }

        public SeriesController
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
        /// Look up a series
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("qidors/studies/{StudyInstanceUID}/series")]
        [Route("qidors/series")]
        [Route("api/studies/{StudyInstanceUID}/series")]
        [Route("api/series")]
        [HttpGet]
        public HttpResponseMessage SearchForSeries
        (
            [ModelBinder(typeof(QidoRequestModelBinder))]
            IQidoRequestModel request
        )
        {
            return QidoService.SearchForSeries(request);
        }

        [HttpGet]
        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}")]
        [Route("api/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}")]
        public HttpResponseMessage GetSeries 
        ( 
            [ModelBinder(typeof(RsSeriesRequestModelBinder))] 
            IWadoRsSeriesRequest request 
        )
        {
            return WadoService.RetrieveSeries ( request ) ;
        }

        [HttpDelete]
        [Route("delowrs/studies/{studyInstanceUID}/series/{seriesInstanceUID}")]
        [Route("api/studies/{studyInstanceUID}/series/{seriesInstanceUID}")]
        public async Task<HttpResponseMessage> DeleteSeries
        (
            [ModelBinder(typeof(RsDeleteRequestModelBinder))]
            WebDeleteRequest request
        )
        {
            return await StorageService.Delete(request);
        }
    }
}