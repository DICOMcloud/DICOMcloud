using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud;

namespace DICOMcloud.Wado.Models
{
    public interface IWadoRsStudiesRequest : IWadoRequestHeader, IStudyId
    {
    
    }

    public interface IWadoRsSeriesRequest : IWadoRsStudiesRequest, ISeriesId
    {
        
    }

    public interface IWadoRSInstanceRequest : IWadoRsSeriesRequest, IObjectId
    {

    }

    public interface IWadoRSFramesRequest :IWadoRSInstanceRequest
    {
        int[] Frames { get; set; }
    }
}
