using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Matching
{
    public interface IQueryInfo
    {
        bool IsCaseSensitive { get; }
        bool ExactMatch      { get; }
        bool SupportFuzzy    { get; }    
    }
}
