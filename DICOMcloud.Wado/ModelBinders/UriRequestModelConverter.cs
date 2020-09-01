
using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud.Wado;


namespace DICOMcloud.Wado
{
    public class UriRequestModelConverter 
    {
        public UriRequestModelConverter ( )
      { }

        public bool TryParse ( HttpRequestMessage request, out IWadoUriRequest result )
        {
            IWadoUriRequest wadoReq = CreateWadoUriRequestModel ( );

            wadoReq.Headers = request.Headers;
            wadoReq.AcceptHeader = request.Headers.Accept;
            wadoReq.AcceptCharsetHeader = request.Headers.AcceptCharset;
            var query = request.RequestUri.ParseQuery(); 

            wadoReq.Query = query; 
            wadoReq.RequestType = query[WadoRequestKeys.RequestType];
            wadoReq.StudyInstanceUID = query[WadoRequestKeys.StudyUID];
            wadoReq.SeriesInstanceUID = query[WadoRequestKeys.SeriesUID];
            wadoReq.SOPInstanceUID = query[WadoRequestKeys.ObjectUID];
            wadoReq.ContentType = query[WadoRequestKeys.ContentType];
            wadoReq.Charset = query[WadoRequestKeys.Charset];

            wadoReq.Anonymize = string.Compare(query[WadoRequestKeys.Anonymize], "yes", true) == 0;

            wadoReq.ImageRequestInfo = new WadoUriImageRequestParams();
            wadoReq.ImageRequestInfo.BurnAnnotation = ParseAnnotation(query[WadoRequestKeys.Annotation]);
            wadoReq.ImageRequestInfo.Rows = GetIntValue(query[WadoRequestKeys.Rows]);
            wadoReq.ImageRequestInfo.Columns = GetIntValue(query[WadoRequestKeys.Columns]);
            wadoReq.ImageRequestInfo.Region = query[WadoRequestKeys.Region];
            wadoReq.ImageRequestInfo.WindowWidth = query[WadoRequestKeys.WindowWidth];
            wadoReq.ImageRequestInfo.WindowCenter = query[WadoRequestKeys.WindowCenter];
            wadoReq.Frame = wadoReq.ImageRequestInfo.FrameNumber = GetIntValue(query[WadoRequestKeys.FrameNumber]);
            wadoReq.ImageRequestInfo.ImageQuality = GetIntValue(query[WadoRequestKeys.ImageQuality]);
            wadoReq.ImageRequestInfo.PresentationUID = query[WadoRequestKeys.PresentationUID];
            wadoReq.ImageRequestInfo.presentationSeriesUID = query[WadoRequestKeys.PresentationSeriesUID];
            wadoReq.ImageRequestInfo.TransferSyntax = query[WadoRequestKeys.TransferSyntax];

            result = wadoReq;

            return true;
        }

        protected virtual IWadoUriRequest CreateWadoUriRequestModel ( )
        {
            return new WadoUriRequest();
        }

        private WadoBurnAnnotation ParseAnnotation ( string annotationString)
      {
         WadoBurnAnnotation annotation = WadoBurnAnnotation.None ;

         if ( !string.IsNullOrWhiteSpace ( annotationString ) )
         { 
            string[] parts = annotationString.Trim().Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries) ;
         
            foreach(string part in parts)
            {
               WadoBurnAnnotation tempAnn ; 
               
               if ( Enum.TryParse<WadoBurnAnnotation>(part.Trim(), true, out tempAnn ) )
               { 
                  annotation |= tempAnn ;
               }
            }
         }

         return annotation ;
      }

      private int? GetIntValue ( string stringValue )
      {
         if ( string.IsNullOrWhiteSpace(stringValue))
         {
            return null ;
         }
         else
         { 
            int parsedVal ;

            if ( int.TryParse (stringValue.Trim(), out parsedVal))
            { 
               return parsedVal ;
            }
            else
            { 
               return null ;
            }
         }
      }
   }

   public abstract class WadoRequestKeys
   {
      private WadoRequestKeys (){} 

      public const string RequestType           = "requestType" ;
      public const string StudyUID              = "studyUID" ;
      public const string SeriesUID             = "seriesUID" ;
      public const string ObjectUID             = "objectUID" ;
      public const string ContentType           = "contentType" ;
      public const string Charset               = "charset" ;
      public const string Anonymize             = "anonymize" ;
      public const string Annotation            = "annotation" ;
      public const string Rows                  = "rows" ;
      public const string Columns               = "columns" ;
      public const string Region                = "region" ;
      public const string WindowWidth           = "windowWidth" ;
      public const string WindowCenter          = "windowCenter" ;
      public const string FrameNumber           = "frameNumber" ;
      public const string ImageQuality          = "imageQuality" ;
      public const string PresentationUID       = "presentationUID" ;
      public const string PresentationSeriesUID = "presentationSeriesUID" ;
      public const string TransferSyntax        = "transferSyntax" ;
      
   }
}
