using System.Net.Http;
using System.Threading.Tasks;
using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public interface IWebObjectStoreService
    {
        Task<HttpResponseMessage> Store  ( WebStoreRequest request, IStudyId studyId = null);
        Task<HttpResponseMessage> Delete ( WebDeleteRequest request ) ;
    }
}