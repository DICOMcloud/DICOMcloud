using System;
using System.Collections.Generic;
using fo = Dicom;
using Dicom.Imaging ;
using DICOMcloud.IO;
using DICOMcloud.Media ;
using Dicom;

namespace DICOMcloud.Media
{
    public abstract class DicomMediaWriterBase : IDicomMediaWriter
    {
        public virtual ILocationProvider    MediaStorage { get; protected set ; }
        public virtual IDicomMediaIdFactory MediaFactory { get; protected set ; }

        public DicomMediaWriterBase() : this(new FileStorageService(), new DicomMediaIdFactory ( ) )
        { }

        public DicomMediaWriterBase ( ILocationProvider storageProvider, IDicomMediaIdFactory mediaFactory )
        {
            MediaStorage = storageProvider ;
            MediaFactory = mediaFactory ;
        }

        public abstract string MediaType
        {
            get ;
        }

        public virtual bool CanUpload (DicomDataset ds, int frame)
        {
            return true;
        }

        public IList<IStorageLocation> CreateMedia
        (
            DicomMediaWriterParameters mediaParameters
        )
        {
            return CreateMedia ( mediaParameters, MediaStorage ) ;
        }

        public IList<IStorageLocation> CreateMedia
        (
            DicomMediaWriterParameters mediaParameters, 
            ILocationProvider sotrageProvider 
        )
        {
            if (null != sotrageProvider )
            {
                int                    framesCount    = 1;
                List<IStorageLocation> locations      = new List<IStorageLocation> ( ) ;
                var                    dataset        = GetMediaDataset ( mediaParameters.Dataset, mediaParameters.MediaInfo ) ;
                string                 transferSyntax = ( !string.IsNullOrWhiteSpace (mediaParameters.MediaInfo.TransferSyntax ) ) ? ( mediaParameters.MediaInfo.TransferSyntax ) : "" ;
                var                    pixelDataItem  = dataset.GetDicomItem<DicomItem>(fo.DicomTag.PixelData);

                if (StoreMultiFrames && pixelDataItem != null)
                {
                    DicomPixelData pd ;


                    pd          = DicomPixelData.Create ( mediaParameters.Dataset ) ;
                    framesCount = pd.NumberOfFrames ;
                }
                
                for ( int frame = 1; frame <= framesCount; frame++ )
                {
                    if (CanUpload(dataset, frame))
                    {
                        var storeLocation = sotrageProvider.GetLocation(MediaFactory.Create(mediaParameters.Dataset, frame, MediaType, transferSyntax));


                        Upload(dataset, frame, storeLocation, mediaParameters.MediaInfo);

                        locations.Add(storeLocation);
                    }
                }

                return locations ;
            }

            throw new InvalidOperationException ( "No MediaStorage service found") ;
        }


        public IList<IStorageLocation> CreateMedia
        (
            DicomMediaWriterParameters mediaParameters, 
            ILocationProvider storageProvider,
            int[] frameList
        )
        {
            if ( null == storageProvider ) { throw new InvalidOperationException ( "No MediaStorage service found") ; }

            List<IStorageLocation> locations      = new List<IStorageLocation> ( ) ;
            var                    dataset        = GetMediaDataset ( mediaParameters.Dataset, mediaParameters.MediaInfo ) ;
            string                 transferSyntax = ( !string.IsNullOrWhiteSpace (mediaParameters.MediaInfo.TransferSyntax ) ) ? ( mediaParameters.MediaInfo.TransferSyntax ) : "" ;


            if ( !StoreMultiFrames )
            {
                throw new InvalidOperationException ( "Media writer doesn't support generating frames" ) ;
            }

            foreach ( int frame in frameList )
            {
                if (CanUpload(dataset, frame))
                {
                    var storeLocation = storageProvider.GetLocation(MediaFactory.Create(mediaParameters.Dataset, frame, MediaType, transferSyntax));


                    Upload(mediaParameters.Dataset, frame, storeLocation, mediaParameters.MediaInfo);

                    locations.Add(storeLocation);
                }
            }

            return locations ;
        
        }

        protected virtual fo.DicomDataset GetMediaDataset ( fo.DicomDataset data, DicomMediaProperties mediaInfo )
        {
            return data ;
        }

        protected abstract bool StoreMultiFrames { get; }

        protected abstract void Upload(fo.DicomDataset dataset, int frame, IStorageLocation storeLocation, DicomMediaProperties mediaProperties );
    }
}
