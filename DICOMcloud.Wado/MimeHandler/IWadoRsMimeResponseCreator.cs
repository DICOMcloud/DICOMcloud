using Dicom;
using DICOMcloud.IO;

using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using DICOMcloud.Media;


namespace DICOMcloud.Wado
{
    //public interface IWadoRsMimeResponseCreator
    //{ 
    //    IWadoResponse[] CreateRetrieveInstanceResponse ( IWadoRequestHeader header, IStorageLocation[] locations, out string mimeType ) ;

    //    IWadoResponse[] CreateRetrieveMetadataResponse ( IWadoRequestHeader header, IStorageLocation[] storagelocations, out MimeMediaType mimeType);
    //}
    
    //public class WadoRsMimeResponseCreator : IWadoRsMimeResponseCreator
    //{

    //    //private List<MimeResponseCreator> _handlers = new List<MimeResponseCreator> ( ) ;





    //    private static DicomFile GetDicom(IStorageLocation storage)
    //    {
    //        DicomFile file = new DicomFile(storage.GetName());

    //        file.Load(DicomReadOptions.Default); //TODO: change read options
    //        return file;
    //    }

    //    protected virtual IDicomConverter<T> CreateDicomConverter<T>(MimeMediaType requestedMimeType)
    //    {
    //        Type converterType = typeof(T) ;
            
            
    //        switch ( requestedMimeType.MimeType )
    //        { 
    //            case MimeMediaTypes.UncompressedData:
    //            { 
    //                return new UncompressedPixelDataConverter ( ) as IDicomConverter<T>;
    //            }

    //            case MimeMediaTypes.xmlDicom:
    //            { 
    //                if ( converterType == typeof(string) )
    //                { 
    //                    return new XmlDicomConverter ( ) as IDicomConverter<T>;
    //                }
    //                else if ( converterType == typeof(Stream) )
    //                { 
    //                    return new XmlStreamDicomConverter ( ) as IDicomConverter<T> ;
    //                }
    //            }
    //            break;

    //            case MimeMediaTypes.Json:
    //            {
    //                return new JsonDicomConverter ( ) as IDicomConverter<T> ;
    //            }
    //        }
            
    //        throw new ApplicationException ( "No converter supported for mimeType ") ;
    //    }
    //}


    public class MediaTypeMapper
    { 
//1.2.840.10008.1.2.4.50 image/dicom+jpeg; transfer-syntax=1.2.840.10008.1.2.4.50
//1.2.840.10008.1.2.4.51 image/dicom+jpeg; transfer-syntax=1.2.840.10008.1.2.4.51
//image/dicom+jpeg; transfer-syntax=1.2.840.10008.1.2.4.57
//1.2.840.10008.1.2.4.57
//image/dicom+jpeg
//1.2.840.10008.1.2.4.70
//image/dicom+jpeg; transfer-syntax=1.2.840.10008.1.2.4.70
//1.2.840.10008.1.2.4.70
//image/dicom+rle
//1.2.840.10008.1.2.5
//image/dicom+rle; transfer-syntax=1.2.840.10008.1.2.5
//1.2.840.10008.1.2.5
//image/dicom+jpeg-ls
//1.2.840.10008.1.2.4.80
//image/dicom+jpeg-ls; transfer-syntax=1.2.840.10008.1.2.4.80
//1.2.840.10008.1.2.4.80
//image/dicom+jpeg-ls; transfer-syntax=1.2.840.10008.1.2.4.81
//1.2.840.10008.1.2.4.81
//image/dicom+jp2
//1.2.840.10008.1.2.4.90
//image/dicom+jp2; transfer-syntax=1.2.840.10008.1.2.4.90
//1.2.840.10008.1.2.4.90
//image/dicom+jp2; transfer-syntax=1.2.840.10008.1.2.4.91
//1.2.840.10008.1.2.4.91
//image/dicom+jpx
//1.2.840.10008.1.2.4.92
//image/dicom+jpx; transfer-syntax=1.2.840.10008.1.2.4.92
//1.2.840.10008.1.2.4.92
//image/dicom+jpx; transfer-syntax=1.2.840.10008.1.2.4.93
//1.2.840.10008.1.2.4.93
//Multi-frame media types
//image/dicom+jpx
//1.2.840.10008.1.2.4.92
//image/dicom+jpx; transfer-syntax=1.2.840.10008.1.2.4.92
//1.2.840.10008.1.2.4.92
//image/dicom+jpx; transfer-syntax=1.2.840.10008.1.2.4.93
//1.2.840.10008.1.2.4.93
//video/mpeg; transfer-syntax=1.2.840.10008.1.2.4.100
//1.2.840.10008.1.2.4.100
//video/mpeg; transfer-syntax=1.2.840.10008.1.2.4.101
//1.2.840.10008.1.2.4.101
//video/mp4; transfer-syntax=1.2.840.10008.1.2.4.102
//1.2.840.10008.1.2.4.102
//video/mp4; transfer-syntax=1.2.840.10008.1.2.4.103
//1.2.840.10008.1.2.4.103    
    }
}
