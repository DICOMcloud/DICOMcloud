
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace DICOMcloud.Wado
{
    public class RsRequestModelBinder<T> : IModelBinder where T : class
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(T))
            {
                T result ;
                var theValue = bindingContext.ValueProvider.GetValue ( bindingContext.ModelName);
                
                
                if ( GetConverter ( ).TryParse ( actionContext.Request, bindingContext, out result) )
                { 
                    bindingContext.Model = result;
               
                    return true;
                }
                else
                { 
                    bindingContext.ModelState.AddModelError( bindingContext.ModelName, "Cannot convert request to a Wado-RS valid request");

                    return false;
                }
            }

            return false ;
        }

        protected virtual RsRequestModelConverter<T> GetConverter ( ) 
        {
            return new RsRequestModelConverter<T> ( ) ;
        }
    }
}
