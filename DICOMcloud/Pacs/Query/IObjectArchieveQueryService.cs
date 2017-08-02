using System.Collections.Generic;
using DICOMcloud.DataAccess;
using fo = Dicom;

namespace DICOMcloud.Pacs
{
    public interface IObjectArchieveQueryService
    {
        ICollection<fo.DicomDataset> FindStudies 
        ( 
            fo.DicomDataset request, 
            IQueryOptions options

        ) ;

        ICollection<fo.DicomDataset> FindObjectInstances
        (
            fo.DicomDataset request,
            IQueryOptions options

        ) ;

        ICollection<fo.DicomDataset> FindSeries
        (
            fo.DicomDataset request,
            IQueryOptions options
        ) ;
    }
}