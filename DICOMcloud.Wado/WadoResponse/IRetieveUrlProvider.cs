namespace DICOMcloud.Wado
{
    public interface IRetieveUrlProvider
    {
        string BaseUrl { get; set; }

        string GetInstanceUrl(ObjectId instance);
        string GetInstanceUrl(string studyInstanceUID, string seriesInstanceUID, string sopInstanceUID);
        string GetStudyUrl(string studyInstanceUID);
    }
}