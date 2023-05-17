using DICOMcloud.IO;
using System.Collections.Generic;

namespace DICOMcloud.Media
{
    public interface IDicomMediaWriter : IMediaWriter<DicomMediaWriterParameters>
    {
        IList<IStorageLocation> CreateMedia ( DicomMediaWriterParameters data, ILocationProvider storage, int[] frameList ) ;
    }
}