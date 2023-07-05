
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace DICOMcloud.Wado
{
    public class RsStudiesRequestModelBinder : RsRequestModelBinder<IWadoRsStudiesRequest>, IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(IWadoRsStudiesRequest))
            {
                throw new NotImplementedException();
            }
        }
    }
}
