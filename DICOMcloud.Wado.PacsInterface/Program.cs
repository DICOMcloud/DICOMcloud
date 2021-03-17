using System;
using Dicom.Log;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DICOMcloud.Wado.PacsInterface.PacsService;

namespace DICOMcloud.Wado.PacsInterface
{
    class Program
    {
        static void Main(string[] args)
        {

            LogManager.SetImplementation(ConsoleLogManager.Instance);

            int tmp;
            var port = args != null && args.Length > 0 && int.TryParse(args[0], out tmp) ? tmp : 8001;

            Console.WriteLine($"Starting QR SCP server with AET: QRSCP on port {port}");

            PacsService.PacsServer.Start(port, "QRSCP");

            Console.WriteLine("Press any key to stop the service");

            Console.Read();

            Console.WriteLine("Stopping QR service");

            PacsService.PacsServer.Stop();
        }
    }
}
