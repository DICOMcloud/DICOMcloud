using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;


namespace DICOMcloud.Wado.WebApi.Controllers
{
    public class OhifViewerController : ApiController
    {
        protected IOhifService OhifService {get; set;}

        public OhifViewerController ( IOhifService ohifService )
        {
            OhifService = ohifService;
        }

        [Route("ohif/study/{studyUid}/series")]
        [HttpGet]
        public HttpResponseMessage GetStudy(string studyUid )
        {
            return OhifService.GetStudies (new ObjectId ( ) {StudyInstanceUID = studyUid}) ;
        }
    }
}