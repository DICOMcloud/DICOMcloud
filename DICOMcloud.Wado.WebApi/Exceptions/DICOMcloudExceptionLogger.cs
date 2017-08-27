using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;


namespace DICOMcloud.Wado.WebApi.Exceptions
{
    public class DICOMcloudExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            if (context !=null && context.Exception != null)
            {
                Dicom.Log.LogManager.GetLogger ( "Global" ).Error ( context.Exception.Message, 
                                                                    context.Exception, 
                                                                    context.Request ) ;
            }
        }
    }
}