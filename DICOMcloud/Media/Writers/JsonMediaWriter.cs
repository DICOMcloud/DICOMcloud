using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom;
using DICOMcloud.IO;

namespace DICOMcloud.Media
{
    public class JsonMediaWriter : DicomMediaWriter
    {
        public JsonMediaWriter ( IMediaStorageService mediaStorage, IDicomMediaIdFactory mediaFactory ) 
        : base ( mediaStorage, new JsonDicomConverter ( ), MimeMediaTypes.Json, mediaFactory )
        {}
    }
}
