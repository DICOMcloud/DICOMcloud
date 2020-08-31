// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Web;
// using System.Web.Configuration;
// using System.Web.Http;
// using System.Web.Mvc;
// using System.Web.Optimization;
// using System.Web.Routing;
// using Microsoft.ApplicationInsights.Extensibility;
// using WebApi.StructureMap;

// namespace DICOMcloud.Wado
// {
//     public class WebApiApplication : System.Web.HttpApplication
//     {
//         protected void Application_Start()
//         {
//             AreaRegistration.RegisterAllAreas();
//             GlobalConfiguration.Configure(WebApiConfig.Register);
//             FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
//             RouteConfig.RegisterRoutes(RouteTable.Routes);
//             BundleConfig.RegisterBundles(BundleTable.Bundles);

//             AddInsights ( ) ;

//             GlobalConfiguration.Configure(DICOMcloudBuilder.ConfigureLogging);

//             GlobalConfiguration.Configuration.UseStructureMap(x =>
//             {
//                 x.AddRegistry<DICOMcloudBuilder>();
//             });
//         }

//         protected virtual void AddInsights ( )
//         {
//             if( WebConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"] != null )
//             {
//                 TelemetryConfiguration.Active.InstrumentationKey = WebConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];    

//                 System.Diagnostics.Trace.TraceInformation ( "Insights key added" ) ;
//             }
//         }
//     }
// }
