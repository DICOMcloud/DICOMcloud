using Dicom;
using DICOMcloud.DataAccess;
using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado
{
    public class QidoResponse
    {
        public IQidoRequestModel Request { get; set; }
        public DicomDataset QueryDataset {  get; set; }
        public PagedResult<DicomDataset> Result { get; set; }

        public QidoResponse
        (
            IQidoRequestModel request, 
            DicomDataset queryDataset, 
            PagedResult<DicomDataset> result 
        ) 
        {
            Request = request;
            QueryDataset = queryDataset;
            Result = result;
        }
    }
}
