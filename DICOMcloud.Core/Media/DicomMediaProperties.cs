using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom ;

namespace DICOMcloud.Media
{
    public class DicomMediaProperties
    {
        public DicomMediaProperties ( ) 
        {}

        public DicomMediaProperties ( string mediaType ) : this ( mediaType, null ) {}
        public DicomMediaProperties ( string mediaType, string transferSyntax )
        {
            MediaType      = mediaType ;
            TransferSyntax = transferSyntax ;
        }

        public string MediaType
        {
            get; set;
        }

        public string TransferSyntax
        {
            get; set;
        }

        public override string ToString ( )
        {
            return string.Format ("Media Type:{0}; TransferSyntax{1}", MediaType, TransferSyntax ?? "" ) ;
        }

        // https://stackoverflow.com/questions/25461585/operator-overloading-equals
        public static bool operator ==(DicomMediaProperties obj1, DicomMediaProperties obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            if (ReferenceEquals(obj1, null))
            {
                return false;
            }
            if (ReferenceEquals(obj2, null))
            {
                return false;
            }

            return (obj1.MediaType == obj2.MediaType
                    && obj1.TransferSyntax == obj2.TransferSyntax);
        }

        // this is second one '!='
        public static bool operator !=(DicomMediaProperties obj1, DicomMediaProperties obj2)
        {
            return !(obj1 == obj2);
        }

        public bool Equals(DicomMediaProperties other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return MediaType.Equals(other.MediaType)
                   && TransferSyntax.Equals(other.TransferSyntax);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((DicomMediaProperties)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = MediaType.GetHashCode();
                hashCode = (hashCode * 397) ^ TransferSyntax.GetHashCode();

                return hashCode;
            }
        }
    }
}
