using System;

namespace DICOMcloud
{
    public interface IObjectId : ISeriesId
    {
        string SOPInstanceUID {  get; set; }

        int? Frame { get; set; }
    }
}
