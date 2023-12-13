using System.Net;
using DICOMcloud.Wado.Models;
using DICOMcloud.Wado.WebApi;
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
        public IActionResult Get
        (
            [ModelBinder(typeof(UriRequestModelBinder))]
            IWadoUriRequest request
        )
        {
            return new WadoUriResult(ServiceHandler.GetInstance(request));
        }
   }
}
