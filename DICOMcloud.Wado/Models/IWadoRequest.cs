using System;
using System.Collections.Specialized;

namespace DICOMcloud.Wado.Models
{
    public interface IWadoUriRequest : IWadoRequestHeader, IObjectId
   {
      string RequestType   { get; set; }
      string ContentType   { get; set; }
      string Charset       { get; set; }
      bool   Anonymize     { get; set; }

      NameValueCollection Query { get; set; }

      IWadoUriImageRequestParams ImageRequestInfo { get; set ;}
   }

   public interface IWadoUriImageRequestParams
   {
      WadoBurnAnnotation BurnAnnotation { get; set; }

      int? Rows    { get; set; }
      int? Columns { get; set; }
      int? FrameNumber  { get; set; }
      int? ImageQuality { get; set; }

      string Region                { get; set; }
      string WindowCenter          { get; set ;}
      string WindowWidth           { get; set ;}
      string PresentationUID       { get; set; }
      string presentationSeriesUID { get; set; }
      string TransferSyntax        { get; set; }
   }

   [Flags]
   public enum WadoBurnAnnotation
   { 
      None = 0,
      Patient = 1,
      Technique = 2
   }
}
