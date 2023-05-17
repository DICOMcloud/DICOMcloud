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

        public HttpResponseMessage GetSeries(IStudyId studyId, ISeriesId seriesId)

        {
            IEnumerable<DicomDataset> instances = QueryInstances(studyId, seriesId);

            OHIFViewerModel result = GetOHIFModel(instances, studyId);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            response.Content = new StringContent(result.ToJson(true));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        }

        public HttpResponseMessage GetInstances(IStudyId studyId, ISeriesId seriesId, IObjectId sopUid)

        {
            IEnumerable<DicomDataset> instances = QueryInstances(studyId, seriesId, sopUid);

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


        protected virtual IQueryOptions GetQueryOptions(IStudyId studyId, ISeriesId seriesId)
        {
            return CreateNewQueryOptions();
        }

        protected virtual IQueryOptions GetQueryOptions(IStudyId studyId, ISeriesId seriesId, IObjectId sopId)
        {
            return CreateNewQueryOptions();
        }

        protected virtual IEnumerable<DicomDataset> QueryInstances (IStudyId studyId)
        {
            DicomDataset ds = new DicomDataset().NotValidated();

            ds.Add(Dicom.DicomTag.StudyInstanceUID, studyId.StudyInstanceUID);
            ds.Add(DicomTag.SeriesInstanceUID, "");
            ds.Add(DicomTag.PatientID, "");
            ds.Add(DicomTag.PatientName, "");
            ds.Add(DicomTag.SeriesDescription, "");
            ds.Add(DicomTag.SOPInstanceUID, "");
            ds.Add(DicomTag.NumberOfFrames, "");

            return QueryService.FindObjectInstances (ds, GetQueryOptions(studyId));
            
        }

        protected virtual IEnumerable<DicomDataset> QueryInstances(IStudyId studyId,ISeriesId seriesId)
        {
            DicomDataset ds = new DicomDataset().NotValidated();

            ds.Add(Dicom.DicomTag.StudyInstanceUID, studyId.StudyInstanceUID);
            ds.Add(DicomTag.SeriesInstanceUID, seriesId.SeriesInstanceUID);
            ds.Add(DicomTag.PatientID, "");
            ds.Add(DicomTag.PatientName, "");
            ds.Add(DicomTag.SeriesDescription, "");
            ds.Add(DicomTag.SOPInstanceUID, "");
            ds.Add(DicomTag.NumberOfFrames, "");

            return QueryService.FindObjectInstances(ds, GetQueryOptions(studyId, seriesId));

        }

        protected virtual IEnumerable<DicomDataset> QueryInstances(IStudyId studyId, ISeriesId seriesId, IObjectId sopId)
        {
            DicomDataset ds = new DicomDataset().NotValidated();

            ds.Add(Dicom.DicomTag.StudyInstanceUID, studyId.StudyInstanceUID);
            ds.Add(DicomTag.SeriesInstanceUID, seriesId.SeriesInstanceUID);
            ds.Add(DicomTag.PatientID, "");
            ds.Add(DicomTag.PatientName, "");
            ds.Add(DicomTag.SeriesDescription, "");
            ds.Add(DicomTag.SOPInstanceUID, sopId.SOPInstanceUID);
            ds.Add(DicomTag.NumberOfFrames, "");

            return QueryService.FindObjectInstances(ds, GetQueryOptions(studyId, seriesId, sopId));

        }

        protected virtual OHIFViewerModel GetOHIFModel(IEnumerable<DicomDataset> instances, IStudyId studyId)
        {
            Dictionary<string, OHIFStudy>  studies = new Dictionary<string, OHIFStudy>();
            Dictionary<string, OHIFSeries> series = new Dictionary<string, OHIFSeries>();
            OHIFViewerModel                result = new OHIFViewerModel();


            result.TransactionId = Guid.NewGuid().ToString();

            foreach (var instance in instances)
            {
                var currentStudyUid = instance.GetSingleValue<string>(DicomTag.StudyInstanceUID);
                var currentSeriesUid = instance.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
                var currentInstanceUid = instance.GetSingleValue<string>(DicomTag.SOPInstanceUID);
                OHIFInstance ohifInstance = new OHIFInstance() { SopInstanceUid = currentInstanceUid };
                OHIFStudy ohifStudy;
                OHIFSeries ohifSeries;


                if (!studies.TryGetValue(currentStudyUid, out ohifStudy))
                {
                    ohifStudy = new OHIFStudy() { StudyInstanceUid = currentStudyUid };

                    ohifStudy.PatientName = instance.GetSingleValueOrDefault<string>(DicomTag.PatientName, "");

                    result.Studies.Add(ohifStudy);

                    studies.Add ( currentStudyUid, ohifStudy) ;
                }

                if (!series.TryGetValue(currentSeriesUid, out ohifSeries))
                {
                    ohifSeries = new OHIFSeries() { SeriesInstanceUid = currentSeriesUid };

                    ohifSeries.SeriesDescription = instance.GetSingleValueOrDefault<string>(DicomTag.SeriesDescription, "");

                    ohifStudy.SeriesList.Add(ohifSeries);

                    series.Add(currentSeriesUid, ohifSeries);
                }

                ohifInstance.Rows = 1;
                ohifInstance.Url = CreateOHIFUrl (instance, studyId);
                ohifInstance.NumberOfFrames = instance.GetSingleValueOrDefault<int?>(DicomTag.NumberOfFrames, null);

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
