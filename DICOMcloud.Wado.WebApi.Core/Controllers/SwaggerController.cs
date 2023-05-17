using Microsoft.AspNetCore.Mvc;

namespace DICOMcloud.Wado.Controllers
{
    public class SwaggerController : ControllerBase
    {
        // GET: Swagger
        public ActionResult Index()
        {
            return File ( "~/swagger/index.html", "text/html" ) ;
        }

        public ActionResult Json()
        {
            return File ("~/swagger/swagger-restful.json", "application/json") ;
        }
    }
}