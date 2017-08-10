using System.Net.Http;
using System.Threading.Tasks;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public interface IWebObjectStoreService
    {
        Task<HttpResponseMessage> Store  ( WebStoreRequest request, string studyInstanceUID );
        Task<HttpResponseMessage> Delete ( WebDeleteRequest request ) ;
    }
}