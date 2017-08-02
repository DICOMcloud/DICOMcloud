using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Messaging
{
    public interface IPublisherSubscriber
    {
        void Publish<T>     ( object sender, T message ) where T : ITransportMessage ;
        void Subscribe<T>   ( object sender, Action<T> handler) where T : ITransportMessage ;
        void Unsubscribe<T> ( object sender, Action<T> handler) where T : ITransportMessage;
    }
}
