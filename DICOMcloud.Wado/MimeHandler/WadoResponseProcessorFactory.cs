using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud.IO;
using DICOMcloud.Media;

namespace DICOMcloud.Wado
{
    public class WadoResponseProcessorFactory
    {
        public IMediaStorageService MediaStorage { get; private set; }
        public IDicomMediaIdFactory MediaFactory { get; private set; }

        public WadoResponseProcessorFactory ( IMediaStorageService stroageService, IDicomMediaIdFactory mediaFactory )
        {
            MediaStorage = stroageService ;
        }
        
        public IMimeResponseHandler GetHandler(List<MediaTypeHeaderValue> mimeType)
        {
            return new ImageObjectHandler ( MediaStorage, MediaFactory ) ;
        }
    }
}
