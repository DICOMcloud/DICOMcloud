using System.Web.Mvc;

namespace DICOMcloud.Wado.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect ( "/swagger/" ) ;
        }
   }
}
