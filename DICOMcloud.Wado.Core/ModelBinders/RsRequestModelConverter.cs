
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http.Headers;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Net.Http.Headers;

namespace DICOMcloud.Wado
{
    public class RsRequestModelConverter<T> where T :class
    {
        public RsRequestModelConverter ( )
        { }

        public virtual bool TryParse ( ModelBindingContext bindingContext, out T result )
        {
            var request = bindingContext.HttpContext.Request;
            var valueProvider = bindingContext.ValueProvider;
            result = null;

            result = CreateWadoRsModel(bindingContext.ValueProvider, result);

            if (null != result)
            {
                WadoRsRequestBase reqBase = result as WadoRsRequestBase;
                StringValues charsetValue;
                IEnumerable<MediaTypeHeaderValue> accept;
                IEnumerable<StringWithQualityHeaderValue> acceptCharsetHeader;

                accept              = GetAcceptMediaTypes(bindingContext, request);
                acceptCharsetHeader = GetCharsetHeader(bindingContext, request);

                reqBase.Headers = request.GetTypedHeaders();
                reqBase.AcceptHeader = accept;
                reqBase.AcceptCharsetHeader = acceptCharsetHeader;
                reqBase.QueryLevel = ObjectQueryLevel.Instance;

                return true;
            }
            else
            {
                return false;
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

        protected virtual void FillStudyParams (IValueProvider valueProvider, IWadoRsStudiesRequest result )
        {
            result.StudyInstanceUID = valueProvider.GetValue("StudyInstanceUID").FirstValue;
        }

        protected virtual void FillSeriesParams (IValueProvider valueProvider, IWadoRsSeriesRequest result )
        { 
            FillStudyParams ( valueProvider, result ) ;

            result.SeriesInstanceUID = valueProvider.GetValue("SeriesInstanceUID").FirstValue;
        }

        protected virtual void FillInstanceParams (IValueProvider valueProvider, IWadoRsInstanceRequest result )
        { 
            FillSeriesParams ( valueProvider, result ) ;

            result.SOPInstanceUID = valueProvider.GetValue("SOPInstanceUID").FirstValue;
        }

        protected virtual void FillIFramesParams(IValueProvider valueProvider, IWadoRsFramesRequest result)
        {
            FillInstanceParams(valueProvider, result);

            result.Frames = ParseFrames(valueProvider.GetValue("FrameList").FirstValue);
        }

        private int[] ParseFrames(string frames)
        {
            if (!string.IsNullOrEmpty(frames))
            {
                return frames.Split(',').Select(Int32.Parse).ToArray();
            }

            return null;
        }

        private static IEnumerable<StringWithQualityHeaderValue> GetCharsetHeader
        (
            ModelBindingContext bindingContext,
            HttpRequest request
        )
        {
            StringValues charsetValue;


            if (bindingContext.HttpContext.Request.Query.TryGetValue("charset", out charsetValue))
            {
                List<StringWithQualityHeaderValue> acceptCharsetHeader;


                acceptCharsetHeader = new List<StringWithQualityHeaderValue>();

                charsetValue.Select((string charsettSeg) =>
                {
                    var charset = new StringWithQualityHeaderValue(charsettSeg);
                    acceptCharsetHeader.Add(charset);

                    return charset;
                });

                return acceptCharsetHeader;
            }
            else
            {
                return request.GetTypedHeaders().AcceptCharset;
            }
        }

        private static IEnumerable<MediaTypeHeaderValue> GetAcceptMediaTypes
        (
            ModelBindingContext bindingContext,
            HttpRequest request
        )
        {
            StringValues acceptValue;

            if (bindingContext.HttpContext.Request.Query.TryGetValue("accept", out acceptValue))
            {
                List<MediaTypeHeaderValue> acceptList;


                acceptList = new List<MediaTypeHeaderValue>();

                acceptValue.Select((string acceptSeg) =>
                {
                    var mediaType = new MediaTypeHeaderValue(acceptSeg);

                    acceptList.Add(mediaType);

                    return mediaType;
                });

                return acceptList;
            }
            else
            {
                return GetAcceptHeaders(request);
            }
        }

        private T CreateWadoRsModel(IValueProvider valueProvider, T result)
        {
            if (typeof(T) == typeof(IWadoRsStudiesRequest))
            {
                IWadoRsStudiesRequest wadoReq = CreateWadoRsStudiesRequestModel();

                FillStudyParams(valueProvider, wadoReq);

                wadoReq.QueryLevel = ObjectQueryLevel.Study;

                result = wadoReq as T;
            }

            if (typeof(T) == typeof(IWadoRsSeriesRequest))
            {
                IWadoRsSeriesRequest wadoReq = CreateWadoRsSeriesRequestModel();

                FillSeriesParams(valueProvider, wadoReq);

                wadoReq.QueryLevel = ObjectQueryLevel.Series;

                result = wadoReq as T;
            }

            if (typeof(T) == typeof(IWadoRsInstanceRequest))
            {
                IWadoRsInstanceRequest wadoReq = CreateWadoRsInstanceRequestModel();

                FillInstanceParams(valueProvider, wadoReq);

                wadoReq.QueryLevel = ObjectQueryLevel.Instance;

                result = wadoReq as T;
            }

            if (typeof(T) == typeof(IWadoRsFramesRequest))
            {
                IWadoRsFramesRequest wadoReq = CreateWadoRsFramesRequestModel();

                FillIFramesParams(valueProvider, wadoReq);

                wadoReq.QueryLevel = ObjectQueryLevel.Instance;

                result = wadoReq as T;
            }

            return result;
        }

        private static IEnumerable<MediaTypeHeaderValue> GetAcceptHeaders(HttpRequest request)
        {
            IEnumerable<MediaTypeHeaderValue> accept;
            {
                var acceptList = new List<MediaTypeHeaderValue>();

                foreach (var acceptHeaderValue in request.Headers["accept"])
                {
                    var acceptHeaderValues = acceptHeaderValue.Split(",");

                    foreach (var acceptHeaderString in acceptHeaderValues)
                    {
                        if (acceptHeaderString.Contains("multipart"))
                        {
                            var multiparts = acceptHeaderString.Split(";");
                            var types = multiparts[1].Split("=");

                            if (!types[1].Trim().StartsWith("\""))
                            {
                                var newValue = acceptHeaderString.Replace(types[1], "\"" + types[1] + "\"");

                                acceptList.Add(MediaTypeHeaderValue.Parse(newValue));
                            }
                            else
                            {
                                acceptList.Add(MediaTypeHeaderValue.Parse(acceptHeaderString));
                            }
                        }
                        else
                        {
                            acceptList.Add(new MediaTypeHeaderValue(acceptHeaderString));
                        }
                    }
                }

                accept = acceptList;
            }

            return accept;
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
