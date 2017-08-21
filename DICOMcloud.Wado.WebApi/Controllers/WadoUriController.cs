using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using DICOMcloud.Wado;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado.Controllers
{
    public class WadoUriController : ApiController
    {
        IWadoUriService ServiceHandler { get; set; }

        public WadoUriController ( IWadoUriService serviceHandler )
        {
            ServiceHandler = serviceHandler ;
        }

        [Route("wadouri")]
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
