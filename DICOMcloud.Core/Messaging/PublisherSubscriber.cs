using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubSub ;


namespace DICOMcloud.Messaging
{
    public class PublisherSubscriber : IPublisherSubscriber
    {
        public void Publish<T>(T message) where T : ITransportMessage
        {
            OnPublishing ( message ) ;
            
            _eventBroker.Publish <T> ( message ) ;
        }

        public void Subscribe<T>(object sender, Action<T> handler) where T : ITransportMessage
        {
            OnSubscribing ( this, handler ) ;

            _eventBroker.Subscribe <T> ( sender, handler ) ;
        }

        public void Unsubscribe<T>(object sender, Action<T> handler) where T : ITransportMessage
        {
            OnUnsubscribing ( sender, handler ) ;

            _eventBroker.Unsubscribe <T> ( sender, handler ) ;
        }


        protected virtual void OnPublishing<T> ( T message ) where T : ITransportMessage
        {}

        protected virtual void OnSubscribing<T> ( object sender, Action<T> handler ) where T : ITransportMessage
        {}

        protected virtual void OnUnsubscribing<T> ( object sender, Action<T> handler ) where T : ITransportMessage
        {}

        private Hub _eventBroker = new Hub ( ) ;
    }
}
