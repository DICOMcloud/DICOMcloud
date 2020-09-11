using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace DICOMcloud.Wado.WebApi.Controllers
{
    public class OhifViewerController : ControllerBase
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

        [Route("ohif/study/{studyUid}/series/{seriesUid}/instances")]
        [HttpGet]
        public HttpResponseMessage GetSeries(string studyUid, string seriesUid)
        {
            return OhifService.GetSeries(new ObjectId( ) {StudyInstanceUID = studyUid, SeriesInstanceUID = seriesUid}, new ObjectId() { StudyInstanceUID = studyUid, SeriesInstanceUID = seriesUid});
        }
 
        [Route("ohif/study/{studyUid}/series/{seriesUid}/instances/{sopUid}/frames")]
        [HttpGet]
        public HttpResponseMessage GetInstances(string studyUid, string seriesUid, string sopUid)
        {
            return OhifService.GetInstances(new ObjectId() { StudyInstanceUID = studyUid, SeriesInstanceUID = seriesUid }, new ObjectId() { StudyInstanceUID = studyUid, SeriesInstanceUID = seriesUid }, new ObjectId() { StudyInstanceUID = studyUid, SeriesInstanceUID = seriesUid,SOPInstanceUID = sopUid });
        }
    }
}
