﻿using DICOMcloud.IO;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System;

namespace DICOMcloud.Wado.Models
{
    public interface IWebStoreRequest : IWadoRequestHeader
    {
        string MediaType { get; set; }

        Collection<HttpContent> Contents { get; }
    }

    public class WebStoreRequest : MultipartRelatedStreamProvider, IWebStoreRequest
    {
        public WebStoreRequest ( HttpRequestMessage request )
        {
            Request = request ;

            Headers             = request.Headers;
            AcceptCharsetHeader = Request.Headers.AcceptCharset ;
            AcceptHeader        = Request.Headers.Accept ;
        }

        public HttpRequestMessage Request { get; private set; }

        public HttpRequestHeaders Headers { get; set; }

        public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptCharsetHeader
        {
            get ;

            set ;
        }

        public HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> AcceptHeader
        {
            get ;

            set ;
        }

        public string MediaType
        {
            get; set;
        }

        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            NameValueHeaderValue dicomType = parent.Headers.ContentType.Parameters.Where ( n=>n.Name ==  "type" ).FirstOrDefault ( ) ;

            MediaType = dicomType.Value.Trim(new char[] {'"'}) ;

            return base.GetStream ( parent, headers ) ;
        }
    }
}
