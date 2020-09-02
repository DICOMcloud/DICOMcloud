using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DICOMcloud.Wado
{
   public class OtherObjectsHandler : IMimeResponseHandler
   {
      public bool CanProcess(string mimeType)
      {
         return false;
      }

      public Task<IWadoRsResponse> Process(IWadoUriRequest request, string mimeType)
      {
         throw new NotImplementedException();
      }
   }
}
