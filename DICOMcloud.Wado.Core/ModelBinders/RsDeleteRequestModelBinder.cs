using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace DICOMcloud.Wado
{
    public class RsDeleteRequestModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(WebDeleteRequest))
            {
                return Task.CompletedTask;
            }

            WebDeleteRequest result;

            if (new DeleteRsRequestModelConverter().TryParse(bindingContext, out result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);;

                return Task.CompletedTask;
            }
            else
            {
                bindingContext.Model = ModelBindingResult.Failed();
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Cannot convert value to Location");
                return Task.CompletedTask;
            }

        }
    }
}
