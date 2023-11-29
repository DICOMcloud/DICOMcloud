﻿using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom;

namespace DICOMcloud
{
    static class Utilities
    {
        public static bool IsBinaryVR ( DicomVR dicomVr ) 
        {
            return ( dicomVr == DicomVR.OB || dicomVr == DicomVR.OD ||
                     dicomVr == DicomVR.OF || dicomVr == DicomVR.OW ||
                     dicomVr == DicomVR.OL || dicomVr == DicomVR.UN ) ;
        }

        public class PersonNameParts
        {
            public const string PN_Family = "FamilyName" ;
            public const string PN_Given  = "GivenName" ;
            public const string PN_Midlle = "MiddleName" ;
            public const string PN_Prefix = "NamePrefix" ;
            public const string PN_Suffix = "NameSuffix" ;
        }    

        public class PersonNameComponents
        {
            static PersonNameComponents ( ) 
            {
                PN_Components.Add ( PersonNameComponents.PN_COMP_ALPHABETIC  );
                PN_Components.Add ( PersonNameComponents.PN_COMP_IDEOGRAPHIC );
                PN_Components.Add ( PersonNameComponents.PN_COMP_PHONETIC    );
            }

            public static List<string> PN_Components = new List<string> ( ) ;

            public const string PN_COMP_ALPHABETIC  = "Alphabetic" ;
            public const string PN_COMP_IDEOGRAPHIC = "Ideographic" ;
            public const string PN_COMP_PHONETIC    = "Phonetic" ;
        }
    }
}
