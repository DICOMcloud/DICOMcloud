using Dicom;
using Dicom.Imaging;
using DICOMcloud;
using DICOMcloud.IO;
using DICOMcloud.Media;

namespace DICOMcloud.Core.Test
{
    [TestClass]
    public class DatasetConvertertsTests
    {
        private DicomHelpers Helper { get; set; }
        public string TestDirPath { get; set; }
        private string _testFolderName;

        [TestInitialize]
        public void Initialize()
        {
            _testFolderName = "conversions";

            TestDirPath = DicomHelpers.GetTestDataFolder(_testFolderName, true);
            Helper = new DicomHelpers();
        }


        public void TestMethod1()
        {
            JsonDicomConverter jsonConverter = new JsonDicomConverter();

            //jsonConverter.Convert()
        }

        [TestMethod]
        public void ConvertToXML()
        {
            var testDir = Path.Combine(TestDirPath, "convertToXml");
            var xmlConverter = new XmlDicomConverter() { WriteInlineBinary = true };


            Directory.CreateDirectory(testDir);
            //DicomDataset sourceDS = Helper.GetDicomDataset ( 10 ).Clone ( DicomTransferSyntax.ExplicitVRLittleEndian ) ;
            foreach (string file in Directory.GetFiles(DicomHelpers.GetSampleImagesFolder()))
            {
                string fullPath = Path.Combine(testDir, Path.GetFileName(file));
                DicomDataset sourceDS = DicomFile.Open(file).Dataset;


                var sourceXmlDicom = xmlConverter.Convert(sourceDS);

                System.IO.File.WriteAllText(fullPath + ".xml", sourceXmlDicom);

                DicomDataset targetDs = xmlConverter.Convert(sourceXmlDicom);

                var dsF = new DicomFile(targetDs);

                dsF.FileMetaInfo.TransferSyntax = DicomTransferSyntax.Parse(targetDs.GetSingleValueOrDefault(DicomTag.TransferSyntaxUID, targetDs.InternalTransferSyntax.ToString()));

                dsF.Save(fullPath + ".gen.dcm");

                var destXmlDicom = xmlConverter.Convert(targetDs);

                System.IO.File.WriteAllText(fullPath + ".gen.xml", destXmlDicom);

                //private tags with private creator will cause this to fail
                //VR for OW change to OB
                Assert.AreEqual(sourceXmlDicom, destXmlDicom);
            }
        }

        [TestMethod]
        public void ConvertToJson()
        {
            var testDir = Path.Combine(TestDirPath, "convertToJson");
            JsonDicomConverter jsonConverter = new JsonDicomConverter();

            Directory.CreateDirectory(testDir);

            foreach (string file in Directory.GetFiles(DicomHelpers.GetSampleImagesFolder()))
            {
                string fullPath = Path.Combine(testDir, Path.GetFileName(file));
                DicomDataset sourceDS = DicomFile.Open(file).Dataset;

                jsonConverter.WriteInlineBinary = true;

                string sourceJsonDicom = jsonConverter.Convert(sourceDS);

                System.IO.File.WriteAllText(fullPath + ".jsn", sourceJsonDicom);


                DicomDataset targetDs = jsonConverter.Convert(sourceJsonDicom);

                var dsF = new DicomFile(targetDs);

                dsF.Save(fullPath + ".jsn.dcm");

                string destJsonDicom = jsonConverter.Convert(targetDs);

                System.IO.File.WriteAllText(fullPath + ".gen.jsn", destJsonDicom);

                Assert.AreEqual(sourceJsonDicom, destJsonDicom);
            }
        }

        [TestMethod]
        public void ConvertToJpeg()
        {
            ImageManager.SetImplementation(new ImageSharpImageManager());
            var testDir = Path.Combine(TestDirPath, "convertToJPG");
            var storageService = new FileStorageService(testDir);
            var jpgWriter = new JpegMediaWriter(storageService, new DicomMediaIdFactory());

            Directory.CreateDirectory(testDir);

            foreach (string file in Directory.GetFiles(DicomHelpers.GetSampleImagesFolder()))
            {
                string fullPath = Path.Combine(testDir, Path.GetFileName(file));
                DicomDataset sourceDS = DicomFile.Open(file).Dataset;
                var mediaProp = new DicomMediaProperties(MimeMediaTypes.Jpeg);
                jpgWriter.CreateMedia(new DicomMediaWriterParameters(){ Dataset = sourceDS, MediaInfo = mediaProp });
            }
        }
    }
}