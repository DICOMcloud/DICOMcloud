using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DICOMcloud.DataAccess.Database;
using Microsoft.Azure;

namespace DICOMcloud.Wado
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        //public ConnectionStringProvider(IConfigurationBuilder configuration)
        public string ConnectionString => configuration.GetSetting   ( "app:PacsDataArchieve" ) ;
    }
}