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

        public string ConnectionString
        {
            get
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");

                return _configuration.GetValue<string>("app:PacsDataArchieve").Replace("|DataDirectory|", path);
            }
        }

    }
}