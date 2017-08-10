using Dicom;
using DICOMcloud.Messaging;
using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado
{
    public class WebStoreDatasetProcessingFailureMessage : WebStoreDatasetMessage
    {
        public WebStoreDatasetProcessingFailureMessage ( WebStoreRequest request, DicomDataset dataset, Exception error )
        : base ( request, dataset )
        {
            Error = error ;
        }

        public Exception Error { get; set; }
    }
}
