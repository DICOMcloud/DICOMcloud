namespace DICOMcloud.Wado
{
    public interface IRetieveUrlProvider
    {
        string BaseWadoRsUrl { get; set; }
        string BaseWadoUriUrl { get; set; }
        bool PreferWadoUri    { get; set; }

        string GetInstanceUrl(IObjectId instance);
        string GetInstanceUrl(string studyInstanceUID, string seriesInstanceUID, string sopInstanceUID);
        string GetStudyUrl(string studyInstanceUID);
    }
}