using DICOMcloud;
using DICOMcloud.DataAccess;
using DICOMcloud.IO;
using DICOMcloud.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DICOMcloud.Pacs.Commands
{
    public class DeleteCommand : IDCloudCommand<DeleteCommandData, DCloudCommandResult>, IDeleteCommand
    {
        public IMediaStorageService            StorageService { get; set; }
        public IObjectArchieveDataAccess DataAccess     { get; set; }
        public IDicomMediaIdFactory            MediaFactory   { get; set; }
        
        public DeleteCommand
        ( 
            IMediaStorageService storageService,    
            IObjectArchieveDataAccess dataAccess,
            IDicomMediaIdFactory mediaFactory
        )
        {
            StorageService = storageService ;
            DataAccess     = dataAccess ;
            MediaFactory   = mediaFactory ;
        }
        
        public DCloudCommandResult Execute ( DeleteCommandData commandData )
        {
            switch ( commandData.DeleteLevel )
            {
                case ObjectQueryLevel.Study:
                {
                    return DeleteStudy ( commandData.Instances ) ;
                }

                case ObjectQueryLevel.Series:
                {
                    return DeleteSeries ( commandData.Instances ) ;
                }

                case ObjectQueryLevel.Instance:
                {
                    return DeleteInstance ( commandData.Instances ) ;
                }

                default:
                {
                    throw new ApplicationException ( "Invalid delete level" ) ;//TODO:
                }
            }
        }

        protected  virtual DCloudCommandResult DeleteStudy ( IEnumerable<IStudyId> studies )
        {
            foreach (var study in studies )
            {
                DeleteMediaLocations   ( study ) ;
                DataAccess.DeleteStudy ( study );
            }
                  
            return new DCloudCommandResult ( ) ;//TODO: currently nothing to return    
        }

        protected  virtual DCloudCommandResult DeleteSeries ( IEnumerable<ISeriesId> seriesIds )
        {
            foreach ( var series in seriesIds )
            {
                DeleteMediaLocations    ( series ) ;
                DataAccess.DeleteSeries ( series );
            }
                        
            return new DCloudCommandResult ( ) ;//TODO: currently nothing to return    
        }

        protected  virtual DCloudCommandResult DeleteInstance ( IEnumerable<IObjectId> instances )
        {
            foreach ( var instance in instances )
            {
                DeleteMediaLocations      ( instance );
                DataAccess.DeleteInstance ( instance ); //delete from DB after all dependencies are completed
            }

            return new DCloudCommandResult ( ) ;//TODO: currently nothing to return    
        }

        private void DeleteMediaLocations ( IStudyId study )
        {
            var studyMeta = DataAccess.GetStudyMetadata ( study );


            if ( null != studyMeta )
            {
                foreach ( var objectMetaRaw in studyMeta )
                {
                    DeleteMediaLocations ( objectMetaRaw );
                }
            }
        }

        private void DeleteMediaLocations ( ISeriesId series )
        {
            var seriesMeta = DataAccess.GetSeriesMetadata ( series );


            if ( null != seriesMeta )
            {
                foreach ( var objectMetaRaw in seriesMeta )
                {
                    DeleteMediaLocations ( objectMetaRaw );
                }
            }
        }

        private void DeleteMediaLocations ( IObjectId instance )
        {
            var objectMetaRaw = DataAccess.GetInstanceMetadata ( instance );

            DeleteMediaLocations ( objectMetaRaw );
        }

        private void DeleteMediaLocations ( InstanceMetadata objectMetaRaw )
        {
            if ( null != objectMetaRaw )
            {
                var mediaLocations = objectMetaRaw.MediaLocations;


                foreach ( var dicomMediaLocation in mediaLocations )
                {
                    foreach ( var locationParts in dicomMediaLocation.Locations )
                    {
                        IStorageLocation location;
                        IMediaId mediaId;


                        mediaId = MediaFactory.Create ( locationParts.Parts );
                        location = StorageService.GetLocation ( mediaId );
                        
                        if ( location.Exists ( ) )
                        {
                            location.Delete ( );
                        }

                    }
                }
            }
        }

        private static IEnumerable<DicomMediaLocations> GetDistinctMedia ( InstanceMetadata objectMetaRaw )
        {
            //http://stackoverflow.com/questions/489258/linqs-distinct-on-a-particular-property
            return objectMetaRaw.MediaLocations
                                                .GroupBy ( n => new
                                                {
                                                    n.TransferSyntax,
                                                    n.MediaType
                                                } )
                                                .Select ( m => m.First ( ) );
        }
    }
}
