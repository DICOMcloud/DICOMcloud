using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom ;
using DICOMcloud.IO;


namespace DICOMcloud.Media
{
    public class DicomMediaWriter : DicomMediaWriterBase
    {
        private string _mediaType ;

        public IDicomConverter<string> Converter { get; set; }
        
        public DicomMediaWriter 
        (  
            IDicomConverter<string> converter, 
            string mediaType 
        ) : this ( new FileStorageService ( ), converter, mediaType, new DicomMediaIdFactory ( ) )
        {}

        public DicomMediaWriter 
        ( 
            IMediaStorageService mediaStorage, 
            IDicomConverter<string> converter, 
            string mediaType, 
            IDicomMediaIdFactory mediaFactory 
        ) : base ( mediaStorage, mediaFactory )
        {
            Converter  = converter ;
            _mediaType = mediaType ;
        }

        public override string MediaType
        {
            get 
            {
                return _mediaType ;
            }
        }

        protected override bool StoreMultiFrames
        {
            get
            {
                return false ;
            }
        }


        protected override void Upload 
        ( 
            fo.DicomDataset dicomDataset, 
            int frame, 
            IStorageLocation location, 
            DicomMediaProperties mediaProperties )
        {
            location.Upload ( System.Text.Encoding.UTF8.GetBytes (Converter.Convert(dicomDataset)), MediaType) ;
        }
    }
}
