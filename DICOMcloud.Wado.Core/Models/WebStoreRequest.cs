using DICOMcloud.IO;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http.Extensions;
using System.Threading.Tasks;

namespace DICOMcloud.Wado.Models
{
    public interface IWebStoreRequest : IWadoRequestHeader
    {
        string MediaType { get; set; }

        IAsyncEnumerable<MultipartSection> GetContents();
    }

    public class WebStoreRequest : IWebStoreRequest
    {
        public WebStoreRequest ( HttpRequest request )
        {
            Request = request ;

            Headers             = request.GetTypedHeaders();
            AcceptCharsetHeader = Headers.AcceptCharset ;
            AcceptHeader        = Headers.Accept ;

            var dicomType = Headers.ContentType.Parameters.Where(n => n.Name == "type").FirstOrDefault();

            if ( dicomType != null ) 
            {
                MediaType = dicomType.Value.Value.Trim(new char[] { '"' });
            }
        }

        public HttpRequest Request { get; private set; }

        public RequestHeaders Headers { get; set; }

        public IEnumerable<StringWithQualityHeaderValue> AcceptCharsetHeader
        {
            get ;

            set ;
        }

        public IEnumerable<MediaTypeHeaderValue> AcceptHeader
        {
            get ;

            set ;
        }

        public string MediaType
        {
            get; set;
        }

        public async IAsyncEnumerable<MultipartSection> GetContents()
        { 
            var boundary = Request.GetMultipartBoundary();
                
            if ( boundary != null ) 
            {
                var reader = new MultipartReader(boundary, Request.Body);
                MultipartSection section;
                
                
                while ((section = await reader.ReadNextSectionAsync()) != null) 
                {
                    yield return section;
                }
            }

            yield break;
        }
    }
}
