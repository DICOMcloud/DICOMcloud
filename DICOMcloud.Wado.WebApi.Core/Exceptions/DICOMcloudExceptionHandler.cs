using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
    public class DICOMcloudExceptionHandler : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context == null || context.Exception == null)
            {
                return;
            }

            Exception? exception = context.Exception;

            if ( exception is DCloudException )
            {
                context.Result = new ObjectResult (exception.Message) { StatusCode = (int)HttpStatusCode.BadRequest};
                
                context.ExceptionHandled = true;
            }
        }
    }
}