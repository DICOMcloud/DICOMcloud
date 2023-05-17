using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;
using Microsoft.ApplicationInsights;

namespace DICOMcloud.Wado.WebApi.Exceptions
{
    public class DICOMcloudExceptionLogger : ExceptionLogger
    {
        private readonly TelemetryClient _telemetryClient = new TelemetryClient();
        
        public override void Log(ExceptionLoggerContext context)
        {
            if (context !=null && context.Exception != null)
            {
                _telemetryClient.TrackException (context.Exception);
            }
        }
    }
}