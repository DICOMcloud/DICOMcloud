using System;
using DICOMcloud.Pacs.Commands;

namespace DICOMcloud.Pacs
{
    public class DicomOperationResult
    {
        public Exception     Error   { get; set; }
        public string        Message { get; set; }
        public CommandStatus Status  { get; set; }
    }
}