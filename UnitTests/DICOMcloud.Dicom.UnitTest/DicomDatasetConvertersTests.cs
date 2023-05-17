using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dicom;
using Dicom.Imaging.Codec;
using System.IO;

namespace DICOMcloud.UnitTest
{
    [TestClass]
    public class DicomDatasetConvertersTests
    {
        private DicomHelpers Helper { get; set; } 

        [TestInitialize]
        public void Initialize ( ) 
        {
            _testFolderName = "conversions" ;

            TestDirPath = DicomHelpers.GetTestDataFolder (_testFolderName, true) ;
            Helper      = new DicomHelpers ( ) ;
        }

        [TestCleanup]
        public void Cleanup ( )
        {
            var directory = new DirectoryInfo ( TestDirPath );


            if ( directory.Exists )
            {
                try
                {
                    //directory.Delete ( true ) ;
                }
                catch ( Exception ex )
                {
                    System.Diagnostics.Debug.Assert (false,ex.Message) ;
                }
            }
        }

        /// <summary>
        /// Method will:
        /// 1. Load a DICOM Dataset: sourceDS
        /// 2. Convert to XML: sourceXMLDicom
        /// 3. Write to desk 
        /// 4. Convert sourceXmlDicom to DicomDataset: targetDs
        /// 5. Save targetDs to desk
        /// 6. Convert targetDs to XML: destXmlDicom
        /// 7. write to desk
        /// 8. Assert sourceXml == destXmlDicom
        /// </summary>
        [TestMethod]
        public void ConvertToXml ( )
        {
            var testDir = Path.Combine ( TestDirPath, "convertToXml" ) ;
            var xmlConverter = new XmlDicomConverter ( ) { WriteInlineBinary = true };
            

            Directory.CreateDirectory ( testDir ) ;
            //DicomDataset sourceDS = Helper.GetDicomDataset ( 10 ).Clone ( DicomTransferSyntax.ExplicitVRLittleEndian ) ;
            foreach ( string file in Directory.GetFiles (DicomHelpers.GetSampleImagesFolder ( ) ) )
            {
                string          fullPath = Path.Combine ( testDir, Path.GetFileName ( file ) ) ; 
                DicomDataset sourceDS = DicomFile.Open ( file ).Dataset ;
                

                var sourceXmlDicom = xmlConverter.Convert  (sourceDS) ;
            
                System.IO.File.WriteAllText ( fullPath + ".xml", sourceXmlDicom ) ;

                DicomDataset targetDs = xmlConverter.Convert ( sourceXmlDicom ) ;
            
                var dsF = new DicomFile ( targetDs ) ;
            
                dsF.FileMetaInfo.TransferSyntax = DicomTransferSyntax.Parse( targetDs.GetSingleValueOrDefault ( DicomTag.TransferSyntaxUID, targetDs.InternalTransferSyntax.ToString ( ) ) ) ;
                
                dsF.Save ( fullPath + ".gen.dcm" ) ;

                var destXmlDicom = xmlConverter.Convert ( targetDs ) ;

                System.IO.File.WriteAllText (fullPath + ".gen.xml", destXmlDicom);

                //private tags with private creator will cause this to fail
                //VR for OW change to OB
                Assert.AreEqual ( sourceXmlDicom, destXmlDicom ) ;
            }
        }

        [TestMethod]
        public void ConvertToJson()
        {
            var testDir = Path.Combine ( TestDirPath, "convertToJson" ) ;
            JsonDicomConverter jsonConverter = new JsonDicomConverter ( ) ;

            Directory.CreateDirectory ( testDir ) ;

            foreach ( string file in Directory.GetFiles (DicomHelpers.GetSampleImagesFolder ( )) )
            {
                string          fullPath = Path.Combine ( testDir, Path.GetFileName ( file ) ) ; 
                DicomDataset sourceDS = DicomFile.Open ( file ).Dataset ;
           
                jsonConverter.WriteInlineBinary = true ;

                string sourceJsonDicom = jsonConverter.Convert (sourceDS) ;

                System.IO.File.WriteAllText (fullPath + ".jsn", sourceJsonDicom);


                DicomDataset targetDs = jsonConverter.Convert ( sourceJsonDicom ) ;
            
                var dsF = new DicomFile ( targetDs ) ;

                dsF.Save ( fullPath + ".jsn.dcm" ) ;

                string destJsonDicom = jsonConverter.Convert ( targetDs ) ;

                System.IO.File.WriteAllText (fullPath + ".gen.jsn", destJsonDicom);
                
                Assert.AreEqual ( sourceJsonDicom, destJsonDicom ) ;
            }
        }

        public string TestDirPath  { get; set; }
        private string _testFolderName ;
    }
}
