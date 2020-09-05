
using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado
{
   public interface IMimeResponseHandler
   {
      bool CanProcess(string mimeType);

      Task<IWadoRsResponse> Process(IWadoUriRequest request, string mimeType);
   }
}
