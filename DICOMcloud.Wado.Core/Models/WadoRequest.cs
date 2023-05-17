using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado.Models
{
    public class WadoUriRequest : IWadoUriRequest
    {
        public HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> AcceptHeader { get; set; }
        public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptCharsetHeader { get; set; }

        public string RequestType {  get; set; }
        public string ContentType   {  get; set; }
        public string Charset {  get ; set; }

        public string StudyInstanceUID   { get; set; }
        public string SeriesInstanceUID  { get; set; }
        public string SOPInstanceUID     { get; set; }
        public int?   Frame              { get; set; }
        public bool   Anonymize          { get; set; }
        
        public NameValueCollection        Query            { get; set; }
        public IWadoUriImageRequestParams ImageRequestInfo { get; set; }
        public HttpRequestHeaders         Headers          { get; set; }

        public override string ToString()
        {
            return "should be json!";
            //TODO:
            //return JsonConvert.SerializeObject(this);
        }
    }

   public class WadoUriImageRequestParams : IWadoUriImageRequestParams
   {
      public WadoBurnAnnotation BurnAnnotation { get; set; }

      public int? Rows    { get; set; }
      public int? Columns { get; set; }
      public int? FrameNumber  { get; set; }
      public int? ImageQuality { get; set; }

      public string Region                { get; set; }
      public string WindowCenter          { get; set ;}
      public string WindowWidth           { get; set ;}
      public string PresentationUID       { get; set; }
      public string presentationSeriesUID { get; set; }
      public string TransferSyntax        { get; set; }
   }
}
