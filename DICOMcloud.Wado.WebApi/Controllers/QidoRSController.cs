using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using DICOMcloud.Wado;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado.Controllers
{
    /// <summary>
    /// Query based on ID for DICOM Objects (QIDO) enables you to search for studies, series and instances by patient ID, 
    /// and receive their unique identifiers for further usage (i.e., to retrieve their rendered representations). More detail can be found in PS3.18 6.7.
    /// https://dicomweb.hcintegrations.ca/services/query/
    /// </summary>
    public class QidoRSController : ApiController
    {
        protected IQidoRsService QidoService {get; set;}

        public QidoRSController ( IQidoRsService qidoService )
        {
            QidoService = qidoService ;
        }

        /// <summary>
        /// Look up studies (i.e., for a particular patient)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("qidors/studies")]
        [HttpGet]
        public HttpResponseMessage SearchForStudies
        (
            [ModelBinder(typeof(QidoRequestModelBinder))] 
            IQidoRequestModel request
        )
        {
            return QidoService.SearchForStudies ( request ) ;
        }

        /// <summary>
        /// Look up a series
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("qidors/studies/{StudyInstanceUID}/series")]
        [Route("qidors/series")]
        [HttpGet]
        public HttpResponseMessage SearchForSeries 
        ( 
            [ModelBinder(typeof(QidoRequestModelBinder))] 
            IQidoRequestModel request 
        ) 
        {
            return QidoService.SearchForSeries ( request ) ;
        }

        /// <summary>
        /// Look up instances
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("qidors/studies/{StudyInstanceUID}/series/{SeriesInstanceUID}/instances")]
        [Route("qidors/studies/{StudyInstanceUID}/instances")]
        [Route("qidors/instances")]
        [HttpGet]
        public HttpResponseMessage SearchForInstances 
        (
            [ModelBinder(typeof(QidoRequestModelBinder))] 
            IQidoRequestModel request  
        ) 
        {
            return QidoService.SearchForInstances ( request ) ;
        }
    }
}