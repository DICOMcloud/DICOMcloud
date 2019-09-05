using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using NLog;
using static System.Diagnostics.Trace;

namespace DICOMcloud.Wado
{
  
  

    public class CustomLogHandler: IHttpModule
    {
        public CustomLogHandler() { }
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(BeginRequest);
        } 
        
        static readonly ILogger _logRequets=LogManager.GetLogger("Request");        
        private static void BeginRequest(object sender, EventArgs e)
        {
            if (sender != null && sender is HttpApplication)
            {
                var request = (sender as HttpApplication).Request;
                if (request != null )
                {
                    _logRequets.Debug($"{request.RawUrl}");
                }
            }
        }
    }
}