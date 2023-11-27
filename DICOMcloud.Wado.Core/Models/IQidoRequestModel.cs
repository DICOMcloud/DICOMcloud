namespace DICOMcloud.Wado.Models
{
    public interface IQidoRequestModel : IWadoRequestHeader
    {
        bool? FuzzyMatching
        {
            get; set;
        }

        int? Limit
        {
            get; set;
        }

        int? Offset
        {
            get; set;
        }
        
        QidoQuery Query
        {
            get; set;
        }
    }
}
