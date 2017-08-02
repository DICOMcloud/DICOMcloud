using System.Net.Http;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public interface IWadoRsService
    {
        HttpResponseMessage RetrieveStudy(IWadoRsStudiesRequest request);
        HttpResponseMessage RetrieveSeries(IWadoRsSeriesRequest request);
        HttpResponseMessage RetrieveInstance(IWadoRSInstanceRequest request);
        HttpResponseMessage RetrieveFrames(IWadoRSFramesRequest request);

        HttpResponseMessage RetrieveBulkData(IWadoRSInstanceRequest request);
        HttpResponseMessage RetrieveBulkData(IWadoRSFramesRequest request);
        
        HttpResponseMessage RetrieveStudyMetadata(IWadoRsStudiesRequest request);
        HttpResponseMessage RetrieveSeriesMetadata(IWadoRsSeriesRequest request);
        HttpResponseMessage RetrieveInstanceMetadata(IWadoRSInstanceRequest request);
    }
}