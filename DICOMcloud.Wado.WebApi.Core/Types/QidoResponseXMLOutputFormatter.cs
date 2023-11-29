
using DICOMcloud.DataAccess;
using DICOMcloud.Media;
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace DICOMcloud.Wado.WebApi.Core.Types
{
    public class QidoResponseXMLOutputFormatter : TextOutputFormatter
    {
        public QidoResponseXMLOutputFormatter() 
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("multipart / related; type = \"application/dicom+xml\"" ));

            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type? type)
                => typeof(QidoResponse).IsAssignableFrom(type);

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            //TODO: optimize this so the multipartContent is writing directly to the response
            var multiContent = new MultipartContent("related", MultipartResponseHelper.DicomDataBoundary);
            QidoResponse? qidoResponse = context.Object as QidoResponse;
            var response = context.HttpContext.Response;

            if (qidoResponse != null && qidoResponse.Result != null && qidoResponse.Result.TotalCount > 0) 
            {
                foreach (var result in qidoResponse.Result.Result)
                {
                    XmlDicomConverter converter = new XmlDicomConverter();

                    MultipartResponseHelper.AddMultipartContent ( multiContent,
                                                                  new WadoResponse (new MemoryStream(Encoding.ASCII.GetBytes(converter.Convert(result))),
                                                                                    MimeMediaTypes.XmlDicom));
                }

                multiContent.Headers.ContentType.Parameters.Add(new System.Net.Http.Headers.NameValueHeaderValue("type",
                                                                "\"" + MimeMediaTypes.XmlDicom + "\""));
            }

            return multiContent.CopyToAsync(response.Body);
        }
    }
}
