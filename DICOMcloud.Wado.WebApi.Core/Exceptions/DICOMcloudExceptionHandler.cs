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
    public class DICOMcloudExceptionHandler
    {
        private RequestDelegate _next;
        private ILogger<DICOMcloudExceptionHandler> _logger;

        public DICOMcloudExceptionHandler(RequestDelegate next, ILogger<DICOMcloudExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            //context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorMessage = "";

            switch (exception)
            {
                case DCloudException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorMessage = ex.Message;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorMessage = "Internal server error!";
                    break;
            }

            _logger.LogError(exception.Message);
            
            await context.Response.WriteAsync(errorMessage);
        }
    }
}