using Dicom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom;


namespace DICOMcloud.UnitTest
{
    public class DicomHelpers
    {
        public DicomHelpers ( ) 
        {
            Study1UID    = "test.study.1" ;
            Study2UID    = "test.study.2" ;
            Study3UID    =  Study2UID ;
            Series1UID   = "test.series.1" ;
            Series2UID   = "test.series.2" ;
            Series3UID   = "test.series.3" ;
            Instance1UID = "test.instance.1" ;
            Instance2UID = "test.instance.2" ;
            Instance3UID = "test.instance.3" ;
        }
        
        public static string GetBaseFolder ( ) 
        {
            string baseFolder = System.AppDomain.CurrentDomain.BaseDirectory ;        

            return new DirectoryInfo ( baseFolder ).Parent.Parent.Parent.Parent.FullName ;
        }

        public static string GetTestDataFolder (string testDataFolder, bool create = false )
        {
            string   folderPath  = Path.Combine (GetBaseFolder ( ), TestFolderName, testDataFolder ) ;

            if ( create )
            {
                Directory.CreateDirectory ( folderPath ) ;
            }

            return folderPath ;
        }

        public static string GetSampleImagesFolder ( ) 
        { 
            return Path.Combine ( DicomHelpers.GetBaseFolder ( ), "resources", "sampleimages" ) ;
        }

        public static string   TestFolderName = "Test_Data" ;

        public fo.DicomDataset GetDicomDataset ( uint dsNumber) 
        {
            uint testDsCase = dsNumber % 3 ;
            fo.DicomDataset testDs = new fo.DicomDataset ( ) ;


            switch ( testDsCase )
            {
                case 0:
                {
                    return GetTemplateDataset ( ).CopyTo ( testDs ) ;
                }

                case 1:
                {

                    testDs = GetTemplateDataset().CopyTo ( testDs ) ;

                    testDs .AddOrUpdate ( fo.DicomTag.StudyInstanceUID, Study2UID );
                    testDs .AddOrUpdate ( fo.DicomTag.SeriesInstanceUID, Series2UID );
                    testDs .AddOrUpdate ( fo.DicomTag.SOPInstanceUID, Instance2UID );
                }
                break ;

                case 2:
                {
                    testDs = GetTemplateDataset().CopyTo ( testDs ) ;

                    testDs.AddOrUpdate ( fo.DicomTag.StudyInstanceUID, Study3UID );
                    testDs.AddOrUpdate ( fo.DicomTag.SeriesInstanceUID, Series3UID );
                    testDs.AddOrUpdate  ( fo.DicomTag.SOPInstanceUID, Instance3UID ) ;
                }
                break ;

                default:
                {
                    throw new IndexOutOfRangeException ( "Test dataset case not implemented") ;
                }
            }

            return testDs ;
        }
        
        public string Study1UID
        {
            get; set;
        }

        public string Study2UID
        {
            get; set;
        }

        public string Study3UID
        {
            get; set;
        }

        public string Series1UID
        {
            get; set;
        }

        public string Series2UID
        {
            get; set;
        }

        public string Series3UID
        {
            get; set;
        }

        public string Instance1UID
        {
            get; set;
        }

        public string Instance2UID
        {
            get; set;
        }

        public string Instance3UID
        {
            get; set;
        }


        public fo.DicomDataset GetQueryDataset ( ) 
        {
            var ds = new fo.DicomDataset ( ) ;


            ds.Add<object> ( fo.DicomTag.PatientID,(object) null) ;
            ds.Add<object>( fo.DicomTag.PatientName, (object)null);
            ds.Add<object>( fo.DicomTag.StudyInstanceUID, (object)null);
            ds.Add<object>( fo.DicomTag.StudyID, (object)null);
            ds.Add<object>(fo.DicomTag.StudyDate, (object)null);
            ds.Add<object>( fo.DicomTag.AccessionNumber, (object)null);
            ds.Add<object>(fo.DicomTag.StudyDescription, (object)null);
            ds.Add<object>( fo.DicomTag.SeriesInstanceUID, (object)null);
            ds.Add<object>( fo.DicomTag.SeriesNumber, (object)null);
            ds.Add<object>( fo.DicomTag.Modality, (object)null);
            ds.Add<object>( fo.DicomTag.SOPInstanceUID, (object)null);
            ds.Add<object>( fo.DicomTag.SOPClassUID, (object)null);
            ds.Add<object>( fo.DicomTag.InstanceNumber, (object)null);

            ds.Add<object>(fo.DicomTag.NumberOfFrames, (object)null);
            ds.Add<object>(fo.DicomTag.BitsAllocated,(object) null);
            ds.Add<object>(fo.DicomTag.Rows,(object) null);
            ds.Add<object>(fo.DicomTag.Columns,(object) null);

            return ds ;
        }

        private fo.DicomDataset GetTemplateDataset ( ) 
        {
            var ds = new fo.DicomDataset ( ) ;


            ds.Add ( fo.DicomTag.PatientID, "test-pid") ;
            ds.Add ( fo.DicomTag.PatientName, "test^patient name" );
            ds.Add ( fo.DicomTag.StudyInstanceUID, Study1UID );
            ds.Add ( fo.DicomTag.StudyID, "test-studyid" );
            ds.Add (fo.DicomTag.StudyDate, "20181112");
            ds.Add ( fo.DicomTag.AccessionNumber, "test-accession" );
            ds.Add (fo.DicomTag.StudyDescription, "test-description");
            ds.Add ( fo.DicomTag.SeriesInstanceUID, Series1UID );
            ds.Add ( fo.DicomTag.SeriesNumber, 1 );
            ds.Add ( fo.DicomTag.Modality, "XA" );
            ds.Add ( fo.DicomTag.SOPInstanceUID, Instance1UID );
            ds.Add ( fo.DicomTag.SOPClassUID, "test.instance.class.uid" );
            ds.Add ( fo.DicomTag.InstanceNumber, 1 );

            ds.Add(fo.DicomTag.NumberOfFrames, 1);
            ds.Add(fo.DicomTag.BitsAllocated, (ushort)16);
            ds.Add(fo.DicomTag.Rows, (ushort)255);
            ds.Add(fo.DicomTag.Columns, (ushort)512);

            return ds ;
        }

    }
}
