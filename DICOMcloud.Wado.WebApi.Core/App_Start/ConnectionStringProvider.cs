using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using DICOMcloud.DataAccess.Database;
using Microsoft.Azure;
using SixLabors.ImageSharp;

namespace DICOMcloud.Wado
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        //public ConnectionStringProvider(IConfigurationBuilder configuration)
         public string ConnectionString =>  _configuration.GetConnectionString("DefaultConnection");
        //public string ConnectionString => configuration.GetSetting   ( "app:PacsDataArchieve" ) ;
    }
}