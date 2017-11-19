using Dicom.Imaging.Codec;
using DICOMcloud.IO;
using DICOMcloud;
using DICOMcloud.DataAccess;
using DICOMcloud.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DICOMcloud.Messaging;
using Dicom;

namespace DICOMcloud.Pacs.Commands
{
    public class StoreCommand : DCloudCommand<StoreCommandData,DCloudCommandResult>, IStoreCommand
    {
        static StoreCommand ( ) 
        {
            RequiredDsElements = new DicomDataset ( ) ;

            RequiredDsElements.Add<object> ( DicomTag.PatientID, null) ;
            RequiredDsElements.Add<object> ( DicomTag.StudyInstanceUID, null) ;
            RequiredDsElements.Add<object> ( DicomTag.SeriesInstanceUID, null) ;
            RequiredDsElements.Add<object> ( DicomTag.Modality, null) ;
            RequiredDsElements.Add<object> ( DicomTag.SOPClassUID, null) ;
            RequiredDsElements.Add<object> ( DicomTag.SOPInstanceUID, null) ;
        }

        public StoreCommand ( ) : this ( null, null ) 
        {}

        public StoreCommand 
        ( 
            IObjectArchieveDataAccess dataStorage, 
            IDicomMediaWriterFactory mediaFactory
        )
        : base ( dataStorage )
        {
            Settings = new StorageSettings ( ) ;
            MediaFactory = mediaFactory ;
        }

        public override DCloudCommandResult Execute ( StoreCommandData request )
        {

            //TODO: Check against supported types/association, validation, can store, return appropriate error
            ValidateDataset ( request.Dataset ) ;

            ValidateDuplicateInstance ( request ) ;

            request.Metadata.MediaLocations = SaveDicomMedia ( request.Dataset ) ;

            StoreQueryModel ( request ) ;
            
            PublisherSubscriberFactory.Instance.Publish ( this, new DicomStoreSuccessMessage ( request.Metadata ) ) ;            
            
            return new DCloudCommandResult ( ) ;
        }

        protected virtual void ValidateDuplicateInstance ( StoreCommandData request )
        {
            if ( DataAccess.Exists ( DicomObjectIdFactory.Instance.CreateObjectId ( request.Dataset ) ) )
            {
                throw new DCloudDuplicateInstanceException ( request.Dataset ) ;
            }
        }

        public StorageSettings Settings { get; set;  }
        
        public IDicomMediaWriterFactory MediaFactory { get; set; }

        public static DicomDataset RequiredDsElements
        {
            get ;
            private set ;
        }

        protected virtual void ValidateDataset ( DicomDataset dataset )
        {
            foreach ( var element in dataset )
            {
                if ( !dataset.Contains ( element.Tag ) )
                {
                    throw new DCloudException ( "Required element is missing. Element: " + element.Tag.DictionaryEntry.ToString ( ) ) ;
                }
            }
        }

        protected virtual DicomMediaLocations[] SaveDicomMedia 
        ( 
            DicomDataset dicomObject
        )
        {
            List<DicomMediaLocations> mediaLocations = new List<DicomMediaLocations> ( ) ;
            DicomDataset storageDataset = dicomObject.Clone ( DicomTransferSyntax.ExplicitVRLittleEndian ) ;


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

        protected virtual void StoreQueryModel
        (
            StoreCommandData data
        )
        {
            IDicomDataParameterFactory<StoreParameter> condFactory ;
            IEnumerable<StoreParameter>                conditions ;

            condFactory = new DicomStoreParameterFactory ( ) ;
            conditions  = condFactory.ProcessDataSet ( data.Dataset ) ;

            DataAccess.StoreInstance ( DicomObjectIdFactory.Instance.CreateObjectId ( data.Dataset ), conditions, data.Metadata ) ;
        }

        protected virtual void CreateMedia 
        ( 
            List<DicomMediaLocations> mediaLocations, 
            DicomDataset storageDataset, 
            DicomMediaProperties mediaInfo 
        )
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

    }

    public class StorageSettings
    {
        public StorageSettings ( ) 
        {
            MediaTypes = new List<DicomMediaProperties> ( ) ;
        
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.DICOM, DicomTransferSyntax.ExplicitVRLittleEndian.UID.UID ) ) ;
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.Json ) ) ;
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.UncompressedData, DicomTransferSyntax.ExplicitVRLittleEndian.UID.UID ) ) ;
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.xmlDicom ) ) ;
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.DICOM, DicomTransferSyntax.JPEG2000Lossless.UID.UID ) ) ;
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.DICOM, DicomTransferSyntax.JPEG2000Lossy.UID.UID ) ) ;
            //MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.Jpeg, DicomTransferSyntax.JPEGProcess14SV1.UID.UID ) ) ;
            //MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.Jpeg, DicomTransferSyntax.JPEGProcess1.UID.UID ) ) ;
        }

        public IList<DicomMediaProperties> MediaTypes ;
    }
}
