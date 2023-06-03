using System.Linq;
using System.Net.Http ;
using Dicom;
//using DICOMcloud.Azure.IO;
using DICOMcloud.DataAccess;
using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.IO;
using DICOMcloud.Media;
using DICOMcloud.Messaging;
using DICOMcloud.Pacs;
using DICOMcloud.Pacs.Commands;
using Microsoft.Azure;
//using Microsoft.WindowsAzure.Storage;
//using StructureMap;
using System;
using fo = Dicom;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using DICOMcloud.Wado.WebApi.Exceptions;
using DICOMcloud.DataAccess.Database;
using DICOMcloud.Wado.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using DICOMcloud.Wado.WebApi.Core.App_Start;

namespace DICOMcloud.Wado
{
    public static class DICOMcloudBuilder 
    {
        //private WebApplicationBuilder _Builder { get; set;}
        //private readonly IConfiguration _configuration ;
        //private readonly IHostingEnvironment _hostingEnvironment;

        //public DICOMcloudBuilder(WebApplicationBuilder builder, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        //{
        //    _Builder = builder;

        //    Build();
        //    _configuration = configuration;
        //    _hostingEnvironment = hostingEnvironment;
        //}


        public static void ConfigureLogging (IServiceCollection services)
        {
            var config = new HttpConfiguration();
            fo.Log.LogManager.SetImplementation ( TraceLogManager.Instance );

            config.Services.Add ( typeof(IExceptionLogger), new DICOMcloudExceptionLogger()) ;
            config.Services.Replace(typeof(IExceptionHandler), new DICOMcloudExceptionHandler());

            services.AddSingleton(config);
        }

        public static void  Build (this IServiceCollection services)
        {
            //Init ( ) ;
            RegisterEvents (services) ;

            RegisterComponents (services) ;

            RegisterMediaWriters (services) ;

            ConfigureLogging(services) ;

            WebApiConfig.Register(services,Config) ;

            //EnsureCodecsLoaded ( ) ;
        }
        
        static bool AzureStorageSupported { get;  set; }

        public static  void Init (this WebApplicationBuilder _Builder)
        {
            ConnectionStringProvider =  new ConnectionStringProvider (_Builder.Configuration ) ;
            Config =  new Config (_Builder.Configuration ) ;
            StorageConection         = Config.StorageConection ;
            var supportPreSignedUrl = Config.SupportPreSignedUrl;

            //StorageConection         = CloudConfigurationManager.GetSetting ( "app:PacsStorageConnection" ) ;
            
            //var supportPreSignedUrl = CloudConfigurationManager.GetSetting("app:supportPreSignedUrls");

            // For backward compatability - Feb-1-2020
            if (string.IsNullOrEmpty(supportPreSignedUrl))
            {
                supportPreSignedUrl = Config.SupportSelfSignedUrl;
                //supportPreSignedUrl = CloudConfigurationManager.GetSetting("app:supportSelfSignedUrls");
            }

            if (!string.IsNullOrWhiteSpace(supportPreSignedUrl))
            { 
                DicomWebServerSettings.Instance.SupportPreSignedUrls =  bool.Parse (supportPreSignedUrl.Trim());
            }
        }

        static void RegisterEvents (IServiceCollection services)
        {
            RegisterAnonymizer (services) ;
        }

        static void RegisterAnonymizer (IServiceCollection services)
        {
            string enableAnonymizer = Config.EnableAnonymizer ;
            //string enableAnonymizer = CloudConfigurationManager.GetSetting   ( "app:enableAnonymizer" ) ;
            bool   enable             = true ;
            

            if ( !bool.TryParse ( enableAnonymizer, out enable ) || enable )
            {
                string anonymizerOptions = Config.AnonymizerOptions;
                //string anonymizerOptions = CloudConfigurationManager.GetSetting   ( "app:anonymizerOptions" ) ;
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

                PublisherSubscriberFactory.Instance.Subscribe<WebStoreDatasetProcessingMessage>(services, (message) =>
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

        static void RegisterComponents (IServiceCollection services)
        {
            services.AddScoped<IConnectionStringProvider, ConnectionStringProvider>();
            services.AddScoped<DbSchemaProvider, StorageDbSchemaProvider>();
            services.AddScoped<IDatabaseFactory, SqlDatabaseFactory>();
            services.AddScoped<ISortingStrategyFactory, SortingStrategyFactory>();
            services.AddScoped<ObjectArchieveDataAdapter>();
            services.AddScoped<IObjectArchieveDataAccess, ObjectArchieveDataAccess>();

            //For<IConnectionStringProvider> ( ).Use<ConnectionStringProvider> ( ) ;
            //For<DbSchemaProvider> ( ).Use<StorageDbSchemaProvider> ( ) ; //default constructor
            //For<IDatabaseFactory> ( ).Use<SqlDatabaseFactory> ( ) ;
            //For<ISortingStrategyFactory> ( ).Use <SortingStrategyFactory> ( ) ;
            //For<ObjectArchieveDataAdapter> ( ).Use <ObjectArchieveDataAdapter> ( ) ;
            //For<IObjectArchieveDataAccess> ( ).Use <ObjectArchieveDataAccess> ( ) ;

            IRetrieveUrlProvider urlProvider = new RetrieveUrlProvider(Config.Config_WadoRs_API_URL, Config.Config_WadoUri_API_URL);

            //IRetrieveUrlProvider urlProvider = new RetrieveUrlProvider ( CloudConfigurationManager.GetSetting ( RetrieveUrlProvider.config_WadoRs_API_URL),
            //                                                             CloudConfigurationManager.GetSetting ( RetrieveUrlProvider.config_WadoUri_API_URL) ) ;

            services.AddScoped<IDCloudCommandFactory, DCloudCommandFactory>();
            services.AddScoped<IObjectArchieveQueryService, ObjectArchieveQueryService>();
            services.AddScoped<IObjectStoreService, ObjectStoreService>();
            services.AddScoped<IObjectRetrieveService, ObjectRetrieveService>();
            services.AddScoped<IWadoRsService, WadoRsService>();
            services.AddScoped<IWebObjectStoreService, WebObjectStoreService>();
            services.AddScoped<IQidoRsService, QidoRsService>();
            services.AddScoped<IWadoUriService, WadoUriService>();
            services.AddScoped<IOhifService, OhifService>();
            services.AddScoped<IDicomMediaIdFactory, DicomMediaIdFactory>();
            services.AddScoped<IRetrieveUrlProvider, RetrieveUrlProvider>();

            //For<IDCloudCommandFactory> ( ).Use<DCloudCommandFactory> ( ) ;

            //For<IObjectArchieveQueryService> ( ).Use<ObjectArchieveQueryService> ( ) ;
            //For<IObjectStoreService>         ( ).Use<ObjectStoreService>         ( ) ;
            //For<IObjectRetrieveService>      ( ).Use<ObjectRetrieveService>      ( ) ;

            //For<IWadoRsService>         ( ).Use<WadoRsService>         ( ) ;
            //For<IWebObjectStoreService> ( ).Use<WebObjectStoreService> ( ) ;
            //For<IQidoRsService>         ( ).Use<QidoRsService>         ( ) ;
            //For<IWadoUriService>        ( ).Use<WadoUriService>        ( ) ;
            //For<IOhifService>           ( ).Use<OhifService>           ( ) ;

            //For<IDicomMediaIdFactory> ( ).Use <DicomMediaIdFactory> ( ) ;

            //For<IRetrieveUrlProvider> ( ).Use (urlProvider) ;

            RegisterStoreCommandSettings(services);

            if ( StorageConection.StartsWith("|datadirectory|", StringComparison.OrdinalIgnoreCase))
            {
                var appDataPath  = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) ;
                var lastIndex    = StorageConection.IndexOf ('|', 1 ) ;
                var userPathPart = StorageConection.Substring ( lastIndex + 1 ) ;
                
                
                StorageConection = appDataPath + userPathPart ;
            }
            
            if ( System.IO.Path.IsPathRooted ( StorageConection ) )
            {
                services.AddScoped<IKeyProvider, HashedFileKeyProvider>();
                services.AddScoped<IMediaStorageService, FileStorageService>();

                //For<IKeyProvider> ( ).Use<HashedFileKeyProvider> ( ) ;
                //For<IMediaStorageService> ( ).Use<FileStorageService> ( ).Ctor<string> ( ).Is (StorageConection) ;
            }
            else
            {
                StorageAccount = CloudStorageAccount.Parse ( StorageConection ) ;

                AzureStorageSupported = true ;

                services.AddScoped<CloudStorageAccount>();
                services.AddScoped<IMediaStorageService>();

                //For<CloudStorageAccount> ( ).Use ( @StorageAccount ) ;
                
                //For<IMediaStorageService> ( ).Use <AzureStorageService> ( ).Ctor<CloudStorageAccount> ( ).Is ( StorageAccount ) ;
            }
        }

        static void RegisterStoreCommandSettings(IServiceCollection services)
        {
            StorageSettings storageSettings = new StorageSettings ( ) ;

            var validateDuplicateInstance = Config.ValidateDuplicateInstance;
            var storeOriginalDataset = Config.StoreOriginalDataset;
            var storeQueryModel = Config.StoreQueryModel;            

            //var validateDuplicateInstance = CloudConfigurationManager.GetSetting("app:storecommand.validateDuplicateInstance");
            //var storeOriginalDataset = CloudConfigurationManager.GetSetting("app:storecommand.storeOriginalDataset");
            //var storeQueryModel = CloudConfigurationManager.GetSetting("app:storecommand.storeQueryModel");

            if (bool.TryParse (validateDuplicateInstance, out bool validateDuplicateValue))
            { 
                storageSettings.ValidateDuplicateInstance = validateDuplicateValue;
            }

            if (bool.TryParse(storeOriginalDataset, out bool storeOriginalDatasetValue))
            {
                storageSettings.StoreOriginal = storeOriginalDatasetValue;
            }

            if (bool.TryParse(storeQueryModel, out bool storeQueryModelValue))
            {
                storageSettings.StoreQueryModel = validateDuplicateValue;
            }

            services.AddScoped<StorageSettings>();
            //For<StorageSettings>().Use (@storageSettings);
        }

        static void RegisterMediaWriters (IServiceCollection services) 
        {
            //var factory = new InjectionFactory(c => new Func<string, IDicomMediaWriter> (name => c.Resolve<IDicomMediaWriter>(name))) ;


            // You can also use a factory method to create the instances

            services.AddScoped<IDicomMediaWriter, NativeMediaWriter>(provider =>
            {
                var name = MimeMediaTypes.DICOM; 
                return provider.GetServices<NativeMediaWriter>().FirstOrDefault(service => service.GetType().Name == name);
            });

            services.AddScoped<IDicomMediaWriter, JsonMediaWriter>(provider =>
            {
                var name = MimeMediaTypes.Json; 
                return provider.GetServices<JsonMediaWriter>().FirstOrDefault(service => service.GetType().Name == name);
            });
            
            services.AddScoped<IDicomMediaWriter, XmlMediaWriter>(provider =>
            {
                var name = MimeMediaTypes.xmlDicom; 
                return provider.GetServices<XmlMediaWriter>().FirstOrDefault(service => service.GetType().Name == name);
            });

            services.AddScoped<IDicomMediaWriter, JpegMediaWriter>(provider =>
            {
                var name = MimeMediaTypes.Jpeg; 
                return provider.GetServices<JpegMediaWriter>().FirstOrDefault(service => service.GetType().Name == name);
            });

            services.AddScoped<IDicomMediaWriter, UncompressedMediaWriter>(provider =>
            {
                var name = MimeMediaTypes.UncompressedData; 
                return provider.GetServices<UncompressedMediaWriter>().FirstOrDefault(service => service.GetType().Name == name);
            });

            //For<IDicomMediaWriter>().Use<NativeMediaWriter>().Named(MimeMediaTypes.DICOM);
            //For<IDicomMediaWriter>().Use<JsonMediaWriter>().Named(MimeMediaTypes.Json);
            //For<IDicomMediaWriter>().Use<XmlMediaWriter>().Named(MimeMediaTypes.xmlDicom);
            //For<IDicomMediaWriter>().Use<JpegMediaWriter>().Named(MimeMediaTypes.Jpeg);
            //For<IDicomMediaWriter>().Use<UncompressedMediaWriter>().Named(MimeMediaTypes.UncompressedData);

            //For<Func<string, IDicomMediaWriter>> ( ).Use ( ( m=> new Func<String,IDicomMediaWriter> ( name => m.TryGetInstance<IDicomMediaWriter> (name)) )) ;

            services.AddScoped<IDicomMediaWriterFactory, DicomMediaWriterFactory>();
            services.AddScoped<IJsonDicomConverter, JsonDicomConverter>();

            //For<IDicomMediaWriterFactory>( ).Use <DicomMediaWriterFactory>();

            //For<IJsonDicomConverter> ( ).Use <JsonDicomConverter>();
        }

        public static void EnsureCodecsLoaded (this IWebHostEnvironment _hostingEnvironment) 
        {
            var path = System.IO.Path.Combine (_hostingEnvironment.ContentRootPath, "bin" );
            //var path = System.IO.Path.Combine ( System.Web.Hosting.HostingEnvironment.MapPath ( "~/" ), "bin" );

            System.Diagnostics.Trace.TraceInformation ( "Path: " + path );

            fo.Imaging.Codec.TranscoderManager.LoadCodecs ( path ) ;
        }

        static string  StorageConection { get; set; }
        /// <summary>
        static ConnectionStringProvider  ConnectionStringProvider { get; set; }
        static Config  Config { get; set; }
        /// </summary>
        static CloudStorageAccount       StorageAccount   { get;  set ; }

        //public static IConfiguration Configuration => Configuration1;

        //public static IConfiguration Configuration1 => _configuration;

        private static void RemoveAnonymizerTag ( DicomAnonymizer anonymizer, DicomTag tag )
        {
            var parenthesis = new[] { '(', ')' };
            var tagString = tag.ToString ( ).Trim(parenthesis);
            var action = anonymizer.Profile.FirstOrDefault(pair => pair.Key.IsMatch(tagString));
            anonymizer.Profile.Remove(action.Key);
        }
    }
}