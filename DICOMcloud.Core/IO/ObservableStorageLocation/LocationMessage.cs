using DICOMcloud.Messaging;

namespace DICOMcloud.IO
{
    public class LocationMessage : TransportMessage
    {
        public IStorageLocation Location
        {
            get; set;
        }

        public long ContentLength
        {
            get; set;
        }


        public LocationMessage ( IStorageLocation location ) 
        {
            Location = location ;
        }
    }
}
