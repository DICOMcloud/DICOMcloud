using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DICOMcloud.DataAccess.Database;
using Microsoft.Azure;
using Microsoft.Extensions.Configuration;

namespace DICOMcloud.Wado
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private IConfiguration _configuration;
        public ConnectionStringProvider(IConfiguration configuration)
        {
            this._configuration = configuration ?? throw new ArgumentException(nameof(configuration));
        }
        public string ConnectionString => this._configuration.GetConnectionString("pacsDataArchieve");
    }
}