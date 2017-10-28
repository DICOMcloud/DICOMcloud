using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Dicom;
using DICOMcloud.Extensions;
using DICOMcloud.Pacs;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public class OhifService : IOhifService
    {
        protected IObjectArchieveQueryService QueryService {get; set;}
        protected IRetieveUrlProvider UrlProvier {get; set;}

        public OhifService ( IObjectArchieveQueryService queryService, IRetieveUrlProvider urlProvier )
        {
            QueryService = queryService ;
            UrlProvier   = urlProvier ;

            UrlProvier.PreferWadoUri = true ;
        }


        public HttpResponseMessage GetStudies ( string studyInstanceUid )

        {
            ICollection<DicomDataset> instances = QueryInstances(studyInstanceUid);

            OHIFViewerModel result = GetOHIFModel(instances);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            response.Content = new StringContent(result.ToJson(true));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        }

        private ICollection<DicomDataset> QueryInstances(string studyInstanceUid)
        {
            Dicom.DicomDataset ds = new Dicom.DicomDataset();

            ds.Add(Dicom.DicomTag.StudyInstanceUID, studyInstanceUid);
            ds.Add(DicomTag.SeriesInstanceUID, "");
            ds.Add(DicomTag.PatientID, "");
            ds.Add(DicomTag.SeriesDescription, "");
            ds.Add(DicomTag.SOPInstanceUID, "");
            
            return QueryService.FindObjectInstances(ds, null);
            
        }

        private OHIFViewerModel GetOHIFModel(ICollection<DicomDataset> instances)
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
                }

                if (!series.TryGetValue(currentSeriesUid, out ohifSeries))
                {
                    ohifSeries = new OHIFSeries() { SeriesInstanceUid = currentSeriesUid };

                    ohifSeries.SeriesDescription = instance.Get<string>(DicomTag.SeriesDescription, "");

                    ohifStudy.SeriesList.Add(ohifSeries);

                    series.Add(currentSeriesUid, ohifSeries);
                }

                ohifInstance.Rows = 1;
                ohifInstance.Url = CreateOHIFUrl(instance );

                ohifSeries.Instances.Add(ohifInstance);
            }

            return result;
        }

        private string CreateOHIFUrl(DicomDataset instance)
        {
            var url = UrlProvier.GetInstanceUrl(DicomObjectIdFactory.Instance.CreateObjectId(instance));

            url = url.Remove(0, url.IndexOf(":") + 1);
            
            return "dicomweb:" + url;
        }
    }
}
