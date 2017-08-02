using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using fo = Dicom;
using System.IO;
using DICOMcloud.Media;
using DICOMcloud.IO;

namespace DICOMcloud.Wado
{
    public class TextObjectHandler : ObjectHandlerBase
    {
        public TextObjectHandler ( IMediaStorageService mediaStorage, IDicomMediaIdFactory mediaFactory ) : base ( mediaStorage, mediaFactory )
        {}

        public override bool CanProcess(string mimeType)
        {
            return string.Compare (mimeType , MimeMediaTypes.PlainText, true ) == 0 ;
        }

        protected override WadoResponse DoProcess(IWadoUriRequest request, string mimeType)
        {
            fo.DicomFile df = fo.DicomFile.Open (Location.GetReadStream ( )) ;//TODO: check how the toolkit loads the image in memory or not. we do not need to load it
            WadoResponse response = new WadoResponse ( ) ;


            //df.Load ( Location.GetReadStream ( ), null, DicomReadOptions.DoNotStorePixelDataInDataSet);

            response.Content = GenerateStreamFromString ( df.ToString ( ) );
            response.MimeType = mimeType ;

            return response ;
        }

        public Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
   }
}
