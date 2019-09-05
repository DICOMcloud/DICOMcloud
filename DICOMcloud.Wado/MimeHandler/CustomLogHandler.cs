using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Routing;
using NLog;
using static System.Diagnostics.Trace;

namespace DICOMcloud.Wado
{



    public class CustomLogHandler : IHttpModule
    {
        public CustomLogHandler()
        {
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(BeginRequest);
        }

        static readonly ILogger _logRequets = LogManager.GetLogger("Request");

        private static void BeginRequest(object sender, EventArgs e)
        {
            if (sender != null && sender is HttpApplication)
            {
                var httpApp = sender as HttpApplication;
                var request = (sender as HttpApplication).Request;
                if (request != null)
                {
                    if (_logRequets.IsDebugEnabled)
                    {
                        var Context = httpApp.Context;
                        _logRequets.Debug($"{request.RawUrl}");
                        HttpContextBase ctx = new HttpContextWrapper(Context);
                        foreach (Route rte in RouteTable.Routes)
                        {
                            var routeData = RouteTable.Routes.GetRouteData(ctx);

                            if (routeData != null)
                            {
                                if (rte.RouteHandler.GetType().Name == "MvcRouteHandler")
                                {
                                    _logRequets.Debug(string.Format("Following: {1} for request: {0}",
                                        Context.Request.Url,
                                        rte.Url));
                                }
                                else
                                {

                                    _logRequets.Trace(string.Format("Ignoring route: {1} for request: {0}",
                                        Context.Request.Url, rte.Url));
                                }

                                break;
                            }
                        }

                    }
                }
            }
        }
    }
}