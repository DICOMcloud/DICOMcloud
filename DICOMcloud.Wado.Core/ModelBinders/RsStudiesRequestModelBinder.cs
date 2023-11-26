
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace DICOMcloud.Wado
{
    public class RsStudiesRequestModelBinder : RsRequestModelBinder<IWadoRsStudiesRequest>
    {
    }
}
