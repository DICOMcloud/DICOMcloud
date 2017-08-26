using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace DICOMcloud.Wado.WebApi.Exceptions
{
    public class DICOMcloudExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            context.Result = new TextPlainErrorResult 
            {
                Request = context.Request,
                Content = "An error has occured." + context.Request.GetCorrelationId ( )  
            };
        }
    
        private class TextPlainErrorResult : IHttpActionResult
        {
            public HttpRequestMessage Request { get; set; }

            public string Content { get; set; }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);


                response.Content = new StringContent(Content);
                response.RequestMessage = Request;
            
                return Task.FromResult(response);
            }
        }
    }
}