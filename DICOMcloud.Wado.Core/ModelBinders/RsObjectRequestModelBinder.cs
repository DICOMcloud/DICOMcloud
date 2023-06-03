
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace DICOMcloud.Wado
{
    public class RsObjectRequestModelBinder : RsRequestModelBinder<IWadoRsInstanceRequest>, IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
