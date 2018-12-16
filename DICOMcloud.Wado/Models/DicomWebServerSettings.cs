using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado.Models
{
    public class DicomWebServerSettings
    {
        private DicomWebServerSettings ()
        {
            SelfSignedUrlReadExpiryTimeInHours = 1;
        }

        public static DicomWebServerSettings Instance
        { 
            get 
            { 
                return _instance;
            }
        }

        public double SelfSignedUrlReadExpiryTimeInHours { get; set; }
        public bool CanUserOverrideSelfSignedUrlReadExpiryTime { get; set; }
        public bool SupportSelfSignedUrls { get; set;}

        private static DicomWebServerSettings _instance = DicomWebServerSettingsFactory.Create ( );

        internal abstract class DicomWebServerSettingsFactory
        {
            public static DicomWebServerSettings Create()
            {
                return new DicomWebServerSettings();
            }
        }

    }
}
