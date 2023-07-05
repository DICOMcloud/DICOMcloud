
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace DICOMcloud.Wado
{
    public class RsObjectRequestModelBinder : RsRequestModelBinder<IWadoRsInstanceRequest>, IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(IWadoRsInstanceRequest))
            {
                throw new NotImplementedException();
            }
        }
    }
}
