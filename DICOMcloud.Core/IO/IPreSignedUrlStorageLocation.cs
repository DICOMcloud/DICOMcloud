﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public interface IPreSignedUrlStorageLocation : IStorageLocation
    {
        Uri GetReadUrl  ( DateTimeOffset expiryTime );
        Uri GetWriteUrl ( DateTimeOffset expiryTime );
    }


}
