using Dicom.Network;
using DICOMcloud.DataAccess;
using DICOMcloud.DataAccess.Database;
using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.IO;
using DICOMcloud.Media;
using DICOMcloud.Pacs;
using DICOMcloud.Wado.PacsInterface.PacsService.SCPReleated;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado.PacsInterface.PacsService
{
    public class PacsServer
    {
        private static IDicomServer _server;

        public static string AETitle { get; private set; }

        public static IObjectStoreService StorageService { get; private set; }

        public static IObjectArchieveQueryService QueryService { get; private set; }

        public static IObjectRetrieveService RetrieveService { get; private set; }

        public static IDatabaseFactory DatabaseService { get; private set; }

        public static void Start(int port, string aet)
        {
            AETitle = aet;
            string storageConection = ConfigurationManager.AppSettings["app:PacsStorageConnection"];
            if (storageConection.StartsWith("|datadirectory|", StringComparison.OrdinalIgnoreCase))
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var lastIndex = storageConection.IndexOf('|', 1);
                var userPathPart = storageConection.Substring(lastIndex + 1);
                storageConection = appDataPath + userPathPart;
            }
            IDicomMediaIdFactory mediaIdFactory = new DicomMediaIdFactory();
            DbSchemaProvider schemaProvider = new StorageDbSchemaProvider();

            DatabaseService = new SqlDatabaseFactory(ConfigurationManager.AppSettings["app:PacsDataArchieve"]);

            IObjectArchieveDataAccess dataAccess = 
            new ObjectArchieveDataAccess(
                schemaProvider, 
                new ObjectArchieveDataAdapter(
                    schemaProvider,
                    DatabaseService
                )
            );

            IMediaStorageService storageService = new FileStorageService(storageConection);

            IDicomMediaWriterFactory dicomMediaWriterFactory =  
            new DicomMediaWriterFactory(
                storageService, 
                mediaIdFactory
            );

            StorageService = new ObjectStoreService(
                new Pacs.Commands.DCloudCommandFactory(
                    storageService, 
                    dataAccess, 
                    dicomMediaWriterFactory, 
                    mediaIdFactory
                )
            );

            QueryService = new ObjectArchieveQueryService(dataAccess);

            RetrieveService = new ObjectRetrieveService(
                storageService, 
                dicomMediaWriterFactory, 
                mediaIdFactory
            );

            _server = DicomServer.Create<SCP>(port);
        }

        public static void Stop()
        {
            _server.Dispose();
        }
    }
}
