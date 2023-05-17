using Microsoft.AspNetCore.Mvc;

namespace DICOMcloud.Wado.Controllers
{

    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            return Redirect ( "/swagger/" ) ;
        }
   }
}
