
using DICOMcloud.IO;
using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;


namespace DICOMcloud
{
    
    public interface IUncompressedPixelDataConverter : IDicomConverter<Stream>
    {}

    public class UncompressedPixelDataConverter : IUncompressedPixelDataConverter
    {
        public UncompressedPixelDataConverter()
        {
        }

        //TODO: is this used? update with fo-dicom
        public Stream Convert ( DicomDataset ds )
        {
            //DicomDataset command = new DicomDataset () ;

            //command[DicomTag.TransferSyntaxUid] = ds[DicomTag.TransferSyntaxUid] ;
            //DicomMessage message = new DicomMessage (command, ds) ;

            
            //DicomPixelData pd = DicomPixelData.CreateFrom ( message ) ;
            //string tempFile = System.IO.Path.GetTempFileName ( ) ; 
            //System.IO.File.WriteAllBytes ( tempFile, pd.GetFrame (0));
        
            //return new DICOMcloud.Storage.TempStream (new TempFile(tempFile));
            return null ;
        }

        public DicomDataset Convert ( Stream uncompressedData )
        {
            throw new NotImplementedException ( "Converting image data to DICOM is not supported." ) ;
        }

    }
}
