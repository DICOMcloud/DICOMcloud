
using DICOMcloud.DataAccess;
using DICOMcloud.Media;
using DICOMcloud.Pacs;
using DICOMcloud.Wado.Models;
using FellowOakDicom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Text;

namespace DICOMcloud.Wado.WebApi.Core.Types
{
    public class QidoResult : IActionResult
    {
        public const string Instance_Header_Name = "X-Dicom-Instance";
        public QidoResponse Response {  get; set;}
        private static MediaTypeHeaderValue XML_MEDIA_TYPE = MediaTypeHeaderValue.Parse("multipart/related; type = \"application/dicom+xml\"" );
        private static MediaTypeHeaderValue DJSON_MEDIA_TYPE = MediaTypeHeaderValue.Parse(MimeMediaTypes.JsonDicom);
        private static MediaTypeHeaderValue JSON_MEDIA_TYPE = MediaTypeHeaderValue.Parse(MimeMediaTypes.Json);
        private static MediaTypeHeaderValue ANY_MEDIA_TYPE = MediaTypeHeaderValue.Parse(MimeMediaTypes.Any);

        public QidoResult(QidoResponse response) 
        {
            Response = response;
        }


        public Task ExecuteResultAsync(ActionContext context)
        {

            var qidoResponse = Response;
            var httpContext = context.HttpContext;
            var response = httpContext.Response;
            var archiveService = context.HttpContext.RequestServices.GetService<IObjectArchieveQueryService>();

            response.StatusCode = (int)HttpStatusCode.OK;

            AddResponseHeaders(qidoResponse, response, archiveService);

            foreach ( var accept in Response.Request.AcceptHeader)
            { 
                if (accept.MediaType == XML_MEDIA_TYPE.MediaType) 
                {
                    response.ContentType = accept.MediaType.Value;

                    return CreateXMLResponse(Response.Result.Result).CopyToAsync(response.Body);
                }
                else if ( accept.MediaType == DJSON_MEDIA_TYPE.MediaType || 
                          accept.MediaType == JSON_MEDIA_TYPE.MediaType)
                {

                    response.ContentType = accept.MediaType.Value;
                    return httpContext.Response.WriteAsync(CreateJsonResponse(qidoResponse.Result.Result));
                }

            }

            response.ContentType = MimeMediaTypes.Json;

            return httpContext.Response.WriteAsync(CreateJsonResponse(qidoResponse.Result.Result));
        }

        private MultipartContent CreateXMLResponse(IEnumerable<DicomDataset> results)
        {
            var multiContent = new MultipartContent("related", MultipartResponseHelper.DicomDataBoundary);
            QidoResponse? qidoResponse = Response;

            if (qidoResponse != null && qidoResponse.Result != null && qidoResponse.Result.TotalCount > 0)
            {
                foreach (var result in qidoResponse.Result.Result)
                {
                    XmlDicomConverter converter = new XmlDicomConverter();

                    MultipartResponseHelper.AddMultipartContent(multiContent,
                                                                  new WadoResponse(new MemoryStream(Encoding.ASCII.GetBytes(converter.Convert(result))),
                                                                                    MimeMediaTypes.XmlDicom));
                }

                multiContent.Headers.ContentType.Parameters.Add(new System.Net.Http.Headers.NameValueHeaderValue("type",
                                                                "\"" + MimeMediaTypes.XmlDicom + "\""));
            }

            return multiContent;
        }

        private static string CreateJsonResponse(IEnumerable<DicomDataset> results)
        {
            StringBuilder jsonReturn = new StringBuilder("[");

            JsonDicomConverter converter = new JsonDicomConverter() { IncludeEmptyElements = true };
            int count = 0;


            foreach (var dsResponse in results)
            {
                count++;

                jsonReturn.AppendLine(converter.Convert(dsResponse));

                jsonReturn.Append(",");
            }

            if (count > 0)
            {
                jsonReturn.Remove(jsonReturn.Length - 1, 1);
            }

            jsonReturn.Append("]");

            return jsonReturn.ToString();
        }

        private static void AddPaginationHeders(QidoResponse qidoResponse, HttpResponse response)
        {
            LinkHeaderBuilder headerBuilder = new LinkHeaderBuilder();


            response.Headers.Add("link",
                                  headerBuilder.GetLinkHeader(qidoResponse.Result, response.HttpContext.Request.GetDisplayUrl())); //  HttpContext.Current.Request.Url.AbsoluteUri));

            response.Headers.Add("X-Total-Count", qidoResponse.Result.TotalCount.ToString());

            response.Headers.Add("Access-Control-Expose-Headers", "link, X-Total-Count");
            response.Headers.Add("Access-Control-Allow-Headers", "link, X-Total-Count");

            if (qidoResponse.Result.TotalCount > qidoResponse.Result.Result.Count())
            {
                if (!qidoResponse.Request.Limit.HasValue ||
                     (qidoResponse.Request.Limit.HasValue && qidoResponse.Request.Limit.Value > qidoResponse.Result.PageSize))
                {
                    response.Headers.Add("Warning", "299 " + "DICOMcloud" +
                    "  \"The number of results exceeded the maximum supported by the server. Additional results can be requested.\"");

                    //DICOM: http://dicom.nema.org/dicom/2013/output/chtml/part18/sect_6.7.html
                    //Warning: 299 {SERVICE}: "The number of results exceeded the maximum supported by the server. Additional results can be requested.
                }
            }
        }

        private void AddPreviewInstanceHeader(IEnumerable<DicomDataset> results, HttpResponse response, IObjectArchieveQueryService queryService)
        {
            response.Headers.Add("Access-Control-Expose-Headers", Instance_Header_Name);

            foreach (var result in results)
            {
                var queryDs = DefaultDicomQueryElements.GetDefaultInstanceQuery();
                string studyUid = result.GetSingleValueOrDefault<string>(DicomTag.StudyInstanceUID, "");
                string seriesUid = result.GetSingleValueOrDefault<string>(DicomTag.SeriesInstanceUID, "");
                var queryOptions = new QueryOptions();

                queryDs.AddOrUpdate(DicomTag.StudyInstanceUID, studyUid);
                queryDs.AddOrUpdate(DicomTag.SeriesInstanceUID, seriesUid);
                queryOptions.Limit = 1;
                queryOptions.Offset = 0;

                var instances = queryService.FindObjectInstances(queryDs, queryOptions);
                string? queryStudyUid = null, querySeriesUid = null, queryInstanceUid = null;

                foreach (var instance in instances)
                {
                    queryStudyUid = instance.GetSingleValueOrDefault<string>(DicomTag.StudyInstanceUID, "");
                    querySeriesUid = instance.GetSingleValueOrDefault<string>(DicomTag.SeriesInstanceUID, "");
                    queryInstanceUid = instance.GetSingleValueOrDefault<string>(DicomTag.SOPInstanceUID, "");

                    break;
                }

                if (string.IsNullOrWhiteSpace(queryStudyUid) || string.IsNullOrWhiteSpace(querySeriesUid) || string.IsNullOrWhiteSpace(queryInstanceUid))
                {
                    response.Headers.Add(Instance_Header_Name, "");
                }
                else
                {
                    response.Headers.Add(Instance_Header_Name, string.Format("{0}:{1}:{2}", queryStudyUid, querySeriesUid, queryInstanceUid));
                }
            }
        }

        private void AddResponseHeaders (QidoResponse qidoResponse, HttpResponse response, IObjectArchieveQueryService queryService)
        {
            //special parameters, if included, a representative instance UID (first) will be returned in header for each DS result
            if (/*response.IsSuccessStatusCode && */ qidoResponse.Request.Query.CustomParameters.ContainsKey("_instance-header"))
            {
                AddPreviewInstanceHeader(qidoResponse.Result.Result, response, queryService);
            }

            AddPaginationHeders(qidoResponse, response);
        }


    }
}
