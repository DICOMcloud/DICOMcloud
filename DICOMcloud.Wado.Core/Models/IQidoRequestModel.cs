using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado.Models
{
    public interface IQidoRequestModel : IWadoRequestHeader
    {
        bool? FuzzyMatching
        {
            get; set;
        }

        int? Limit
        {
            get; set;
        }

        int? Offset
        {
            get; set;
        }
        
        QidoQuery Query
        {
            get; set;
        }
    }
}
