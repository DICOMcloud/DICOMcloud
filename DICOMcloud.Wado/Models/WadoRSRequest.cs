using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using DICOMcloud.Pacs;

namespace DICOMcloud.Wado.Models
{
    public abstract class WadoRsRequestBase : IWadoRsRequestBase
    {

       public HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> AcceptHeader
       {
          get ;
          set ;
       }

       public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptCharsetHeader
       {
          get ;
          set ;
       }

        public HttpRequestHeaders Headers { get; set; }

        public ObjectQueryLevel QueryLevel { get; set; } 
    }

    public class WadoRsStudiesRequest : WadoRsRequestBase, IWadoRsStudiesRequest
    {
        public string StudyInstanceUID{get; set;}
    }

    public class  WadoRsSeriesRequest : WadoRsStudiesRequest, IWadoRsSeriesRequest
    {
        public string SeriesInstanceUID{get; set;}
    }

    public class WadoRsInstanceRequest : WadoRsSeriesRequest, IWadoRsInstanceRequest
    {
        public WadoRsInstanceRequest() { }
        public WadoRsInstanceRequest(IWadoRsStudiesRequest request)
        {
            AcceptCharsetHeader = request.AcceptCharsetHeader ;
            AcceptHeader = request.AcceptHeader ;
            StudyInstanceUID = request.StudyInstanceUID ;
        }

        public WadoRsInstanceRequest(IWadoRsSeriesRequest request)
        : this ( (IWadoRsStudiesRequest)request)
        {
            SeriesInstanceUID = request.SeriesInstanceUID ;
        }

        public WadoRsInstanceRequest(WadoRsInstanceRequest request)
        : this ( (IWadoRsSeriesRequest)request)
        {
            SOPInstanceUID = request.SOPInstanceUID;
        }

        public string SOPInstanceUID{get; set;}
        public int? Frame { get; set; }
    }

    public class WadoRsFramesRequest : WadoRsInstanceRequest, IWadoRsFramesRequest
    {
        public int[] Frames{get; set;}
    }
}
