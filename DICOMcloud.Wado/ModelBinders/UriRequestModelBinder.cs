
using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace DICOMcloud.Wado
{
   public class UriRequestModelBinder : IModelBinder
   {
      public UriRequestModelBinder() { }

       public bool BindModel(System.Web.Http.Controllers.HttpActionContext actionContext, ModelBindingContext bindingContext)
       {
         if (bindingContext.ModelType != typeof(IWadoUriRequest))
         {
            return false;
         }
         
         IWadoUriRequest result ;
            
         if ( new UriRequestModelConverter ( ).TryParse ( actionContext.Request, out result) )
         { 
            bindingContext.Model = result;
               
            return true;
         }
         else
         { 
            bindingContext.ModelState.AddModelError( bindingContext.ModelName, "Cannot convert value to Location");
            return false;
         }
       }
   }
}
