using System.Net.Http;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public interface IWadoRsService
    {
        HttpResponseMessage RetrieveStudy(IWadoRsStudiesRequest request);
        HttpResponseMessage RetrieveSeries(IWadoRsSeriesRequest request);
        HttpResponseMessage RetrieveInstance(IWadoRsInstanceRequest request);
        HttpResponseMessage RetrieveFrames(IWadoRsFramesRequest request);

        HttpResponseMessage RetrieveBulkData(IWadoRsInstanceRequest request);
        HttpResponseMessage RetrieveBulkData(IWadoRsFramesRequest request);
        
        HttpResponseMessage RetrieveStudyMetadata(IWadoRsStudiesRequest request);
        HttpResponseMessage RetrieveSeriesMetadata(IWadoRsSeriesRequest request);
        HttpResponseMessage RetrieveInstanceMetadata(IWadoRsInstanceRequest request);
    }
}