using DICOMcloud.IO;
using DICOMcloud;
using DICOMcloud.Media;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dicom ;
using Dicom.Imaging.Codec ;

namespace DICOMcloud.Pacs
{
    public class ObjectRetrieveService : IObjectRetrieveService
    {
        public virtual IMediaStorageService     StorageService     { get; protected set; }
        public virtual IDicomMediaWriterFactory MediaWriterFactory { get; protected set ; }
        public virtual IDicomMediaIdFactory     MediaFactory       { get; protected set ; }
        public virtual string AnyTransferSyntaxValue               { get; set; }

        public ObjectRetrieveService 
        ( 
            IMediaStorageService mediaStorage, 
            IDicomMediaWriterFactory mediaWriterFactory, 
            IDicomMediaIdFactory mediaFactory 
        )
        {
            AnyTransferSyntaxValue = "*" ;
            
            StorageService     = mediaStorage ;
            MediaWriterFactory = mediaWriterFactory ;
            MediaFactory       = mediaFactory ;
        }
        
        public virtual IStorageLocation RetrieveSopInstance ( IObjectId query, DicomMediaProperties mediaInfo ) 
        {
            return StorageService.GetLocation ( MediaFactory.Create (query, mediaInfo ) ) ;
        }
        
        public virtual IEnumerable<IStorageLocation> RetrieveSopInstances ( IObjectId query, DicomMediaProperties mediaInfo ) 
        {
            return StorageService.EnumerateLocation ( MediaFactory.Create ( query, mediaInfo )) ;
        }

        public virtual IEnumerable<ObjectRetrieveResult> FindSopInstances
        ( 
            IObjectId query, 
            string mediaType, 
            IEnumerable<string> transferSyntaxes, 
            string defaultTransfer
        ) 
        {
            foreach ( var transfer in transferSyntaxes )
            {
                string instanceTransfer = (transfer == AnyTransferSyntaxValue) ? defaultTransfer : transfer ;

                var    mediaProperties = new DicomMediaProperties ( mediaType, instanceTransfer ) ;
                var    mediaID         = MediaFactory.Create      ( query, mediaProperties ) ;
                var    found           = false ;
                
                foreach ( IStorageLocation location in StorageService.EnumerateLocation ( mediaID ) )
                {
                    found = true ;

                    yield return new ObjectRetrieveResult ( location, transfer ) ;
                }
                
                if (found)
                {
                    break ;
                }
            }
        }

        public virtual IEnumerable<ObjectRetrieveResult> GetTransformedSopInstances 
        ( 
            IObjectId query, 
            string fromMediaType, 
            string fromTransferSyntax, 
            string toMediaType, 
            string toTransferSyntax 
        )
        {

            IMediaId fromMediaID = GetFromMediaId(query, fromMediaType, fromTransferSyntax);
            var frameList = (null != query.Frame) ? new int[] { query.Frame.Value } : null;


            if (StorageService.Exists(fromMediaID))
            {
                foreach (IStorageLocation location in StorageService.EnumerateLocation(fromMediaID))
                {
                    DicomFile defaultFile = DicomFile.Open(location.GetReadStream());

                    foreach (var transformedLocation in TransformDataset(defaultFile.Dataset, toMediaType, toTransferSyntax, frameList))
                    {
                        yield return new ObjectRetrieveResult(transformedLocation, toTransferSyntax);
                    }
                }
            }
        }

        private IMediaId GetFromMediaId (IObjectId query, string fromMediaType, string fromTransferSyntax)
        {
            DicomDataset ds = new DicomDataset ().NotValidated();

            ds.Add (DicomTag.StudyInstanceUID, query.StudyInstanceUID);
            ds.Add (DicomTag.SeriesInstanceUID, query.SeriesInstanceUID);
            ds.Add (DicomTag.SOPInstanceUID, query.SOPInstanceUID);

            return MediaFactory.Create(ds, 1, fromMediaType, fromTransferSyntax);
        }

        public virtual bool ObjetInstanceExist ( IObjectId objectId, string mediaType, string transferSyntax )
        {
            var mediaProperties = new DicomMediaProperties ( mediaType, transferSyntax ) ;
            var mediaID         = MediaFactory.Create      ( objectId, mediaProperties ) ;
            
                
            return StorageService.Exists (  mediaID ) ;
        }

        protected virtual IEnumerable<IStorageLocation> TransformDataset 
        ( 
            DicomDataset dataset, 
            string mediaType, 
            string instanceTransfer, 
            int[] frameList = null 
        ) 
        {
            var mediaProperties  = new DicomMediaProperties ( mediaType, instanceTransfer ) ;
            var writerParams     = new DicomMediaWriterParameters ( ) { Dataset = dataset, MediaInfo = mediaProperties } ;
            var locationProvider = new MemoryStorageProvider ( ) ;
            

            if ( null == frameList )
            {
                return MediaWriterFactory.GetMediaWriter ( mediaType ).CreateMedia ( writerParams, locationProvider ) ;
            }
            else
            {
                return MediaWriterFactory.GetMediaWriter ( mediaType ).CreateMedia ( writerParams, locationProvider, frameList ) ;
            }
            
        }

        protected virtual DicomDataset RetrieveDicomDataset ( IObjectId objectId, DicomMediaProperties mediainfo )
        {
            IStorageLocation location    ;
            DicomFile defaultFile ;


            location    = RetrieveSopInstance ( objectId, mediainfo ) ;

            if ( location == null )
            {
                return null ;
            }

            defaultFile = DicomFile.Open ( location.GetReadStream ( ) ) ;

            return defaultFile.Dataset ;

        }
    }
}
