using System.Web.Mvc;

namespace DICOMcloud.Wado.Controllers
{
    [LogAction]
    public class SwaggerController : Controller
    {
        // GET: Swagger
        public ActionResult Index()
        {
            return File ( "~/swagger/index.html", "text/html" ) ;
        }

        public ActionResult Json()
        {
            return File ( "~/swagger/swagger.json", "application/json") ;
        }
    }
}