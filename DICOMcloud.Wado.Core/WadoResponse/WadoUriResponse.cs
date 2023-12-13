
using DICOMcloud.DataAccess;
using DICOMcloud.Wado.Models;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Net.Http;

namespace DICOMcloud.Wado
{
    public class WadoUriResponse
    {
        public HttpContent    Content     { get; set; }
        public string         RedirectUrl { get;  set; }
        public HttpStatusCode StatusCode  { get; set; }

        public WadoUriResponse() 
        { 
            StatusCode = HttpStatusCode.OK;
        }

        public WadoUriResponse
        (
            HttpContent content
        ) 
        {
            Content    = content;
            StatusCode = HttpStatusCode.OK;
        }

        public WadoUriResponse
        (
            string redirectUrl
        )
        {
            RedirectUrl = redirectUrl;
            StatusCode = HttpStatusCode.Redirect;
        }
    }
}
