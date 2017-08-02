namespace DICOMcloud.Media
{
    public interface IDicomMediaReaderFactory
    {
        IDicomMediaReader GetMediaReader ( string mimeType ) ;
    }
}