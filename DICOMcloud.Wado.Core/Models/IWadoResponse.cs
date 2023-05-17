using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud.Media;

namespace DICOMcloud.Wado.Models
{
   public interface IWadoRsResponse
   {
      Stream Content { get; set ;}
      MimeMediaType MimeType { get; set ; }
      string TransferSyntax {  get; set; }
      long ContentLength { get; set;  }
    }
}
