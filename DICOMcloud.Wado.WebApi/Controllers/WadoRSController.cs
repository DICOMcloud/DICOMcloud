using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using DICOMcloud.Extensions;
using DICOMcloud.Wado;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado.Controllers
{
    public class WadoRSController : ApiController
    {
        IWadoRsService WadoService { get; set;  }

        public WadoRSController ( IWadoRsService wadoService )
        {
            WadoService = wadoService ;
        }

        [Route("wadors/studies/{StudyInstanceUID}")]
        [HttpGet]
        public HttpResponseMessage GetStudies 
        ( 
            [ModelBinder(typeof(RsStudiesRequestModelBinder))] 
            IWadoRsStudiesRequest request 
        )
        {
            return WadoService.RetrieveStudy ( request ) ;
        }

        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}")]
        [HttpGet]
        public HttpResponseMessage GetSeries 
        ( 
            [ModelBinder(typeof(RsSeriesRequestModelBinder))] 
            IWadoRsSeriesRequest request 
        )
        {
            return WadoService.RetrieveSeries ( request ) ;
        }

        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances/{SOPInstanceUID}")]
        [HttpGet]
        public HttpResponseMessage GetInstance 
        ( 
            [ModelBinder(typeof(RsObjectRequestModelBinder))]  
            IWadoRsInstanceRequest request 
        )
        {
            return WadoService.RetrieveInstance ( request ) ;
        }

        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances/{SOPInstanceUID}/frames/{FrameList}")]
        [HttpGet]
        public HttpResponseMessage GetFrames 
        ( 
            [ModelBinder(typeof(RsFrameRequestModelBinder))]  
            IWadoRsFramesRequest request 
        )
        {
            return WadoService.RetrieveFrames ( request ) ;
        }

        [Route("wadors/studies/{StudyInstanceUID}/metadata")]
        [HttpGet]
        public HttpResponseMessage GetStudiesMetadata
        ( 
            [ModelBinder(typeof(RsStudiesRequestModelBinder))] 
            IWadoRsStudiesRequest request 
        )
        {
            return WadoService.RetrieveStudyMetadata ( request ) ;
        }

        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/metadata")]
        [HttpGet]
        public HttpResponseMessage GetSeriesMetadata
        ( 
            [ModelBinder(typeof(RsSeriesRequestModelBinder))] 
            IWadoRsSeriesRequest request 
        )
        {
            return WadoService.RetrieveSeriesMetadata ( request ) ;
        }

        [Route("wadors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances/{SOPInstanceUID}/metadata")]
        [HttpGet]
        public HttpResponseMessage GetInstanceMetadata
        ( 
            [ModelBinder(typeof(RsObjectRequestModelBinder))]  
            IWadoRsInstanceRequest request 
        )
        {
            try
            { 
                return WadoService.RetrieveInstanceMetadata ( request ) ;
            }
            catch ( Exception ex )
            { 
                return new HttpResponseMessage () { Content = new StringContent ( ex.ToJson())};
            }
        }
    }
}
