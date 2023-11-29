using System.Collections.Generic;

using FellowOakDicom;

namespace DICOMcloud.DataAccess
{
    public interface IDicomDataParameterFactory <T>
        where T : IDicomDataParameter 
    {
        void BeginProcessingElements ( ) ;

        void ProcessElement(DicomItem element) ;

        IEnumerable<T> EndProcessingElements ( ) ;

        IEnumerable<T> ProcessDataSet ( DicomDataset dataset );
    }
}
