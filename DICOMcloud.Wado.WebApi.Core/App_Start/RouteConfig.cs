using System.Web.Http;
using System.Web.Mvc;

namespace DICOMcloud.Wado
{
    public static class RouteConfig//: Ihttpconf
   {
      public static void RegisterRoutes(this WebApplication app)
      {
            app.Map("/{resource}.axd/{*pathInfo}", delegate { });

            app.MapControllerRoute(
                name: "DefaultApi",
                pattern: "api/{controller = \"Home\"}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional
                });            
        }
   }
}
