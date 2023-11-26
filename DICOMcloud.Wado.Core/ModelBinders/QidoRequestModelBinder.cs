
using System;
using System.Threading.Tasks;
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DICOMcloud.Wado
{
    public class QidoRequestModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(IQidoRequestModel))
            {
                IQidoRequestModel result ;
                
                var theValue = bindingContext.ValueProvider.GetValue ( bindingContext.ModelName);

                if ( new QidoRequestModelConverter ( ).TryParse (bindingContext.HttpContext.Request, bindingContext, out result) )
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
               
                    return Task.CompletedTask;
                }
                else
                { 
                    throw new ArgumentException ( Constants.ErrorBindingModel );
                }
            }

            return Task.CompletedTask;
        }

        private class Constants
        {
            public const string ErrorBindingModel = "Cannot convert request to a QIDO-RS valid request" ;
        }
    }
}
