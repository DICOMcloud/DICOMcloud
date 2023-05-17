using System ;
using System.Collections.Generic;

namespace DICOMcloud.Messaging
{
    public class TransportMessage : ITransportMessage
    {
        public TransportMessage ( ) : this ( Guid.NewGuid ( ).ToString ( ), null )  
        {
        
        }

        public TransportMessage ( string id, string name ) 
        {
            ID         = id ;
            Name       = name ?? this.GetType ( ).Name ;
            Properties = new Dictionary<string, string> ( ) ;
        }
        
        public string Name { get; }
        public string ID   { get; set; }
        public Dictionary<string,string> Properties { get; private set; }

    }
}