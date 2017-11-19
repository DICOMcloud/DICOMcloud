using DICOMcloud;
using DICOMcloud.DataAccess.Database.Commands;
using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.DataAccess.Matching;
using DICOMcloud.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using fo = Dicom;

namespace DICOMcloud.DataAccess.Database
{
    public partial class ObjectArchieveDataAdapter
    {
        public static class SqlConstants
        {
            public static string MinDate = "1753/1/1" ;
            public static string MaxDate = "9999/12/31" ;
            public static string MinTime = "00:00:00" ;
            public static string MaxTime = "23:59:59" ;

            public static string MaxDateTime = "9999/12/31 11:59:59"   ;
            public static string MinDateTime = "1753/1/1 00:00:00" ;
        }
    }
}
