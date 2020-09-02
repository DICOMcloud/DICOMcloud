using System.Net.Http;
using System.Threading.Tasks;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public interface IWadoRsService
    {
        Task<HttpResponseMessage> RetrieveStudy(IWadoRsStudiesRequest request);
        Task<HttpResponseMessage> RetrieveSeries(IWadoRsSeriesRequest request);
        Task<HttpResponseMessage> RetrieveInstance(IWadoRsInstanceRequest request);
        Task<HttpResponseMessage> RetrieveFrames(IWadoRsFramesRequest request);

        Task<HttpResponseMessage> RetrieveBulkData(IWadoRsInstanceRequest request);
        Task<HttpResponseMessage> RetrieveBulkData(IWadoRsFramesRequest request);
        
        Task<HttpResponseMessage> RetrieveStudyMetadata(IWadoRsStudiesRequest request);
        Task<HttpResponseMessage> RetrieveSeriesMetadata(IWadoRsSeriesRequest request);
        Task<HttpResponseMessage> RetrieveInstanceMetadata(IWadoRsInstanceRequest request);
    }
}