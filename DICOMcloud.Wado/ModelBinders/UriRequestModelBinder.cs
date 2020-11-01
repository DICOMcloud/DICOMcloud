
using System;
using System.Threading.Tasks;
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DICOMcloud.Wado
{
    public class UriRequestModelBinder : IModelBinder
    {
        public UriRequestModelBinder() { }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(IWadoUriRequest))
            {
                throw new ArgumentException ("Invalid model type");
            }

            IWadoUriRequest result;

            if (new UriRequestModelConverter().TryParse(bindingContext.ActionContext.HttpContext.Request.ToHttpRequestMessage(), out result))
            {
                bindingContext.Model = result;
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }
            else
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Cannot convert value to Location");
                throw new ArgumentException ("Cannot convert value to Location");
            }
        }
    }
}
