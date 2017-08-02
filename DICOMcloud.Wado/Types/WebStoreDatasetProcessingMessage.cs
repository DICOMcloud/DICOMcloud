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
    public class WebStoreDatasetProcessingMessage : TransportMessage
    {
        public WebStoreDatasetProcessingMessage ( IWebStoreRequest request, DicomDataset dataset )
        {
            Request = request ;
            Dataset = dataset ;
        }

        public DicomDataset     Dataset { get; set; }
        public IWebStoreRequest Request { get; set; }
    }
}
