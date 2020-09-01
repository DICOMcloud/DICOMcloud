
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom.Imaging.Mathematics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DICOMcloud.Wado
{
    public class RsRequestModelBinder<T> : IModelBinder where T : class
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(T))
            {
                T result ;
                var theValue = bindingContext.ValueProvider.GetValue ( bindingContext.ModelName);
                
                
                if ( GetConverter ( ).TryParse ( bindingContext.ActionContext.HttpContext.Request.ToHttpRequestMessage(), bindingContext, out result) )
                { 
                    bindingContext.Model = result;
                    return Task.CompletedTask;
                }
                else
                { 
                    bindingContext.ModelState.AddModelError( bindingContext.ModelName, "Cannot convert request to a Wado-RS valid request");
                    throw new ArgumentException ("Invalid binding");
                }
            }

            throw new Exception("Invalid binding");
        }

        protected virtual RsRequestModelConverter<T> GetConverter ( ) 
        {
            return new RsRequestModelConverter<T> ( ) ;
        }
    }
}
