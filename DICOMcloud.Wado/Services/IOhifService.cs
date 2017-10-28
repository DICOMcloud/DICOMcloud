using System.Net.Http;

namespace DICOMcloud.Wado
{
    public interface IOhifService
    {
        HttpResponseMessage GetStudies(string studyInstanceUid);
    }
}