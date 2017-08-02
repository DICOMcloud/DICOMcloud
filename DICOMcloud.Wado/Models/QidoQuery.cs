using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado.Models
{
    public class QidoQuery : IQidoQuery
    {
        public QidoQuery ( )
        {
            MatchingElements = new Dictionary<string, string> ( ) ;
            IncludeElements = new List<string> ( ) ;
        }

        public Dictionary<string,string> MatchingElements { get; private set ;}

        public List<string> IncludeElements {get; private set ;}
    }
}
