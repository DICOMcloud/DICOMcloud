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
    public class WebStoreDatasetProcessingMessage : WebStoreDatasetMessage
    {
        public WebStoreDatasetProcessingMessage ( WebStoreRequest request, DicomDataset dataset )
        : base ( request, dataset )
        {
        }
    }
}
