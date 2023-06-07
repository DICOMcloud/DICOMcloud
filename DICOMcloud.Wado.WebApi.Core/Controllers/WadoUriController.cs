using System.Net;
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc;

namespace DICOMcloud.Wado.Controllers
{
    [ApiController]
    public class WadoUriController : ControllerBase
    {
        IWadoUriService ServiceHandler { get; set; }

        public WadoUriController ( IWadoUriService serviceHandler )
        {
            ServiceHandler = serviceHandler ;
        }

        [HttpGet]
        [Route("wadouri")]
        [Route("api/wadouri")]       
        public HttpResponseMessage Get
        (
            [ModelBinder(typeof(UriRequestModelBinder))]
            IWadoUriRequest request
        )
        {
            if (null == request) { return new HttpResponseMessage(HttpStatusCode.BadRequest ); }

            return ServiceHandler.GetInstance(request);
      }
   }
}
