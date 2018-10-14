using System.Collections.Generic;

namespace DICOMcloud.Wado.Models
{
    public interface IQidoQuery
    {
        List<string> IncludeElements
        {
            get;
        }
        Dictionary<string, string> MatchingElements
        {
            get;
        }

        Dictionary<string,string> CustomParameters { get; }
    }
}