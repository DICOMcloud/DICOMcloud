// using Microsoft.Azure;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Web.Http;
// using System.Web.Http.Cors;


// namespace DICOMcloud.Wado
// {
//     public static class WebApiConfig
//     {
//         public static void Register(HttpConfiguration config)
//         {
//             string enabled = CloudConfigurationManager.GetSetting("cors:enabled");

//             if (bool.TryParse(enabled, out bool isEnabled) && isEnabled)
//             {
//                 string origins = CloudConfigurationManager.GetSetting("cors:origins");
//                 string headers = CloudConfigurationManager.GetSetting("cors:headers");
//                 string methods = CloudConfigurationManager.GetSetting("cors:methods");

//                 config.MessageHandlers.Add(new PreflightRequestsHandler(origins, headers, methods));
//                 config.EnableCors(new EnableCorsAttribute(origins, headers, methods));
//             }

//             // Web API routes
//             config.MapHttpAttributeRoutes();

//             config.Routes.MapHttpRoute(
//                 name: "DefaultApi",
//                 routeTemplate: "api/{controller}/{id}",
//                 defaults: new
//                 {
//                     id = RouteParameter.Optional
//                 }
//             );
//         }
//     }
// }
