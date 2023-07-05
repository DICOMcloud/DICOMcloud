
using System;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModelBinding = Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DICOMcloud.Wado
{
    public class QidoRequestModelBinder : System.Web.Http.ModelBinding.IModelBinder,ModelBinding.IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, System.Web.Http.ModelBinding.ModelBindingContext bindingContext)
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

        public async Task BindModelAsync(ModelBinding.ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(IQidoRequestModel))
            {
                throw new NotImplementedException();
            }
        }

        private class Constants
        {
            public const string ErrorBindingModel = "Cannot convert request to a QIDO-RS valid request" ;
        }
    }
}
