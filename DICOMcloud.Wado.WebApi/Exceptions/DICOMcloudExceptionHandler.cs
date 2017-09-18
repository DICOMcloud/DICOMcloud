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
using System.Web.Http.Results;

namespace DICOMcloud.Wado.WebApi.Exceptions
{
    public class DICOMcloudExceptionHandler : ExceptionHandler
    {
        //implementation of the DefaultExcpetionHandler:
        //https://aspnetwebstack.codeplex.com/SourceControl/latest#src/System.Web.Http/ExceptionHandling/DefaultExceptionHandler.cs
        //This implementation is needed becuase:
        //https://stackoverflow.com/questions/24189315/exceptions-in-asp-net-web-api-custom-exception-handler-never-reach-top-level-whe/24634485#24634485
        public override void Handle(ExceptionHandlerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ExceptionContext   exceptionContext = context.ExceptionContext;
            Exception          exception        = exceptionContext.Exception;
            HttpRequestMessage request          = exceptionContext.Request;

            if (request == null)
            {
                base.Handle ( context ) ;
            }

            if (exceptionContext.CatchBlock == ExceptionCatchBlocks.IExceptionFilter)
            {
                // The exception filter stage propagates unhandled exceptions by default (when no filter handles the
                // exception).
                return;
            }

            if ( exception is DCloudException )
            {
                context.Result = new ResponseMessageResult ( request.CreateErrorResponse ( HttpStatusCode.BadRequest,
                                                                                           exception.Message));
            }
            else
            {
                context.Result = new ResponseMessageResult ( request.CreateErrorResponse (HttpStatusCode.InternalServerError, ""));
            }
        }
    
       public override bool ShouldHandle(ExceptionHandlerContext context)
       {
            return true ;
       }
    }
}