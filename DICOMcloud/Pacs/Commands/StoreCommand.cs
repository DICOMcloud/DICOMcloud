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
            RequiredDsElements = new DicomDataset ( ).NotValidated();

            RequiredDsElements.Add<string> ( DicomTag.PatientID) ;
            RequiredDsElements.Add<string> ( DicomTag.StudyInstanceUID) ;
            RequiredDsElements.Add<string> ( DicomTag.SeriesInstanceUID) ;
            RequiredDsElements.Add<string> ( DicomTag.Modality) ;
            RequiredDsElements.Add<string> ( DicomTag.SOPClassUID) ;
            RequiredDsElements.Add<string> ( DicomTag.SOPInstanceUID) ;
        }

        public StoreCommand ( ) : this ( null, null ) 
        {}

        public StoreCommand 
        ( 
            IObjectArchieveDataAccess dataStorage, 
            IDicomMediaWriterFactory mediaFactory
        )
        : this ( dataStorage, mediaFactory, new StorageSettings())
        {}

        public StoreCommand 
        ( 
            IObjectArchieveDataAccess dataStorage, 
            IDicomMediaWriterFactory mediaFactory,
            StorageSettings settings
        )
        : base ( dataStorage )
        {
            Settings = settings;
            MediaFactory = mediaFactory ;
        }

        public override DCloudCommandResult Execute ( StoreCommandData request )
        {

            //TODO: Check against supported types/association, validation, can store, return appropriate error
            ValidateDataset ( request.Dataset ) ;

            if (Settings.ValidateDuplicateInstance)
            { 
                ValidateDuplicateInstance ( request ) ;
            }

            request.Metadata.MediaLocations = SaveDicomMedia ( request.Dataset ) ;


            if (Settings.StoreQueryModel)
            { 
                StoreQueryModel ( request ) ;
            }
            
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
            foreach ( var element in RequiredDsElements )
            {
                if ( !dataset.Contains ( element.Tag ) )
                {
                    throw new DCloudException ( "Required element is missing. Element: " + element.Tag.DictionaryEntry.Name.ToString ( ) ) ;
                }

                if ( dataset.GetSingleValueOrDefault<string> (element.Tag, null) == null )
                {
                    throw new DCloudException ( "Required element has no value. Element: " + element.Tag.DictionaryEntry.Name.ToString ( ) ) ;
                }
            }
        }

        protected virtual DicomMediaLocations[] SaveDicomMedia 
        ( 
            DicomDataset dicomObject
        )
        {
            List<DicomMediaLocations> mediaLocations = new List<DicomMediaLocations> ( ) ;
            DicomDataset storageDataset = dicomObject.Clone(DicomTransferSyntax.ExplicitVRLittleEndian).NotValidated();
            List<DicomMediaProperties> storedMedia = new List<DicomMediaProperties> ();
            DicomMediaProperties defaultMedia = new DicomMediaProperties(MimeMediaTypes.DICOM, DicomTransferSyntax.ExplicitVRLittleEndian.UID.UID);

            CreateMedia(mediaLocations,
                        storageDataset,
                        defaultMedia);

            storedMedia.Add(defaultMedia);

            if (Settings.StoreOriginal)
            {
                var originalMedia = new DicomMediaProperties(MimeMediaTypes.DICOM, dicomObject.InternalTransferSyntax.UID.UID);

                if (!storedMedia.Contains(originalMedia))
                {
                    CreateMedia(mediaLocations, dicomObject, originalMedia);

                    storedMedia.Add(originalMedia);
                }
            }

            foreach (var mediaType in Settings.MediaTypes)
            {
                try
                { 
                    if (storedMedia.Contains(mediaType))
                    {
                            continue;
                    }

                    CreateMedia(mediaLocations, storageDataset, mediaType);
                }
                catch (Exception)
                {
                    Trace.TraceError($"Failed to create optional media {mediaType.MediaType} Transfer {mediaType.TransferSyntax}");
                }
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
                Trace.TraceWarning ( "Media writer not found for mediaType: " + mediaInfo );
            }
        }

    }

    public class StorageSettings
    {
        public StorageSettings ( ) 
        {
            StoreOriginal             = true ;
            ValidateDuplicateInstance = true;
            StoreQueryModel           = true ;

            MediaTypes = new List<DicomMediaProperties> ( ) ;
        
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.DICOM, DicomTransferSyntax.ExplicitVRLittleEndian.UID.UID ) ) ;
            MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.Json ) ) ;
            //MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.UncompressedData, DicomTransferSyntax.ExplicitVRLittleEndian.UID.UID ) ) ;
            //MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.xmlDicom ) ) ;
            //MediaTypes.Add(new DicomMediaProperties(MimeMediaTypes.DICOM, DicomTransferSyntax.JPEG2000Lossless.UID.UID));
            //MediaTypes.Add(new DicomMediaProperties(MimeMediaTypes.DICOM, DicomTransferSyntax.JPEG2000Lossy.UID.UID));
            //MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.DICOM, DicomTransferSyntax.JPEGProcess14SV1.UID.UID ) ) ;
            //MediaTypes.Add ( new DicomMediaProperties ( MimeMediaTypes.DICOM, DicomTransferSyntax.JPEGProcess1.UID.UID ) ) ;
            //MediaTypes.Add(new DicomMediaProperties(MimeMediaTypes.Jpeg));
        }

        public IList<DicomMediaProperties> MediaTypes { get; private set ;}
   
        public bool StoreOriginal { get; set; }
        public bool ValidateDuplicateInstance { get; set; }
        public bool StoreQueryModel { get; set; }
    }
}
