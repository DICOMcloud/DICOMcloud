using System.Web.Mvc;

namespace DICOMcloud.Wado.Controllers
{
    [LogAction]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect ( "/swagger/" ) ;
        }
   }
}
