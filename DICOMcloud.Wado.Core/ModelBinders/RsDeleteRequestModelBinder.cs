
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado
{
    public class RsDeleteRequestModelBinder : RsRequestModelBinder<WebDeleteRequest> ,IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(WebDeleteRequest))
            {
                throw new NotImplementedException();
            }
        }

        protected override RsRequestModelConverter<WebDeleteRequest> GetConverter ( )
        {
            return new DeleteRsRequestModelConverter ( ) ;
        }
    }
}
