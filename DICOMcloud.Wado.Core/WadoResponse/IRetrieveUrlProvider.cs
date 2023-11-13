namespace DICOMcloud.Wado.Core.WadoResponse
{
    public interface IRetrieveUrlProvider
    {
        string BaseWadoRsUrl { get; set; }
        string BaseWadoUriUrl { get; set; }
        bool PreferWadoUri    { get; set; }

        string GetInstanceUrl(IObjectId instance);
        string GetStudyUrl   (IStudyId study);
    }
}