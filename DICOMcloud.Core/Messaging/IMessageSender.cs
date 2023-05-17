using System ;


namespace DICOMcloud.Messaging
{
    public interface IMessageSender
    {
        void SendMessage ( ITransportMessage message, TimeSpan? delay = default (TimeSpan?) ) ;
    }
}