using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace DICOMcloud.Wado
{
   public interface IWadoUriService
   {
      HttpResponseMessage GetInstance ( IWadoUriRequest request ) ;
   }
}
