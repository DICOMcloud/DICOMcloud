
using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public class QidoRequestModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(IQidoRequestModel))
            {
                IQidoRequestModel result ;
                
                var theValue = bindingContext.ValueProvider.GetValue ( bindingContext.ModelName);

                if ( new QidoRequestModelConverter ( ).TryParse ( actionContext.Request, bindingContext, out result) )
                { 
                    bindingContext.Model = result;
               
                    return true;
                }
                else
                { 
                    throw new ArgumentException ( Constants.ErrorBindingModel );
                }
            }

            return false ;
        }

        private class Constants
        {
            public const string ErrorBindingModel = "Cannot convert request to a QIDO-RS valid request" ;
        }
    }
}
