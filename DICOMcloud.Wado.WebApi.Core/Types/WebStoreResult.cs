using DICOMcloud.Media;
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DICOMcloud.Wado.WebApi
{
    public class WebStoreResult : IActionResult
    {
        public WebStoreResult(WebStoreResponse response) 
        { 
            Response = response;
        }

        public WebStoreResponse Response { get; private set; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var result = new XmlDicomConverter().Convert(Response.GetResponseContent());
            context.HttpContext.Response.StatusCode = (int)Response.HttpStatus;
            context.HttpContext.Response.ContentType = MimeMediaTypes.XML; //TODO: should it be dicom+xml? standard is not clear

            return context.HttpContext.Response.WriteAsync(result);

            //if (!string.IsNullOrWhiteSpace(storeResponse.StatusMessage))
            //{
            //    statusMessage =
            //        storeResponse.StatusMessage.Length > 512 ?
            //        storeResponse.StatusMessage.Substring(0, 512) :
            //        storeResponse.StatusMessage;
            //}


            //await httpResponse.WriteAsync(result);
            //httpResponse.ContentType = "application/dicom+xml";
        }
    }
}
