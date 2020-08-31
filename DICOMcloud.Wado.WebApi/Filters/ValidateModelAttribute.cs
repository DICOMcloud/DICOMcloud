using DICOMcloud.Wado.WebApi.Filters;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DICOMcloud.Wado.WebApi.Api.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new ValidationFailedResult(context.ModelState);
            }
        }
    }
}