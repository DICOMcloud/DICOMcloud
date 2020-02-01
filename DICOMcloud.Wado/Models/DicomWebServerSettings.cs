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
            PreSignedUrlReadExpiryTimeInHours = 1;
        }

        public static DicomWebServerSettings Instance
        { 
            get 
            { 
                return _instance;
            }
        }

        public double PreSignedUrlReadExpiryTimeInHours { get; set; }
        public bool CanUserOverridePreSignedUrlReadExpiryTime { get; set; }
        public bool SupportPreSignedUrls { get; set;}

        private static readonly DicomWebServerSettings _instance = DicomWebServerSettingsFactory.Create ( );

        internal abstract class DicomWebServerSettingsFactory
        {
            public static DicomWebServerSettings Create()
            {
                return new DicomWebServerSettings();
            }
        }

    }
}
