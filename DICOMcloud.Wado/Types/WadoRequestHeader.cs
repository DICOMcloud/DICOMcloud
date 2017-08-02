using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado
{
   public class WadoRequestHeader : IWadoRequestHeader
   {
      public HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> AcceptHeader { get; set; }
      public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptCharsetHeader { get; set; }
   }
}
