using fo = Dicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellowOakDicom;

namespace DICOMcloud.DataAccess.Matching
{
    public interface IMatchingCondition : IQueryInfo, IDicomDataParameter
    {
        bool CanMatch ( DicomItem element ) ;
    }
}
