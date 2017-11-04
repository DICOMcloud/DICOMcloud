using System.Collections.Generic;
using DICOMcloud.DataAccess;
using fo = Dicom;

namespace DICOMcloud.Pacs
{
    public interface IObjectArchieveQueryService
    {
        IEnumerable<fo.DicomDataset> FindStudies 
        ( 
            fo.DicomDataset request, 
            IQueryOptions options

        ) ;

        IEnumerable<fo.DicomDataset> FindObjectInstances
        (
            fo.DicomDataset request,
            IQueryOptions options

        ) ;

        IEnumerable<fo.DicomDataset> FindSeries
        (
            fo.DicomDataset request,
            IQueryOptions options
        ) ;
    }
}