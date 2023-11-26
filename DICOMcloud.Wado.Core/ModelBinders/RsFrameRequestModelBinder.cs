using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace DICOMcloud.Wado
{
    public class RsFrameRequestModelBinder : RsRequestModelBinder<IWadoRsFramesRequest>, IModelBinder
    {
    }
}
