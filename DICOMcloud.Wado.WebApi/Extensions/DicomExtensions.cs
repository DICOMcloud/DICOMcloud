// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DicomExtensions" company="WeBuyCars">
//
// </copyright>
// <summary>
//   The class DicomExtensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DICOMcloud.Wado.WebApi.Extensions
{
    #region Usings

    using System;
    using System.Linq;
    using Dicom;
    using DICOMcloud.DataAccess;
    using DICOMcloud.DataAccess.Database;
    using DICOMcloud.DataAccess.Database.Schema;
    using DICOMcloud.Messaging;
    using DICOMcloud.Pacs;
    using DICOMcloud.Pacs.Commands;
    using DICOMcloud.Wado.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    #endregion

    /// <summary>
    ///     The DicomExtensions.
    /// </summary>
    public class DicomExtensions
    {
        #region Fields
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        #endregion

        #region Public Properties

        public DicomExtensions(
            IConfiguration configuration,
            IServiceCollection services)
        {
            this._configuration = configuration ?? throw new ArgumentException(nameof(configuration));
            this._services = services ?? throw new ArgumentException(nameof(services));
        }


        #endregion

        #region Public Methods And Operators

        public void Build()
        {
            this.Init();
            this.RegisterEvents();
            RegisterComponents();

            //             RegisterMediaWriters ( ) ;

            //             EnsureCodecsLoaded ( ) ;
        }




        #endregion

        #region Other Methods

        private void Init()
        {
            var StorageConection = this._configuration.GetConnectionString("pacsStorageConnection");
            var supportPreSignedUrl = this._configuration.GetValue<string>("Other:supportPreSignedUrls");

            if (!string.IsNullOrWhiteSpace(supportPreSignedUrl))
            {
                DicomWebServerSettings.Instance.SupportPreSignedUrls = bool.Parse(supportPreSignedUrl.Trim());
            }
        }

        private void RegisterEvents()
        {
            RegisterAnonymizer();
        }

        private void RegisterAnonymizer()
        {
            string enableAnonymizer = this._configuration.GetValue<string>("Anonymous:EnableAnonymizer");
            bool enable = true;


            if (!bool.TryParse(enableAnonymizer, out enable) || enable)
            {
                string anonymizerOptions = this._configuration.GetValue<string>("Anonymous:AnonymizerOptions");
                var options = DicomAnonymizer.SecurityProfileOptions.BasicProfile |
                                 DicomAnonymizer.SecurityProfileOptions.RetainUIDs |
                                 DicomAnonymizer.SecurityProfileOptions.RetainLongFullDates |
                                 DicomAnonymizer.SecurityProfileOptions.RetainPatientChars;

                if (!string.IsNullOrWhiteSpace(anonymizerOptions))
                {
                    options = (DicomAnonymizer.SecurityProfileOptions)Enum.Parse(typeof(DicomAnonymizer.SecurityProfileOptions), anonymizerOptions, true);
                }

                var anonymizer = new DicomAnonymizer(DicomAnonymizer.SecurityProfile.LoadProfile(null, options));

                anonymizer.Profile.PatientName = "Dcloud^Anonymized";
                anonymizer.Profile.PatientID = "Dcloud.Anonymized";

                RemoveAnonymizerTag(anonymizer, DicomTag.PatientName);
                RemoveAnonymizerTag(anonymizer, DicomTag.PatientID);

                PublisherSubscriberFactory.Instance.Subscribe<WebStoreDatasetProcessingMessage>(this, (message) =>
                {
                    var queryParams = message.Request.Request.RequestUri.ParseQuery();

                    anonymizer.AnonymizeInPlace(message.Dataset);

                    if (null != queryParams)
                    {
                        foreach (var queryKey in queryParams.OfType<String>())
                        {
                            uint tag;
                            if (string.IsNullOrWhiteSpace(queryKey)) { continue; }
                            if (uint.TryParse(queryKey, System.Globalization.NumberStyles.HexNumber, null, out tag))
                            {
                                message.Dataset.AddOrUpdate(tag, queryParams[queryKey]);
                            }
                        }
                    }
                });
            }
        }

        private static void RemoveAnonymizerTag(DicomAnonymizer anonymizer, DicomTag tag)
        {
            var parenthesis = new[] { '(', ')' };
            var tagString = tag.ToString().Trim(parenthesis);
            var action = anonymizer.Profile.FirstOrDefault(pair => pair.Key.IsMatch(tagString));
            anonymizer.Profile.Remove(action.Key);
        }

        protected virtual void RegisterComponents()
        {
            this._services.AddScoped<DbSchemaProvider,StorageDbSchemaProvider>(); 
            this._services.AddScoped<IDatabaseFactory,SqlDatabaseFactory>(); 
            this._services.AddScoped<ISortingStrategyFactory,SortingStrategyFactory>(); 
            this._services.AddScoped<ObjectArchieveDataAdapter>(); 
            this._services.AddScoped<ObjectArchieveDataAccess>(); 

            // IRetrieveUrlProvider urlProvider = new RetrieveUrlProvider(CloudConfigurationManager.GetSetting(RetrieveUrlProvider.config_WadoRs_API_URL),
            //                                                              CloudConfigurationManager.GetSetting(RetrieveUrlProvider.config_WadoUri_API_URL));
            this._services.AddScoped<IDCloudCommandFactory, DCloudCommandFactory>(); 
            this._services.AddScoped<IObjectArchieveQueryService, ObjectArchieveQueryService>(); 
            this._services.AddScoped<IObjectStoreService, ObjectStoreService>(); 
            this._services.AddScoped<IObjectRetrieveService, ObjectRetrieveService>(); 

            // For<IWadoRsService>().Use<WadoRsService>();
            // For<IWebObjectStoreService>().Use<WebObjectStoreService>();
            // For<IQidoRsService>().Use<QidoRsService>();
            // For<IWadoUriService>().Use<WadoUriService>();
            // For<IOhifService>().Use<OhifService>();

            // For<IDicomMediaIdFactory>().Use<DicomMediaIdFactory>();

            // For<IRetrieveUrlProvider>().Use(urlProvider);

            // RegisterStoreCommandSettings();

            // if (StorageConection.StartsWith("|datadirectory|", StringComparison.OrdinalIgnoreCase))
            // {
            //     var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //     var lastIndex = StorageConection.IndexOf('|', 1);
            //     var userPathPart = StorageConection.Substring(lastIndex + 1);


            //     StorageConection = appDataPath + userPathPart;
            // }

            // if (System.IO.Path.IsPathRooted(StorageConection))
            // {
            //     For<IKeyProvider>().Use<HashedFileKeyProvider>();
            //     For<IMediaStorageService>().Use<FileStorageService>().Ctor<string>().Is(StorageConection);
            // }
            // else
            // {
            //     StorageAccount = CloudStorageAccount.Parse(StorageConection);

            //     AzureStorageSupported = true;

            //     For<CloudStorageAccount>().Use(@StorageAccount);

            //     For<IMediaStorageService>().Use<AzureStorageService>().Ctor<CloudStorageAccount>().Is(StorageAccount);
            // }
        }

        // private void RegisterStoreCommandSettings()
        // {
        //     StorageSettings storageSettings = new StorageSettings();


        //     var validateDuplicateInstance = CloudConfigurationManager.GetSetting("app:storecommand.validateDuplicateInstance");
        //     var storeOriginalDataset = CloudConfigurationManager.GetSetting("app:storecommand.storeOriginalDataset");
        //     var storeQueryModel = CloudConfigurationManager.GetSetting("app:storecommand.storeQueryModel");

        //     if (bool.TryParse(validateDuplicateInstance, out bool validateDuplicateValue))
        //     {
        //         storageSettings.ValidateDuplicateInstance = validateDuplicateValue;
        //     }

        //     if (bool.TryParse(storeOriginalDataset, out bool storeOriginalDatasetValue))
        //     {
        //         storageSettings.StoreOriginal = storeOriginalDatasetValue;
        //     }

        //     if (bool.TryParse(storeQueryModel, out bool storeQueryModelValue))
        //     {
        //         storageSettings.StoreQueryModel = validateDuplicateValue;
        //     }

        //     For<StorageSettings>().Use(@storageSettings);
        // }

        // protected virtual void RegisterMediaWriters()
        // {
        //     //var factory = new InjectionFactory(c => new Func<string, IDicomMediaWriter> (name => c.Resolve<IDicomMediaWriter>(name))) ;

        //     For<IDicomMediaWriter>().Use<NativeMediaWriter>().Named(MimeMediaTypes.DICOM);
        //     For<IDicomMediaWriter>().Use<JsonMediaWriter>().Named(MimeMediaTypes.Json);
        //     For<IDicomMediaWriter>().Use<XmlMediaWriter>().Named(MimeMediaTypes.xmlDicom);
        //     For<IDicomMediaWriter>().Use<JpegMediaWriter>().Named(MimeMediaTypes.Jpeg);
        //     For<IDicomMediaWriter>().Use<UncompressedMediaWriter>().Named(MimeMediaTypes.UncompressedData);

        //     For<Func<string, IDicomMediaWriter>>().Use((m => new Func<String, IDicomMediaWriter>(name => m.TryGetInstance<IDicomMediaWriter>(name))));

        //     For<IDicomMediaWriterFactory>().Use<DicomMediaWriterFactory>();

        //     For<IJsonDicomConverter>().Use<JsonDicomConverter>();
        // }


        #endregion
    }
}