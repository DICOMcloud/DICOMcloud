using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using DICOMcloud.Pacs;

namespace DICOMcloud.Wado.Models
{
    public abstract class WadoRsRequestBase : IWadoRequestHeader
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

       public ObjectQueryLevel QueryLevel { get; set; } 
    }

    public class WadoRSStudiesRequest : WadoRsRequestBase, IWadoRsStudiesRequest
    {
        public string StudyInstanceUID{get; set;}
    }

    public class  WadoRSSeriesRequest : WadoRSStudiesRequest, IWadoRsSeriesRequest
    {
        public string SeriesInstanceUID{get; set;}
    }

    public class WadoRSInstanceRequest : WadoRSSeriesRequest, IWadoRSInstanceRequest
    {
        public WadoRSInstanceRequest() { }
        public WadoRSInstanceRequest(IWadoRsStudiesRequest request)
        {
            AcceptCharsetHeader = request.AcceptCharsetHeader ;
            AcceptHeader = request.AcceptHeader ;
            StudyInstanceUID = request.StudyInstanceUID ;
        }

        public WadoRSInstanceRequest(IWadoRsSeriesRequest request)
        : this ( (IWadoRsStudiesRequest)request)
        {
            SeriesInstanceUID = request.SeriesInstanceUID ;
        }

        public WadoRSInstanceRequest(WadoRSInstanceRequest request)
        : this ( (IWadoRsSeriesRequest)request)
        {
            SOPInstanceUID = request.SOPInstanceUID;
        }

        public string SOPInstanceUID{get; set;}
        public int? Frame { get; set; }
    }

    public class WadoRSFramesRequest : WadoRSInstanceRequest, IWadoRSFramesRequest
    {
        public int[] Frames{get; set;}
    }
}
