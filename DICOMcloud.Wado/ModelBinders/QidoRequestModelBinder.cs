
using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace DICOMcloud.Wado
{
    public class QidoRequestModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            try
            {
                if (bindingContext.ModelType == typeof(IQidoRequestModel))
                {
                    IQidoRequestModel result ;
                
                    var theValue = bindingContext.ValueProvider.GetValue ( bindingContext.ModelName);
                    if ( new QidoRequestModelConverter ( ).TryParse ( actionContext.Request, out result) )
                    { 
                        bindingContext.Model = result;
               
                        return true;
                    }
                    else
                    { 
                        bindingContext.ModelState.AddModelError( bindingContext.ModelName, Constants.ErrorBindingModel ) ;

                        return false;
                    }
                }

                return false ;
            }
            catch ( Exception ex )
            {
                System.Diagnostics.Trace.TraceError ( ex.ToString ( ) ) ;

                bindingContext.ModelState.AddModelError( bindingContext.ModelName, "Cannot convert request to a QIDO-RS valid request");

                return false;
            }
        }

        private class Constants
        {
            public const string ErrorBindingModel = "Cannot convert request to a QIDO-RS valid request" ;
        }
    }
}
