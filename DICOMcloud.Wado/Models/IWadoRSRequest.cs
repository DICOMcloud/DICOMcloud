using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud;

namespace DICOMcloud.Wado.Models
{
    public interface IWadoRsRequestBase : IWadoRequestHeader
    {
        ObjectQueryLevel QueryLevel { get; set; }
    }

    public interface IWadoRsStudiesRequest : IWadoRsRequestBase, IStudyId
    {
    }

    public interface IWadoRsSeriesRequest : IWadoRsStudiesRequest, ISeriesId
    {
        
    }

    public interface IWadoRsInstanceRequest : IWadoRsSeriesRequest, IObjectId
    {

    }

    public interface IWadoRsFramesRequest :IWadoRsInstanceRequest
    {
        int[] Frames { get; set; }
    }
}
