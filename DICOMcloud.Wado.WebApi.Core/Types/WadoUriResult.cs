using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DICOMcloud.Wado.WebApi
{
    public class WadoUriResult : IActionResult
    {
        public WadoUriResult(WadoUriResponse response) 
        { 
            Response = response;
        }

        public WadoUriResponse Response { get; private set; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)Response.StatusCode;
            
            if (Response.Content != null)
            {
                context.HttpContext.Response.ContentType = (Response.Content.Headers.ContentType != null) ? 
                    Response.Content.Headers.ContentType.ToString() : 
                    context.HttpContext.Response.ContentType;

                return Response.Content.CopyToAsync(context.HttpContext.Response.BodyWriter.AsStream());
            }
            else if (!string.IsNullOrWhiteSpace(Response.RedirectUrl))
            { 
                context.HttpContext.Response.Headers.Location = Response.RedirectUrl;
            }

            return Task.CompletedTask;
        }
    }
}
