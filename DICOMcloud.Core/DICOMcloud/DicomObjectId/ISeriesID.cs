using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud
{
    public interface ISeriesId : IStudyId
    { 
        string SeriesInstanceUID { get; set ; }
    }
}
