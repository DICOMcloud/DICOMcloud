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
    public class WebStoreDatasetMessage : TransportMessage
    {
        public WebStoreDatasetMessage ( WebStoreRequest request, DicomDataset dataset )
        {
            Request = request ;
            Dataset = dataset ;
        }

        public DicomDataset    Dataset { get; set; }
        public WebStoreRequest Request { get; set; }
    }
}
