using System.Collections.Generic;
using fo = Dicom;

namespace DICOMcloud.DataAccess
{
    public interface IDicomDataParameterFactory <T>
        where T : IDicomDataParameter 
    {
        void BeginProcessingElements ( ) ;

        void ProcessElement(fo.DicomItem element) ;

        IEnumerable<T> EndProcessingElements ( ) ;

        IEnumerable<T> ProcessDataSet ( fo.DicomDataset dataset );
    }
}
