using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc;

namespace DICOMcloud.Wado.Controllers
{
    public class WadoUriController : ControllerBase
    {
        IWadoUriService ServiceHandler { get; set; }

        public WadoUriController(IWadoUriService serviceHandler)
        {
            ServiceHandler = serviceHandler;
        }

        // [Route("wadouri")]
        // [Route("api/wadouri")]
        // public async Task<HttpResponseMessage> Get
        // (
        //     [ModelBinder(typeof(UriRequestModelBinder))]
        //     IWadoUriRequest request
        // )
        // {
        //     if (null == request)
        //         { return new HttpResponseMessage(HttpStatusCode.BadRequest); }

        //     return await ServiceHandler.GetInstance(request);
        // }
    }
}
