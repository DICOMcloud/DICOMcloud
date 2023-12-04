using DICOMcloud.Media;

namespace DICOMcloud.Wado.WebApi.Core.App_Start
{
    public class Config
    {
        private readonly IConfiguration _configuration;

        public Config(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string StorageConection => _configuration.GetValue<string>("app:PacsStorageConnection");
        public string SupportPreSignedUrl => _configuration.GetValue<string>("app:supportPreSignedUrls");
        public string SupportSelfSignedUrl => _configuration.GetValue<string>("app:supportSelfSignedUrls");
        public string EnableAnonymizer => _configuration.GetValue<string>("app:enableAnonymizer");
        public string AnonymizerOptions => _configuration.GetValue<string>("app:anonymizerOptions");
        public string Config_WadoRs_API_URL => _configuration.GetValue<string>(RetrieveUrlProvider.config_WadoRs_API_URL);
        public string Config_WadoUri_API_URL => _configuration.GetValue<string>(RetrieveUrlProvider.config_WadoUri_API_URL);
        public bool ValidateDuplicateInstance => _configuration.GetValue<bool>("storecommand:validateDuplicateInstance");
        public bool StoreOriginalDataset => _configuration.GetValue<bool>("storecommand:storeOriginalDataset");
        public bool StoreQueryModel => _configuration.GetValue<bool>("storecommand:storeQueryModel");
        public IList<DicomMediaProperties> MediaTypes => _configuration.GetSection("storecommand:mediaTypes").Get<List<DicomMediaProperties>>();
        public string CorsEnabled => _configuration.GetValue<string>("cors:enabled");
        public string Origins => _configuration.GetValue<string>("cors:origins");
        public string Headers => _configuration.GetValue<string>("cors:headers");
        public string Methods => _configuration.GetValue<string>("cors:methods");

    }
}
