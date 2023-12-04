
//using DICOMcloud.Azure.IO;
using DICOMcloud.Azure.IO;
using DICOMcloud.DataAccess;
using DICOMcloud.DataAccess.Database;
using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.IO;
using DICOMcloud.Media;
using DICOMcloud.Messaging;
using DICOMcloud.Pacs;
using DICOMcloud.Pacs.Commands;
using DICOMcloud.Wado.Core.Types;
using DICOMcloud.Wado.Core.WadoResponse;
using DICOMcloud.Wado.Models;
using DICOMcloud.Wado.WebApi.Core.App_Start;
using DICOMcloud.Wado.WebApi.Exceptions;
using FellowOakDicom;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace DICOMcloud.Wado
{
    public static class DICOMcloudBuilder 
    {
        static bool AzureStorageSupported { get; set; }
        static string StorageConection { get; set; }
        static QidoRsServiceConfig QidoRsServiceConfig { get; set; }
        static ConnectionStringProvider ConnectionStringProvider { get; set; }
        static Config Config { get; set; }
        static WebApplicationBuilder App;

        private static void ConfigureLogging (IServiceCollection services)
        {
            var config = new HttpConfiguration();
            //TODO:new logging should uses standard aspnet core. 
            //FellowOakDicom.Log.LogManager.SetImplementation ( TraceLogManager.Instance );

            config.Services.Add ( typeof(IExceptionLogger), new DICOMcloudExceptionLogger()) ;

            services.AddSingleton(config);
        }

        public static void  BuildDICOMcloud (this IServiceCollection services, WebApplicationBuilder app)
        {
            Init(app);

            RegisterAnonymizer(services) ;

            RegisterComponents (services) ;

            RegisterMediaWriters (services) ;

            ConfigureLogging(services) ;

            WebApiConfig.Register(services,Config) ;

        }
        
        public static  void Init (WebApplicationBuilder _Builder)
        {
            App = _Builder;
            ConnectionStringProvider =  new ConnectionStringProvider (_Builder.Configuration ) ;
            Config =  new Config (_Builder.Configuration ) ;
            StorageConection         = Config.StorageConection ;
            QidoRsServiceConfig = new QidoRsServiceConfig(_Builder.Configuration);
            var supportPreSignedUrl = Config.SupportPreSignedUrl;

            // For backward compatability - Feb-1-2020
            if (string.IsNullOrEmpty(supportPreSignedUrl))
            {
                supportPreSignedUrl = Config.SupportSelfSignedUrl;
            }

            if (!string.IsNullOrWhiteSpace(supportPreSignedUrl))
            { 
                DicomWebServerSettings.Instance.SupportPreSignedUrls =  bool.Parse (supportPreSignedUrl.Trim());
            }
        }
       
        static void RegisterAnonymizer (IServiceCollection services)
        {
            string enableAnonymizer = Config.EnableAnonymizer ;
            bool   enable             = true ;
            

            if ( !bool.TryParse ( enableAnonymizer, out enable ) || enable )
            {
                string anonymizerOptions = Config.AnonymizerOptions;
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
                    var queryParams = message.Request.Request.Query;

                    
                    anonymizer.AnonymizeInPlace(message.Dataset);

                    if (null != queryParams)
                    {
                        foreach ( var queryKeyValue in queryParams  )
                        {
                            var queryKey = queryKeyValue.Key;
                            uint tag ;


                            if ( string.IsNullOrWhiteSpace(queryKey)) { continue; }
                            
                            if( uint.TryParse (queryKey, System.Globalization.NumberStyles.HexNumber, null, out tag) )
                            {
                                message.Dataset.AddOrUpdate(tag, queryKeyValue.Value) ;
                            }
                        }
                    }
                });
            }
        }

        static void RegisterComponents (IServiceCollection services)
        {
            services.AddSingleton(QidoRsServiceConfig);

            services.AddScoped<IConnectionStringProvider, ConnectionStringProvider>();
            services.AddScoped<DbSchemaProvider, StorageDbSchemaProvider>();
            services.AddScoped<IDatabaseFactory, SqlDatabaseFactory>();
            services.AddScoped<ISortingStrategyFactory, SortingStrategyFactory>();
            services.AddScoped<ObjectArchieveDataAdapter>();
            services.AddScoped<IObjectArchieveDataAccess, ObjectArchieveDataAccess>();
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
            
            services.AddSingleton<IRetrieveUrlProvider>(new RetrieveUrlProvider(App.Configuration));

            RegisterStoreCommandSettings(services);

            if ( StorageConection.StartsWith("|datadirectory|", StringComparison.OrdinalIgnoreCase))
            {
                var appDataPath  = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) ;
                var lastIndex    = StorageConection.IndexOf ('|', 1 ) ;
                var userPathPart = StorageConection.Substring ( lastIndex + 1 ) ;
                
                
                StorageConection = appDataPath + userPathPart ;
            }
            
            if ( Path.IsPathRooted ( StorageConection ) )
            {
                services.AddScoped<IKeyProvider, HashedFileKeyProvider>();
                services.AddScoped<IMediaStorageService, FileStorageService> ( (serviceProvider) => { 
                    return new FileStorageService(StorageConection);
                });
            }
            else
            {
                services.AddScoped<IMediaStorageService, AzureStorageService>((serviceProvider) => { 
                    return new AzureStorageService(StorageConection);
                });
            }
        }

        static void RegisterStoreCommandSettings(IServiceCollection services)
        {
            StorageSettings storageSettings = new StorageSettings();

            storageSettings.ValidateDuplicateInstance = Config.ValidateDuplicateInstance;
            storageSettings.StoreOriginal             = Config.StoreOriginalDataset;
            storageSettings.StoreQueryModel           = Config.StoreQueryModel;
            storageSettings.MediaTypes                = Config.MediaTypes;

            services.AddSingleton<StorageSettings>(storageSettings);
        }

        static void RegisterMediaWriters (IServiceCollection services) 
        {
            services.AddScoped<NativeMediaWriter>();
            services.AddScoped<JsonMediaWriter>();
            services.AddScoped<XmlMediaWriter>();
            services.AddScoped<JpegMediaWriter>();
            services.AddScoped<UncompressedMediaWriter>();

            services.AddScoped<Func<string, IDicomMediaWriter?>>(serviceProvider => (key) => {
                switch (key)
                {
                    case MimeMediaTypes.DICOM:
                        return serviceProvider.GetService<NativeMediaWriter>();
                    case MimeMediaTypes.JsonDicom:
                        return serviceProvider.GetService<JsonMediaWriter>();
                    case MimeMediaTypes.XmlDicom:
                        return serviceProvider.GetService<XmlMediaWriter>();
                    case MimeMediaTypes.Jpeg:
                        return serviceProvider.GetService<JpegMediaWriter>();
                    case MimeMediaTypes.UncompressedData:
                        return serviceProvider.GetService<UncompressedMediaWriter>();
                    default:
                        throw new KeyNotFoundException(); // or maybe return null, up to you
                }
            });
           
            services.AddScoped<IDicomMediaWriterFactory, DicomMediaWriterFactory>();
            services.AddScoped<IJsonDicomConverter, JsonDicomConverter>();
        }

        
        private static void RemoveAnonymizerTag ( DicomAnonymizer anonymizer, DicomTag tag )
        {
            var parenthesis = new[] { '(', ')' };
            var tagString = tag.ToString ( ).Trim(parenthesis);
            var action = anonymizer.Profile.FirstOrDefault(pair => pair.Key.IsMatch(tagString));
            anonymizer.Profile.Remove(action.Key);
        }
    }
}