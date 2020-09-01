using System;
using DICOMcloud.Wado.Configs;
using Microsoft.Extensions.Options;

namespace DICOMcloud.Wado
{
    public class RetrieveUrlProvider : IRetrieveUrlProvider
    {
        private readonly IOptions<UrlOptions> _options;

        public RetrieveUrlProvider(IOptions<UrlOptions> options)
        {
            this._options = options ?? throw new ArgumentException(nameof(options));
            Init ( this._options.Value.WadoRsUrl, this._options.Value.WadoUriUrl ) ;
        }

        public RetrieveUrlProvider( string wadoRsUrl, string wadoUriUrl )
        {
            Init ( wadoRsUrl, wadoUriUrl ) ;
        }

        public string GetStudyUrl(IStudyId study)
        {
            return GetStudyUrl (study.StudyInstanceUID);
        }

        private string GetStudyUrl (string studyInstanceUID)
        {
            return string.Format("{0}/{1}/studies/{2}", BaseWadoRsUrl, "wadors", studyInstanceUID);
        }

        public virtual string GetInstanceUrl ( IObjectId instance )
        {
            return GetInstanceUrl ( instance.StudyInstanceUID, instance.SeriesInstanceUID, instance.SOPInstanceUID ) ;
        }
        
        private string GetInstanceUrl 
        (
            string studyInstanceUID, 
            string seriesInstanceUID, 
            string sopInstanceUID
        )
        {
            if ( PreferWadoUri )
            {
                return BaseWadoUriUrl + GenerateWadoUriPart (studyInstanceUID, seriesInstanceUID, sopInstanceUID );
            }
            else
            {
                return BaseWadoRsUrl + GenerateWadoRsPart (studyInstanceUID, seriesInstanceUID, sopInstanceUID);
            }
        }

        protected virtual string GenerateWadoUriPart
        ( 
            string studyInstanceUID, 
            string seriesInstanceUID, 
            string sopInstanceUID
        )
        {
            return string.Format("?RequestType=wado&studyUID={0}&seriesUID={1}&objectUID={2}&&contentType=application/dicom", studyInstanceUID, seriesInstanceUID, sopInstanceUID);
        }

        protected virtual string GenerateWadoRsPart
        (
            string studyInstanceUID,
            string seriesInstanceUID,
            string sopInstanceUID
        )
        {
            return string.Format("/studies/{0}/series/{1}/instances/{2}", studyInstanceUID, seriesInstanceUID, sopInstanceUID);
        }


        public virtual string BaseWadoRsUrl  { get; set; }
        public virtual string BaseWadoUriUrl { get; set; }
        public virtual bool PreferWadoUri    { get; set; }


        private void Init ( string wadoRsUrl, string wadoUriUrl ) 
        {
            BaseWadoRsUrl  = wadoRsUrl ;
            BaseWadoUriUrl = wadoUriUrl ;
            PreferWadoUri  = true ;
        }
    }
}
