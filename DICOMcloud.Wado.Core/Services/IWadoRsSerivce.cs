using System.Net.Http;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public interface IWadoRsService
    {
        WadoRsResponse RetrieveStudy(IWadoRsStudiesRequest request);
        WadoRsResponse RetrieveSeries(IWadoRsSeriesRequest request);
        WadoRsResponse RetrieveInstance(IWadoRsInstanceRequest request);
        WadoRsResponse RetrieveFrames(IWadoRsFramesRequest request);

        WadoRsResponse RetrieveBulkData(IWadoRsInstanceRequest request);
        WadoRsResponse RetrieveBulkData(IWadoRsFramesRequest request);
        
        WadoRsResponse RetrieveStudyMetadata(IWadoRsStudiesRequest request);
        WadoRsResponse RetrieveSeriesMetadata(IWadoRsSeriesRequest request);
        WadoRsResponse RetrieveInstanceMetadata(IWadoRsInstanceRequest request);
    }
}