using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud.DataAccess;

namespace DICOMcloud.Wado
{
    /// <summary>
    /// Generate HTTP Link Headers that points to pages from a <see cref="PagedResult{T}"/>
    /// </summary>
    /// <remarks>
    /// Use to send pagination links in response to a client by adding a standard HTTP Link Header [RFC 5988](https://tools.ietf.org/html/rfc5988)
    /// 
    /// The technique is similar to Github method of [Traversing with Pagination](https://developer.github.com/guides/traversing-with-pagination/)
    /// 
    /// The Implementation could be extended to support more pagination information by returning other page links than the 4 implemented here (First, Previous, Next and Last). 
    /// </remarks>
    public class LinkHeaderBuilder
    {
        private static string URL_FORMATTED = "<{0}?offset={1}&limit={2}>;" ;
        private static string REL_FORMATTED = " rel=\"{0}\"" ;

        public LinkHeaderBuilder ( )
        {
            Labels = new LinkHeaderLabels ( ) ;

        }

        public LinkHeaderLabels Labels { get; set; }

        /// <summary>
        /// Build the link header based on the information provided by <paramref name="pagedResult"/>
        /// </summary>
        /// <typeparam name="T">Type of the entities in the result</typeparam>
        /// <param name="pagedResult">Contains the page information</param>
        /// <param name="currentRoute">Resource route (URL) where paging information will be appended</param>
        /// <returns>A string that is used as the value for the link header. Example:
        /// Link: <https://api.github.com/search/code?q=addClass+user%3Amozilla&page=15>; rel="next",
        ///  <https://api.github.com/search/code?q=addClass+user%3Amozilla&page=34>; rel="last",
        ///  <https://api.github.com/search/code?q=addClass+user%3Amozilla&page=1>; rel="first",
        ///  <https://api.github.com/search/code?q=addClass+user%3Amozilla&page=13>; rel="prev"
        /// </returns>
        public virtual string GetLinkHeader<T> ( PagedResult<T> pagedResult, string currentRoute )
        {
            List<string> links = new List<string> ( ) ;

            AddLinks ( links, pagedResult, currentRoute ) ;

            return string.Join ( ",", links.ToArray ( ) ) ;
        }

        /// <summary>
        /// Add pagination links
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="links">A list of strings each representing a value in the Link Header</param>
        /// <param name="pagedResult">The <see cref="PagedResult{T}"/> returned in the response to the client </param>
        /// <param name="currentRoute">The URL for the resource </param>
        /// <remarks>Override this method to add additional link headers.</remarks>
        protected virtual void AddLinks<T> 
        ( 
            List<string> links, 
            PagedResult<T> pagedResult, 
            string currentRoute 
        )
        {
            string nextLink ;
            string lastLink ;
            string firstLink ;
            string prevLink ;

            
            currentRoute = RemovePaginationTokens ( currentRoute ) ;

            if ( Next ( pagedResult, currentRoute, out nextLink ) )
            {
                links.Add (nextLink) ;
            }

            if ( Last ( pagedResult, currentRoute, out lastLink ) )
            {
                links.Add (lastLink) ;
            }

            if ( First ( pagedResult, currentRoute, out firstLink ) )
            {
                links.Add (firstLink) ;
            }

            if ( Previous ( pagedResult, currentRoute, out prevLink ) )
            {
                links.Add (prevLink) ;
            }
        }

        /// <summary>
        /// Returns true if the <paramref name="pagedResult"/> has a previous page and outputs the link.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedResult">The <see cref="PagedResult{T}"/> returned in the response to the client </param>
        /// <param name="currentRoute">The URL for the resource </param>
        /// <param name="prevLink">The URL to the previous page</param>
        /// <returns>true if previous page exists; false, otherwise.</returns>
        protected virtual bool Previous<T> ( PagedResult<T> pagedResult, string currentRoute, out string prevLink )
        {
            prevLink = null ;

            if ( pagedResult.Offset >= pagedResult.PageSize )
            {
                var prevOffset = ((pagedResult.PageNumber -1) - 1) * pagedResult.PageSize;
                prevLink = GetUrl (currentRoute, Labels.Previous, Math.Max (0, pagedResult.Offset - pagedResult.PageSize), pagedResult.PageSize ) ;

                return true ;
            }

            return false ;
        }

        /// <summary>
        /// Returns true if the <paramref name="pagedResult"/> has a first page and outputs the link.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedResult">The <see cref="PagedResult{T}"/> returned in the response to the client </param>
        /// <param name="currentRoute">The URL for the resource </param>
        /// <param name="firstLink">The URL to the first page</param>
        /// <returns>true if first page exists; false, otherwise.</returns>
        protected virtual bool First<T> ( PagedResult<T> pagedResult, string currentRoute, out string firstLink )
        {
            firstLink = null ;

            if ( pagedResult.TotalCount > 0 && pagedResult.PageSize > 0 && pagedResult.Offset != 0 )
            {
                firstLink = GetUrl (currentRoute, Labels.First, 0, pagedResult.PageSize ) ;

                return true ;
            }

            return false ;
        }

        /// <summary>
        /// Returns true if the <paramref name="pagedResult"/> has a last page and outputs the link.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedResult">The <see cref="PagedResult{T}"/> returned in the response to the client </param>
        /// <param name="currentRoute">The URL for the resource </param>
        /// <param name="lastLink">The URL to the last page</param>
        /// <returns>true if last page exists; false, otherwise.</returns>
        protected virtual bool Last<T> ( PagedResult<T> pagedResult, string currentRoute, out string lastLink )
        {
            lastLink = null ;
            
            int lastOffset = (pagedResult.NumberOfPages - 1) * pagedResult.PageSize;

            if ( pagedResult.Offset < lastOffset )
            {
                lastLink = GetUrl (currentRoute, Labels.Last, lastOffset, pagedResult.PageSize ) ;

                return true ;
            }

            return false ;
        }

        /// <summary>
        /// Returns true if the <paramref name="pagedResult"/> has a next page and outputs the link.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedResult">The <see cref="PagedResult{T}"/> returned in the response to the client </param>
        /// <param name="currentRoute">The URL for the resource </param>
        /// <param name="nextLink">The URL to the next page</param>
        /// <returns>true if next page exists; false, otherwise.</returns>
        protected virtual bool Next<T> ( PagedResult<T> pagedResult, string currentRoute, out string nextLink )
        {
            nextLink = null ;

            int nextOffset = ((pagedResult.PageNumber + 1) - 1) * pagedResult.PageSize ;
            //int nextOffset = pagedResult.Offset + pagedResult.PageSize ;

            if ( nextOffset < pagedResult.TotalCount )
            {
                nextLink = GetUrl ( currentRoute, Labels.Next, nextOffset, pagedResult.PageSize );

                return true;
            }

            return false ;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentRoute"></param>
        /// <param name="label"></param>
        /// <param name="nextPage"></param>
        /// <returns></returns>
        protected virtual string GetUrl ( string currentRoute, string label, int offset, int pageLimit )
        {
            string formattedUrl = currentRoute.Contains ( "?") ? URL_FORMATTED.Replace ("?", "&") : URL_FORMATTED ;
            
            return string.Format ( formattedUrl, currentRoute, offset, pageLimit ) +
                                   string.Format ( REL_FORMATTED, label );
        }

        private string RemovePaginationTokens ( string Url )
        {
            
            RemoveToken(ref Url, "limit");
            RemoveToken(ref Url, "offset");

            return Url ;
        }

        private static void RemoveToken(ref string Url, string token )
        {
            int startIndex = Url.IndexOf("?");

            if (-1 != startIndex)
            {
                startIndex = Url.IndexOf(token, startIndex);

                if (-1 != startIndex)
                {
                    int endIndex = Url.IndexOf("&", startIndex);

                    if (-1 != endIndex)
                    {
                        Url = Url.Remove(startIndex, (endIndex - startIndex) + 1);
                    }
                    else
                    {
                        Url = Url.Remove(startIndex, Url.Length - startIndex);
                    }
                }
            }
        }
    }

    public class LinkHeaderLabels
    {
        public LinkHeaderLabels ( ) 
        {
            First    = "first" ;
            Next     = "next" ;
            Previous = "prev" ;
            Last     = "last" ;        
        }

        public string First    { get; set; }
        public string Next     { get; set; }
        public string Previous { get; set; }
        public string Last     { get; set; }
    }
}