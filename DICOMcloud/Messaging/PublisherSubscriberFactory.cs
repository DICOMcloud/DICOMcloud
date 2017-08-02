using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Messaging
{
    public class PublisherSubscriberFactory
    {
        public PublisherSubscriberFactory ( ) {}
        
        static PublisherSubscriberFactory ( )
        {
            Instance = new PublisherSubscriber ( ) ;
        }

        public static void RegisterInstance ( IPublisherSubscriber instance ) 
        {
            lock ( _lockObj )
            {
                Instance = instance ;
            }
        }

        public static IPublisherSubscriber Instance 
        { 
            get ;
            set ; 
        
        }

        private static object _lockObj = new object ( ) ;
    }
}
