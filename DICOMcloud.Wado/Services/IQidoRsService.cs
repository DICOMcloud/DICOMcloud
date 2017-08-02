using System.Net.Http;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public interface IQidoRsService
    {
        HttpResponseMessage SearchForInstances ( IQidoRequestModel request );
        HttpResponseMessage SearchForSeries ( IQidoRequestModel request );
        HttpResponseMessage SearchForStudies ( IQidoRequestModel request );
    }
}