using Dicom.Imaging.Codec;
using DICOMcloud.IO;
using DICOMcloud;
using DICOMcloud.DataAccess;
using DICOMcloud.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using fo = Dicom;
using DICOMcloud.Messaging;

namespace DICOMcloud.Pacs.Commands
{
    public class StoreCommand : DCloudCommand<StoreCommandData,StoreCommandResult>, IStoreCommand
    {
        public StoreCommand ( ) : this ( null, null ) 
        {}

        public StoreCommand 
        ( 
            IObjectStorageDataAccess dataStorage, 
            IDicomMediaWriterFactory mediaFactory
        )
        : base ( dataStorage )
        {
            Settings = new StorageSettings ( ) ;
            MediaFactory = mediaFactory ;
        }

        public override StoreCommandResult Execute ( StoreCommandData request )
        {

            //TODO: Check against supported types/association, validation, can store, return appropriate error
            
            request.Metadata.MediaLocations = SaveDicomMedia ( request.Dataset ) ;

            StoreQueryModel ( request ) ;
            
            PublisherSubscriberFactory.Instance.Publish ( this, new DicomStoreSuccessMessage ( request.Metadata ) ) ;            
            
            return null ;
        }

        protected virtual DicomMediaLocations[] SaveDicomMedia 
        ( 
            fo.DicomDataset dicomObject
        )
        {
            List<DicomMediaLocations> mediaLocations = new List<DicomMediaLocations> ( ) ;
            fo.DicomDataset storageDataset = dicomObject.Clone ( fo.DicomTransferSyntax.ExplicitVRLittleEndian ) ;


            foreach ( var mediaType in Settings.MediaTypes )
            {
                CreateMedia ( mediaLocations, storageDataset, mediaType ) ;
            }

            var mediaInfo = Settings.MediaTypes.Where ( n=>n.MediaType == MimeMediaTypes.DICOM && 
                                                        n.TransferSyntax == dicomObject.InternalTransferSyntax.UID.UID ).FirstOrDefault ( ) ;

            if ( mediaInfo == null )
            {
                CreateMedia ( mediaLocations, dicomObject, new DicomMediaProperties ( MimeMediaTypes.DICOM, dicomObject.InternalTransferSyntax.UID.UID ) ) ;
            }

            return mediaLocations.ToArray ( ) ;
        }

        private void CreateMedia ( List<DicomMediaLocations> mediaLocations, fo.DicomDataset storageDataset, DicomMediaProperties mediaInfo )
        {
            DicomMediaLocations mediaLocation;
            IDicomMediaWriter   writer;
            string transferSytax  = (!string.IsNullOrWhiteSpace (mediaInfo.TransferSyntax ) ) ? mediaInfo.TransferSyntax : "" ;

            mediaLocation = new DicomMediaLocations ( ) { MediaType = mediaInfo.MediaType, TransferSyntax = transferSytax };
            writer = MediaFactory.GetMediaWriter ( mediaInfo.MediaType );

            if ( null != writer )
            {
                try
                {
                    IList<IStorageLocation> createdMedia = writer.CreateMedia ( new DicomMediaWriterParameters ( ) { Dataset = storageDataset, 
                                                                                                                     MediaInfo = mediaInfo } );


                    mediaLocation.Locations = createdMedia.Select ( media => new MediaLocationParts { Parts = media.MediaId.GetIdParts ( ) } ).ToList ( );

                    mediaLocations.Add ( mediaLocation );
                }
                catch ( Exception ex )
                {
                    Trace.TraceError ( "Error creating media: " + ex.ToString ( ) );

                    throw;
                }
            }
            else
            {
                //TODO: log something
                Trace.TraceWarning ( "Media writer not found for mediaType: " + mediaInfo );
            }
        }

        protected virtual void StoreQueryModel
        (
            StoreCommandData data
        )
        {
            IDicomDataParameterFactory<StoreParameter> condFactory ;
            IEnumerable<StoreParameter>                conditions ;

            condFactory = new DicomStoreParameterFactory ( ) ;
            conditions = condFactory.ProcessDataSet ( data.Dataset ) ;

            DataAccess.StoreInstance ( new ObjectId ( data.Dataset ), conditions, data.Metadata ) ;
        }
        

        public StorageSettings Settings { get; set;  }
        public IDicomMediaWriterFactory MediaFactory { get; set; }
    }

    public class StorageSettings
    {
        public StorageSettings ( ) 
        {
            MediaTypes = new List<DicomMediaProperties> ( ) ;
        
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.DICOM, fo.DicomTransferSyntax.ExplicitVRLittleEndian.UID.UID ) ) ;
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.Json ) ) ;
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.UncompressedData, fo.DicomTransferSyntax.ExplicitVRLittleEndian.UID.UID ) ) ;
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.xmlDicom ) ) ;
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.DICOM, fo.DicomTransferSyntax.JPEG2000Lossless.UID.UID ) ) ;
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.DICOM, fo.DicomTransferSyntax.JPEG2000Lossy.UID.UID ) ) ;
            //MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.Jpeg, fo.DicomTransferSyntax.JPEGProcess14SV1.UID.UID ) ) ;
            //MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.Jpeg, fo.DicomTransferSyntax.JPEGProcess1.UID.UID ) ) ;
        }

        public IList<DicomMediaProperties> MediaTypes ;
    }
}
