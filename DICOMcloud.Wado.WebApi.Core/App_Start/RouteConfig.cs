using System.Web.Mvc;

namespace DICOMcloud.Wado
{
    public class RouteConfig: Ihttpconf
   {
      public static void RegisterRoutes(RouteCollection routes)
      {
         //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

         routes.m(
             name: "Default",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
         );
      }
   }
}
