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
    using DICOMcloud.Azure.IO;
    using DICOMcloud.DataAccess;
    using DICOMcloud.DataAccess.Database;
    using DICOMcloud.DataAccess.Database.Schema;
    using DICOMcloud.IO;
    using DICOMcloud.Media;
    using DICOMcloud.Messaging;
    using DICOMcloud.Pacs;
    using DICOMcloud.Pacs.Commands;
    using DICOMcloud.Wado.Configs;
    using DICOMcloud.Wado.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.WindowsAzure.Storage;

    #endregion

    /// <summary>
    ///     The DicomExtensions.
    /// </summary>
    public class DicomExtensions
    {
        #region Fields
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        private readonly IOptions<UrlOptions> _options;
        public bool AzureStorageSupported { get; protected set; }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DicomExtensions"/> class.
        /// </summary>
        public DicomExtensions(
        IConfiguration configuration,
        IServiceCollection services,
        IOptions<UrlOptions> options)
        {
            this._configuration = configuration ?? throw new ArgumentException(nameof(configuration));
            this._services = services ?? throw new ArgumentException(nameof(services));
            this._options = options ?? throw new ArgumentException(nameof(options));
        }

        #endregion

        #region Properties

        public string StorageConection { get; set; }
        protected CloudStorageAccount StorageAccount { get; private set; }

        #endregion

        #region Public Methods And Operators

        public void Build()
        {
            this.Init();
            this.RegisterEvents();
            this.RegisterComponents();
            this.RegisterMediaWriters();
            this.EnsureCodecsLoaded ( ) ;
        }




        #endregion

        #region Other Methods

        private void Init()
        {
            this.StorageConection = this._configuration.GetConnectionString("pacsStorageConnection");
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
            this._services.AddScoped<DbSchemaProvider, StorageDbSchemaProvider>();
            this._services.AddScoped<IDatabaseFactory, SqlDatabaseFactory>();
            this._services.AddScoped<ISortingStrategyFactory, SortingStrategyFactory>();
            this._services.AddScoped<ObjectArchieveDataAdapter, ObjectArchieveDataAdapter>();
            this._services.AddScoped<IObjectArchieveDataAccess, ObjectArchieveDataAccess>();
            IRetrieveUrlProvider urlProvider = new RetrieveUrlProvider(this._options.Value.WadoRsUrl, this._options.Value.WadoUriUrl);

            this._services.AddScoped<IDCloudCommandFactory, DCloudCommandFactory>();
            this._services.AddScoped<IObjectArchieveQueryService, ObjectArchieveQueryService>();
            this._services.AddScoped<IObjectStoreService, ObjectStoreService>();
            this._services.AddScoped<IObjectRetrieveService, ObjectRetrieveService>();
            this._services.AddScoped<IWadoRsService, WadoRsService>();
            this._services.AddScoped<IWebObjectStoreService, WebObjectStoreService>();
            this._services.AddScoped<IQidoRsService, QidoRsService>();
            this._services.AddScoped<IWadoUriService, WadoUriService>();
            this._services.AddScoped<IOhifService, OhifService>();
            this._services.AddScoped<IDicomMediaIdFactory, DicomMediaIdFactory>();
            this._services.AddScoped<IRetrieveUrlProvider>(x => urlProvider);
            this._services.AddScoped<IConnectionStringProvider, ConnectionStringProvider>();
            RegisterStoreCommandSettings();

            //todo: implement properly
            if (StorageConection.StartsWith("|datadirectory|", StringComparison.OrdinalIgnoreCase))
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var lastIndex = StorageConection.IndexOf('|', 1);
                var userPathPart = StorageConection.Substring(lastIndex + 1);


                StorageConection = appDataPath + userPathPart;
            }

            if (System.IO.Path.IsPathRooted(StorageConection))
            {
                this._services.AddScoped<IKeyProvider, HashedFileKeyProvider>();
                this._services.AddScoped<IMediaStorageService>(x => new FileStorageService(StorageConection));
                // For<IMediaStorageService>().Use<FileStorageService>().Ctor<string>().Is(StorageConection);
            }
            else
            {
                StorageAccount = CloudStorageAccount.Parse(StorageConection);
                AzureStorageSupported = true;
                this._services.AddScoped<CloudStorageAccount>(x => StorageAccount);
                // todo: implement properly
                // this._services.AddScoped <CloudStorageAccount>(x => StorageAccount);
                // For<IMediaStorageService>().Use<AzureStorageService>().Ctor<CloudStorageAccount>().Is(StorageAccount);
            }
        }

        private void RegisterStoreCommandSettings()
        {
            StorageSettings storageSettings = new StorageSettings();


            var validateDuplicateInstance = this._configuration.GetValue<string>("Storecommand.ValidateDuplicateInstance");
            var storeOriginalDataset = this._configuration.GetValue<string>("Storecommand.StoreOriginalDataset");
            var storeQueryModel = this._configuration.GetValue<string>("Storecommand.StoreQueryModel");

            if (bool.TryParse(validateDuplicateInstance, out bool validateDuplicateValue))
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

            this._services.AddScoped<StorageSettings>(x => storageSettings);
        }

        protected virtual void RegisterMediaWriters()
        {
            //var factory = new InjectionFactory(c => new Func<string, IDicomMediaWriter> (name => c.Resolve<IDicomMediaWriter>(name))) ;

            // For<IDicomMediaWriter>().Use<NativeMediaWriter>().Named(MimeMediaTypes.DICOM);
            // For<IDicomMediaWriter>().Use<JsonMediaWriter>().Named(MimeMediaTypes.Json);
            // For<IDicomMediaWriter>().Use<XmlMediaWriter>().Named(MimeMediaTypes.xmlDicom);
            // For<IDicomMediaWriter>().Use<JpegMediaWriter>().Named(MimeMediaTypes.Jpeg);
            // For<IDicomMediaWriter>().Use<UncompressedMediaWriter>().Named(MimeMediaTypes.UncompressedData);

            // For<Func<string, IDicomMediaWriter>>().Use((m => new Func<String, IDicomMediaWriter>(name => m.TryGetInstance<IDicomMediaWriter>(name))));

            this._services.AddScoped<IDicomMediaWriterFactory,DicomMediaWriterFactory>();
            this._services.AddScoped<IJsonDicomConverter,JsonDicomConverter>();

            // For<IJsonDicomConverter>().Use<JsonDicomConverter>();
            
        }

        protected virtual void EnsureCodecsLoaded ( ) 
        {
            // var path = System.IO.Path.Combine ( System.Web.Hosting.HostingEnvironment.MapPath ( "~/" ), "bin" );

            // System.Diagnostics.Trace.TraceInformation ( "Path: " + path );

            // fo.Imaging.Codec.TranscoderManager.LoadCodecs ( path ) ;
        }

        #endregion
    }
}