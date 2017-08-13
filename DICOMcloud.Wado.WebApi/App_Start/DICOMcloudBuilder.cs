using System.Linq;
using System.Net.Http ;
using Dicom;
using DICOMcloud.Azure.IO;
using DICOMcloud.DataAccess;
using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.DataAccess.Database.Sql;
using DICOMcloud.IO;
using DICOMcloud.Media;
using DICOMcloud.Messaging;
using DICOMcloud.Pacs;
using DICOMcloud.Pacs.Commands;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using StructureMap;
using System;
using fo = Dicom;

namespace DICOMcloud.Wado
{
    public partial class DICOMcloudBuilder : Registry
    {
        public DICOMcloudBuilder ()
        {
            Build ( ) ;
        }

        private static void ConfigureLogging ( )
        {
            fo.Log.LogManager.SetImplementation ( TraceLogManager.Instance );
        }

        protected virtual void Build ( )
        {
            Init ( ) ;

            ConfigureLogging ( ) ;

            RegisterEvents ( ) ;

            RegisterComponents ( ) ;

            RegisterMediaWriters ( ) ;

            EnsureCodecsLoaded ( ) ;
        }
        
        public bool AzureStorageSupported { get; protected set; }

        protected virtual void Init ( )
        {
            ConnectionString = CloudConfigurationManager.GetSetting   ( "app:PacsDataArchieve" ) ;
            StorageConection = CloudConfigurationManager.GetSetting   ( "app:PacsStorageConnection" ) ;
            DataAccess       = new SqlObjectArchieveDataAccess ( ConnectionString ) ;
        }

        protected virtual void RegisterEvents ( )
        {
            RegisterAnonymizer ( ) ;
        }

        protected virtual void RegisterAnonymizer ( )
        {
            string enableAnonymizer = CloudConfigurationManager.GetSetting   ( "app:enableAnonymizer" ) ;
            bool   enable             = true ;
            

            if ( !bool.TryParse ( enableAnonymizer, out enable ) || enable )
            {
                string anonymizerOptions = CloudConfigurationManager.GetSetting   ( "app:anonymizerOptions" ) ;
                var    options = DicomAnonymizer.SecurityProfileOptions.BasicProfile |
                                 DicomAnonymizer.SecurityProfileOptions.RetainUIDs |
                                 DicomAnonymizer.SecurityProfileOptions.RetainLongFullDates |
                                 DicomAnonymizer.SecurityProfileOptions.RetainPatientChars ;

                if ( !string.IsNullOrWhiteSpace ( anonymizerOptions ) )
                {
                    options = (DicomAnonymizer.SecurityProfileOptions) Enum.Parse ( typeof (DicomAnonymizer.SecurityProfileOptions), anonymizerOptions, true ) ;
                }

                var anonymizer = new DicomAnonymizer ( DicomAnonymizer.SecurityProfile.LoadProfile ( null, options ) ) ;

                anonymizer.Profile.PatientName = "Dcloud^Anonymized";
                anonymizer.Profile.PatientID   = "Dcloud.Anonymized";

                RemoveAnonymizerTag ( anonymizer, DicomTag.PatientName ) ;
                RemoveAnonymizerTag ( anonymizer, DicomTag.PatientID ) ;

                PublisherSubscriberFactory.Instance.Subscribe<WebStoreDatasetProcessingMessage>(this, (message) =>
                {
                    var queryParams = message.Request.Request.RequestUri.ParseQueryString ( ) ;

                    
                    anonymizer.AnonymizeInPlace(message.Dataset);

                    if (null != queryParams)
                    {
                        foreach ( var queryKey in queryParams.OfType<String>()  )
                        {
                            uint tag ;


                            if ( string.IsNullOrWhiteSpace(queryKey)) { continue; }
                            
                            if( uint.TryParse (queryKey, System.Globalization.NumberStyles.HexNumber, null, out tag) )
                            {
                                message.Dataset.AddOrUpdate(tag, queryParams[queryKey] ) ;
                            }
                        }
                    }
                });
            }
        }

        protected virtual void RegisterComponents ( )
        {
            IRetieveUrlProvider urlProvider = new RetieveUrlProvider ( CloudConfigurationManager.GetSetting ( RetieveUrlProvider.config_WadoRs_API_URL) ) ;
            
            
            For<DbSchemaProvider> ( ).Use<StorageDbSchemaProvider> ( ) ; //default constructor

            For<IDCloudCommandFactory> ( ).Use<DCloudCommandFactory> ( ) ;

            For<IObjectArchieveQueryService> ( ).Use<ObjectArchieveQueryService> ( ) ;
            For<IObjectStoreService>         ( ).Use<ObjectStoreService>         ( ) ;
            For<IObjectRetrieveService>      ( ).Use<ObjectRetrieveService>      ( ) ;

            For<IWadoRsService>         ( ).Use<WadoRsService>         ( ) ;
            For<IWebObjectStoreService> ( ).Use<WebObjectStoreService> ( ) ;
            For<IQidoRsService>         ( ).Use<QidoRsService>         ( ) ;
            For<IWadoUriService>        ( ).Use<WadoUriService>        ( ) ;

            For<IObjectStorageDataAccess> ( ).Use ( @DataAccess ) ;
            For<IObjectStorageQueryDataAccess> ( ).Use ( @DataAccess ) ;
            
            For<IDicomMediaIdFactory> ( ).Use <DicomMediaIdFactory> ( ) ;

            For<IRetieveUrlProvider> ( ).Use (urlProvider) ;

            if ( StorageConection.StartsWith("|datadirectory|", StringComparison.OrdinalIgnoreCase))
            {
                var appDataPath  = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) ;
                var lastIndex    = StorageConection.IndexOf ('|', 1 ) ;
                var userPathPart = StorageConection.Substring ( lastIndex + 1 ) ;
                
                
                StorageConection = appDataPath + userPathPart ;
            }

            if ( System.IO.Path.IsPathRooted ( StorageConection ) )
            {
                For<IMediaStorageService> ( ).Use<FileStorageService> ( ).Ctor<string> ( ).Is (StorageConection) ;
            }
            else
            {
                StorageAccount = CloudStorageAccount.Parse ( StorageConection ) ;

                AzureStorageSupported = true ;

                For<CloudStorageAccount> ( ).Use ( @StorageAccount ) ;
                
                For<IMediaStorageService> ( ).Use <AzureStorageService> ( ).Ctor<CloudStorageAccount> ( ).Is ( StorageAccount ) ;
            }
        }

        protected virtual void RegisterMediaWriters ( ) 
        {
            //var factory = new InjectionFactory(c => new Func<string, IDicomMediaWriter> (name => c.Resolve<IDicomMediaWriter>(name))) ;

            For<IDicomMediaWriter>().Use<NativeMediaWriter>().Named(MimeMediaTypes.DICOM);
            For<IDicomMediaWriter>().Use<JsonMediaWriter>().Named(MimeMediaTypes.Json);
            For<IDicomMediaWriter>().Use<XmlMediaWriter>().Named(MimeMediaTypes.xmlDicom);
            For<IDicomMediaWriter>().Use<JpegMediaWriter>().Named(MimeMediaTypes.Jpeg);
            For<IDicomMediaWriter>().Use<UncompressedMediaWriter>().Named(MimeMediaTypes.UncompressedData);
            
            For<Func<string, IDicomMediaWriter>> ( ).Use ( ( m=> new Func<String,IDicomMediaWriter> ( name => m.TryGetInstance<IDicomMediaWriter> (name)) )) ;
            
            For<IDicomMediaWriterFactory>( ).Use <DicomMediaWriterFactory>();

            For<IJsonDicomConverter> ( ).Use <JsonDicomConverter>();
        }

        protected virtual void EnsureCodecsLoaded ( ) 
        {
            var path = System.IO.Path.Combine ( System.Web.Hosting.HostingEnvironment.MapPath ( "~/" ), "bin" );

            System.Diagnostics.Trace.TraceInformation ( "Path: " + path );

            fo.Imaging.Codec.TranscoderManager.LoadCodecs ( path ) ;
        }

        protected string                    ConnectionString { get; set; }
        protected string                    StorageConection { get; set; }
        protected CloudStorageAccount       StorageAccount   { get; private set ; }
        protected IObjectArchieveDataAccess DataAccess       { get; set; }

        private static void RemoveAnonymizerTag ( DicomAnonymizer anonymizer, DicomTag tag )
        {
            var parenthesis = new[] { '(', ')' };
            var tagString = tag.ToString ( ).Trim(parenthesis);
            var action = anonymizer.Profile.FirstOrDefault(pair => pair.Key.IsMatch(tagString));
            anonymizer.Profile.Remove(action.Key);
        }
    }
}