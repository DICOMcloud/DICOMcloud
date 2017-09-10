using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud;

namespace DICOMcloud.Wado
{
    public class RetieveUrlProvider : IRetieveUrlProvider
    {
        public static string config_WadoRs_API_URL = "app:WadoRsUrl" ;
        public static string config_WadoUri_API_URL = "app:WadoUriUrl" ;

        public RetieveUrlProvider ( )
        {
            string wadoRsUrl = System.Configuration.ConfigurationManager.AppSettings[config_WadoRs_API_URL] ;
            string wadoUriUrl = System.Configuration.ConfigurationManager.AppSettings[config_WadoUri_API_URL] ;
            

            wadoRsUrl = wadoRsUrl ?? "" ;
            wadoUriUrl = wadoUriUrl ?? "" ;

            Init ( wadoRsUrl, wadoUriUrl ) ;
        }

        public RetieveUrlProvider ( string wadoRsUrl, string wadoUriUrl )
        {
            Init ( wadoRsUrl, wadoUriUrl ) ;
        }

        public string GetStudyUrl ( string studyInstanceUID )
        {
            return string.Format ( "{0}/{1}/studies/{2}", BaseWadoRsUrl, "wadors", studyInstanceUID )  ;
        }

        public string GetInstanceUrl ( IObjectId instance )
        {
            return GetInstanceUrl ( instance.StudyInstanceUID, instance.SeriesInstanceUID, instance.SOPInstanceUID ) ;
        }
        
        public string GetInstanceUrl ( string studyInstanceUID, string seriesInstanceUID, string sopInstanceUID )
        {
            if ( PreferWadoUri )
            {
                return string.Format ( "{0}?RequestType=wado&studyUID={1}&seriesUID={2}&objectUID={3}&&contentType=application/dicom", BaseWadoUriUrl, studyInstanceUID, seriesInstanceUID, sopInstanceUID )  ;
            }
            else
            {
                return string.Format ( "{0}/studies/{1}/series/{2}/instances/{3}", BaseWadoRsUrl, studyInstanceUID, seriesInstanceUID, sopInstanceUID )  ;
            }
        }

        public string BaseWadoRsUrl  { get; set; }
        public string BaseWadoUriUrl { get; set; }
        public bool PreferWadoUri    { get; set; }


        private void Init ( string wadoRsUrl, string wadoUriUrl ) 
        {
            BaseWadoRsUrl  = wadoRsUrl ;
            BaseWadoUriUrl = wadoUriUrl ;
            PreferWadoUri  = true ;
        }
    }
}
