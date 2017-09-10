using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud
{
    public class DuplicateInstanceException : DICOMcloudException
    {
        public DuplicateInstanceException ( )
        {}

        public DuplicateInstanceException ( string message )
        : base (message)
        { }
    }
}
