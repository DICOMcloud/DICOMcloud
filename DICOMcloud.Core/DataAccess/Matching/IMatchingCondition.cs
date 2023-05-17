using fo = Dicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Matching
{
    public interface IMatchingCondition : IQueryInfo, IDicomDataParameter
    {
        bool CanMatch ( fo.DicomItem element ) ;
    }
}
