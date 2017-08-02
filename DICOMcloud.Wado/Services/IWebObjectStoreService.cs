using System.Net.Http;
using System.Threading.Tasks;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public interface IWebObjectStoreService
    {
        Task<HttpResponseMessage> Store  ( IWebStoreRequest request, string studyInstanceUID );
        Task<HttpResponseMessage> Delete ( IWebDeleteRequest request ) ;
    }
}