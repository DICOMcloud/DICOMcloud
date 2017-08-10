
using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace DICOMcloud.Wado
{
    public class RsDeleteRequestModelBinder : RsRequestModelBinder<WebDeleteRequest> 
    {
        protected override RsRequestModelConverter<WebDeleteRequest> GetConverter ( )
        {
            return new DeleteRsRequestModelConverter ( ) ;
        }
    }
}
