using System.Net.Http;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public interface IQidoRsService
    {
        QidoResponse SearchForInstances ( IQidoRequestModel request );
        QidoResponse SearchForSeries ( IQidoRequestModel request );
        QidoResponse SearchForStudies ( IQidoRequestModel request );
    }
}