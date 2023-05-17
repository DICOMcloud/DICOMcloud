using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado.Models
{
    public class QidoRequestModel : IQidoRequestModel
    {
        public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptCharsetHeader
        {
            get ;
            set ;
        }

        public HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> AcceptHeader
        {
            get ;
            set ;
        }
        
        public HttpRequestHeaders Headers { get; set; }

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
