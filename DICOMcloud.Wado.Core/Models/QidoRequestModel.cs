using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado.Models
{
    public class QidoRequestModel : IQidoRequestModel
    {
        public IEnumerable<StringWithQualityHeaderValue> AcceptCharsetHeader
        {
            get ;
            set ;
        }

        public IEnumerable<MediaTypeHeaderValue> AcceptHeader
        {
            get ;
            set ;
        }
        
        public RequestHeaders Headers { get; set; }

        public bool? FuzzyMatching
        {
            get; set;
        }

        public int? Limit
        {
            get; set;
        }

        public int? Offset
        {
            get; set;
        }

        public QidoQuery Query
        {
            get; set;
        }
    }
}
