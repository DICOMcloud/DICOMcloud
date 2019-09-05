using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Cors;
using System.Web.Mvc;
using NLog;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;


namespace DICOMcloud.Wado
{
    public class LogActionAttribute : ActionFilterAttribute
    {
      
        static readonly ILogger _log= LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            if (filterContext.RequestContext.RouteData.Values.ContainsKey("Controller"))
            {

                var controller = filterContext.RequestContext.RouteData.Values["Controller"];
                var action = filterContext.RequestContext.RouteData.Values["Action"];
                _log.Debug($"Processing action {action} in controller {controller}");
            }
            else
            {
                _log.Warn($"no action controller pair found for {filterContext.Request.RequestUri.AbsoluteUri}");
            }

            base.OnActionExecuting(filterContext);
        }
    }

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
                
            // Web API configuration and services
            var cors = new EnableCorsAttribute ("*", "*", "*" );
            config.EnableCors(cors);
             // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional
                }
            );
            var log = LogManager.GetLogger("WebApiConfig");
            log.Info("completed HttpConfigurationRegistration and initialization of WebApiConfig");
        }
    }
}
