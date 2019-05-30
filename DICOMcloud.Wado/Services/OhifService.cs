using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Dicom;
using DICOMcloud.DataAccess;
using DICOMcloud.Extensions;
using DICOMcloud.Pacs;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public class OhifService : IOhifService
    {
        protected IObjectArchieveQueryService QueryService {get; set;}
        protected IRetrieveUrlProvider UrlProvier {get; set;}
                  
        public OhifService 
        ( 
            IObjectArchieveQueryService queryService, 
            IRetrieveUrlProvider urlProvier 
        )
        {
            QueryService = queryService ;
            UrlProvier   = urlProvier ;

            UrlProvier.PreferWadoUri = true ;
        }


        public HttpResponseMessage GetStudies (IStudyId studyId)

        {
            IEnumerable<DicomDataset> instances = QueryInstances (studyId);

            OHIFViewerModel result = GetOHIFModel(instances, studyId);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            response.Content = new StringContent(result.ToJson(true));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        }

        protected virtual IQueryOptions CreateNewQueryOptions()
        {
            return new QueryOptions();
        }

        protected virtual IQueryOptions GetQueryOptions (IStudyId studyId)
        {
            return CreateNewQueryOptions();
        }

        protected virtual IEnumerable<DicomDataset> QueryInstances (IStudyId studyId)
        {
            Dicom.DicomDataset ds = new Dicom.DicomDataset();

            ds.Add(Dicom.DicomTag.StudyInstanceUID, studyId.StudyInstanceUID);
            ds.Add(DicomTag.SeriesInstanceUID, "");
            ds.Add(DicomTag.PatientID, "");
            ds.Add(DicomTag.PatientName, "");
            ds.Add(DicomTag.SeriesDescription, "");
            ds.Add(DicomTag.SOPInstanceUID, "");
            ds.Add(DicomTag.NumberOfFrames, "");

            return QueryService.FindObjectInstances (ds, GetQueryOptions(studyId));
            
        }

        protected virtual OHIFViewerModel GetOHIFModel(IEnumerable<DicomDataset> instances, IStudyId studyId)
        {
            Dictionary<string, OHIFStudy>  studies = new Dictionary<string, OHIFStudy>();
            Dictionary<string, OHIFSeries> series = new Dictionary<string, OHIFSeries>();
            OHIFViewerModel                result = new OHIFViewerModel();


            result.TransactionId = Guid.NewGuid().ToString();

            foreach (var instance in instances)
            {
                var currentStudyUid = instance.Get<string>(DicomTag.StudyInstanceUID);
                var currentSeriesUid = instance.Get<string>(DicomTag.SeriesInstanceUID);
                var currentInstanceUid = instance.Get<string>(DicomTag.SOPInstanceUID);
                OHIFInstance ohifInstance = new OHIFInstance() { SopInstanceUid = currentInstanceUid };
                OHIFStudy ohifStudy;
                OHIFSeries ohifSeries;


                if (!studies.TryGetValue(currentStudyUid, out ohifStudy))
                {
                    ohifStudy = new OHIFStudy() { StudyInstanceUid = currentStudyUid };

                    ohifStudy.PatientName = instance.Get<string>(DicomTag.PatientName, "");

                    result.Studies.Add(ohifStudy);

                    studies.Add ( currentStudyUid, ohifStudy) ;
                }

                if (!series.TryGetValue(currentSeriesUid, out ohifSeries))
                {
                    ohifSeries = new OHIFSeries() { SeriesInstanceUid = currentSeriesUid };

                    ohifSeries.SeriesDescription = instance.Get<string>(DicomTag.SeriesDescription, "");

                    ohifStudy.SeriesList.Add(ohifSeries);

                    series.Add(currentSeriesUid, ohifSeries);
                }

                ohifInstance.Rows = 1;
                ohifInstance.Url = CreateOHIFUrl (instance, studyId);
                ohifInstance.NumberOfFrames = instance.Get<int?>(DicomTag.NumberOfFrames, null);

                ohifSeries.Instances.Add(ohifInstance);
            }

            return result;
        }

        protected virtual string CreateOHIFUrl (DicomDataset instance, IStudyId studyId)
        {
            var url = UrlProvier.GetInstanceUrl(DicomObjectIdFactory.Instance.CreateObjectId(instance));

            url = url.Remove(0, url.IndexOf(":") + 1);
            
            return "dicomweb:" + url;
        }
    }
}
