using System;
using fo = Dicom;
using DICOMcloud.Pacs.Commands;

namespace DICOMcloud.Pacs
{
    public class StoreResult : DicomOperationResult
    {
        public fo.DicomDataset DataSet { get; set; }    
    }
}