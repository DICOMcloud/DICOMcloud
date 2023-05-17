namespace DICOMcloud.Media
{
    public interface IDicomMediaWriterFactory
    {
        IDicomMediaWriter GetMediaWriter( string mediaType );
    }
}