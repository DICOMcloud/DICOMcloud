namespace DICOMcloud.Pacs.Commands
{
    public interface IDCloudCommandFactory
    {
        IDeleteCommand CreateDeleteCommand ( );
        IStoreCommand CreateStoreCommand ( );
    }
}