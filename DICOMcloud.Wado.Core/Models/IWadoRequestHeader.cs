using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;


namespace DICOMcloud.Wado.Models
{
   public interface IWadoRequestHeader
   {
      IEnumerable<MediaTypeHeaderValue>         AcceptHeader        { get; set; }
      IEnumerable<StringWithQualityHeaderValue> AcceptCharsetHeader { get; set; }
      RequestHeaders                            Headers             { get; set; }

    }
}
