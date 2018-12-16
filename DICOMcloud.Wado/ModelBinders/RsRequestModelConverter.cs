
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

using DICOMcloud.Wado.Models;

namespace DICOMcloud.Wado
{
    public class RsRequestModelConverter<T> where T :class
    {
        public RsRequestModelConverter ( )
        { }

        public virtual bool TryParse ( HttpRequestMessage request, ModelBindingContext bindingContext, out T result )
        {
            var query = request.RequestUri.ParseQueryString ( ) ;        
            result = null ;

            if ( typeof(T) == typeof(IWadoRsStudiesRequest) )
            {
                IWadoRsStudiesRequest wadoReq = CreateWadoRsStudiesRequestModel( );

                FillStudyParams(bindingContext.ValueProvider, wadoReq);

                wadoReq.QueryLevel = ObjectQueryLevel.Study;

                result = wadoReq as T;
            }

            if ( typeof(T) == typeof(IWadoRsSeriesRequest) )
            {
                IWadoRsSeriesRequest wadoReq = CreateWadoRsSeriesRequestModel ( );

                FillSeriesParams(bindingContext.ValueProvider, wadoReq);

                wadoReq.QueryLevel = ObjectQueryLevel.Series;

                result = wadoReq as T;
            }

            if ( typeof(T) == typeof(IWadoRsInstanceRequest) )
            {
                IWadoRsInstanceRequest wadoReq = CreateWadoRsInstanceRequestModel ( );

                FillInstanceParams(bindingContext.ValueProvider, wadoReq);

                wadoReq.QueryLevel = ObjectQueryLevel.Instance;

                result = wadoReq as T;
            }

            if ( typeof(T) == typeof(IWadoRsFramesRequest) )
            {
                IWadoRsFramesRequest wadoReq = CreateWadoRsFramesRequestModel ( );

                FillIFramesParams(bindingContext.ValueProvider, wadoReq);

                wadoReq.QueryLevel = ObjectQueryLevel.Instance;

                result = wadoReq as T;
            }

            if ( null != result)
            { 
                WadoRsRequestBase reqBase = result as WadoRsRequestBase ;

               reqBase.Headers              = request.Headers;
                reqBase.AcceptHeader        = request.Headers.Accept;
                reqBase.AcceptCharsetHeader = request.Headers.AcceptCharset;
                reqBase.QueryLevel          = ObjectQueryLevel.Instance ;
                
                return true ;
            }
            else
            { 
                return false ;
            }
        }

        protected virtual IWadoRsFramesRequest CreateWadoRsFramesRequestModel()
        {
            return new WadoRsFramesRequest();
        }

        protected virtual IWadoRsInstanceRequest CreateWadoRsInstanceRequestModel()
        {
            return new WadoRsInstanceRequest();
        }

        protected virtual IWadoRsSeriesRequest CreateWadoRsSeriesRequestModel()
        {
            return new WadoRsSeriesRequest();
        }

        protected virtual WadoRsStudiesRequest CreateWadoRsStudiesRequestModel ( )
        {
            return new WadoRsStudiesRequest();
        }

        protected virtual WadoBurnAnnotation ParseAnnotation ( string annotationString)
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

        protected virtual void FillStudyParams ( IValueProvider valueProvider, IWadoRsStudiesRequest result )
        { 
            result.StudyInstanceUID = valueProvider.GetValue ("StudyInstanceUID").RawValue as string  ;
        }

        protected virtual void FillSeriesParams ( IValueProvider valueProvider, IWadoRsSeriesRequest result )
        { 
            FillStudyParams ( valueProvider, result ) ;

            result.SeriesInstanceUID = valueProvider.GetValue ("SeriesInstanceUID").RawValue as string  ;
        }

        protected virtual void FillInstanceParams ( IValueProvider valueProvider, IWadoRsInstanceRequest result )
        { 
            FillSeriesParams ( valueProvider, result ) ;

            result.SOPInstanceUID = valueProvider.GetValue ("SOPInstanceUID").RawValue as string  ;
        }

        protected virtual void FillIFramesParams ( IValueProvider valueProvider, IWadoRsFramesRequest result )
        { 
            FillInstanceParams ( valueProvider, result ) ;

            result.Frames = ParseFrames ( valueProvider.GetValue ( "FrameList" ).RawValue as string ) ;
        }

        private int[] ParseFrames(string frames)
        {
            if (!string.IsNullOrEmpty(frames))
            {
                return frames.Split(',').Select(Int32.Parse).ToArray();
            }

            return null;
        }
    }

   //public abstract class WadoRequestKeys
   //{
   //   private WadoRequestKeys (){} 

   //   public const string RequestType           = "requestType" ;
   //   public const string StudyUID              = "studyUID" ;
   //   public const string SeriesUID             = "seriesUID" ;
   //   public const string ObjectUID             = "objectUID" ;
   //   public const string ContentType           = "contentType" ;
   //   public const string Charset               = "charset" ;
   //   public const string Anonymize             = "anonymize" ;
   //   public const string Annotation            = "annotation" ;
   //   public const string Rows                  = "rows" ;
   //   public const string Columns               = "columns" ;
   //   public const string Region                = "region" ;
   //   public const string WindowWidth           = "windowWidth" ;
   //   public const string WindowCenter          = "windowCenter" ;
   //   public const string FrameNumber           = "frameNumber" ;
   //   public const string ImageQuality          = "imageQuality" ;
   //   public const string PresentationUID       = "presentationUID" ;
   //   public const string PresentationSeriesUID = "presentationSeriesUID" ;
   //   public const string TransferSyntax        = "transferSyntax" ;
      
   //}
}
