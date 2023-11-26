
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Http.ModelBinding;

namespace DICOMcloud.Wado
{
   public class UriRequestModelBinder : IModelBinder
   {
      public UriRequestModelBinder() { }

       public Task BindModelAsync(ModelBindingContext bindingContext)
       {
         if (bindingContext.ModelType != typeof(IWadoUriRequest))
         {
            return Task.CompletedTask;
         }
         
         IWadoUriRequest result ;
            
         if ( new UriRequestModelConverter ( ).TryParse ( bindingContext.HttpContext.Request, out result) )
         { 
            bindingContext.Model = result;
               
            return Task.CompletedTask;
         }
         else
         { 
            bindingContext.ModelState.AddModelError( bindingContext.ModelName, "Cannot convert value to Location");
            return Task.CompletedTask;
         }
       }
    }
}
