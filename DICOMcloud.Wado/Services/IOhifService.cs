using System.Net.Http;

namespace DICOMcloud.Wado
{
    public interface IOhifService
    {
        HttpResponseMessage GetStudies(IStudyId studyId);
    }
}