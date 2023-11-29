
using DICOMcloud.DataAccess;
using DICOMcloud.Wado.Models;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Net.Http;

namespace DICOMcloud.Wado
{
    public class WadoRsResponse
    {
        public HttpContent          Content { get; set; }
        public HttpStatusCode       StatusCode { get; set; }

        public WadoRsResponse () 
        { 
            StatusCode = HttpStatusCode.OK;
        }

        public WadoRsResponse
        (
            IWadoRequestHeader header, 
            IObjectId request,
            MultipartContent content
        ) 
        {
            Content    = content;
            StatusCode = HttpStatusCode.OK;
        }
    }
}
