using System ;
using System.Collections.Generic;

namespace DICOMcloud.Messaging
{
    public interface ITransportMessage
    {
        string Name { get; }
        string ID   { get; set; }
        Dictionary<string,string> Properties { get; }
    }
}