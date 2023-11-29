using System.Collections.Generic;
using DICOMcloud.DataAccess;
using FellowOakDicom;
using fo = Dicom;

namespace DICOMcloud.Pacs
{
    public interface IObjectArchieveQueryService
    {
        IEnumerable<DicomDataset> FindStudies 
        ( 
            DicomDataset request, 
            IQueryOptions options

        ) ;

        IEnumerable<DicomDataset> FindObjectInstances
        (
            DicomDataset request,
            IQueryOptions options

        ) ;

        IEnumerable<DicomDataset> FindSeries
        (
            DicomDataset request,
            IQueryOptions options
        ) ;


        PagedResult<DicomDataset> FindStudiesPaged
        ( 
            DicomDataset request, 
            IQueryOptions options

        ) ;

        PagedResult<DicomDataset> FindObjectInstancesPaged
        (
            DicomDataset request,
            IQueryOptions options

        ) ;

        PagedResult<DicomDataset> FindSeriesPaged
        (
            DicomDataset request,
            IQueryOptions options
        ) ;
    }
}