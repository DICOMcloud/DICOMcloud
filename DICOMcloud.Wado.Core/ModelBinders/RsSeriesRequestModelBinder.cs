
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado
{
    public class RsSeriesRequestModelBinder : RsRequestModelBinder<IWadoRsSeriesRequest>, IModelBinder
    {
    }
}
