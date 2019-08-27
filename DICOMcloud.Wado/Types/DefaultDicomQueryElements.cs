using fo = Dicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado
{
    public class DefaultDicomQueryElements
    {
        static fo.DicomDataset _studyDs = new fo.DicomDataset ( ) ;
        static fo.DicomDataset _seriesDs = new fo.DicomDataset ( ) ;
        static fo.DicomDataset _instanceDs = new fo.DicomDataset ( ) ;
            
    
        public virtual fo.DicomDataset GetStudyQuery ( ) 
        {
            return GetDefaultStudyQuery ( ) ;
        }

        public virtual fo.DicomDataset GetSeriesQuery ( ) 
        {
            return GetDefaultSeriesQuery ( ) ;
        }

        public virtual fo.DicomDataset GetInstanceQuery ( ) 
        {
            return GetDefaultInstanceQuery ( ) ;
        }


        public static fo.DicomDataset GetDefaultStudyQuery ( ) 
        {
            fo.DicomDataset ds = new fo.DicomDataset ( ) ;
            _studyDs.CopyTo ( ds ) ; 

            return ds ;
        }

        public static fo.DicomDataset GetDefaultSeriesQuery ( ) 
        {
            fo.DicomDataset ds = new fo.DicomDataset ( ) ;
            
            _seriesDs.CopyTo ( ds ) ;

            return ds ;
        }

        public static fo.DicomDataset GetDefaultInstanceQuery ( ) 
        {
            fo.DicomDataset ds = new fo.DicomDataset ( ) ;
            
            _instanceDs.CopyTo ( ds ) ;

            return ds ;
        }

        static DefaultDicomQueryElements ( ) 
        {
            
            FillStudyLevel     ( _studyDs ) ;
            FillSeriesLevel    ( _seriesDs ) ;
            FillInstsanceLevel ( _instanceDs ) ;

        }

        private static void FillStudyLevel(fo.DicomDataset studyDs)
        {
            studyDs.Add<string>(fo.DicomTag.SpecificCharacterSet,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.StudyDate,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.StudyTime,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.StudyDescription,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.AccessionNumber,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.InstanceAvailability,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.ModalitiesInStudy,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.ReferringPhysicianName,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.TimezoneOffsetFromUTC,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.RetrieveURI,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.PatientName,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.PatientID,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.PatientBirthDate,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.PatientSex,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.StudyInstanceUID,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.StudyID,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.NumberOfStudyRelatedSeries,(string)null) ;
            studyDs.Add<string>(fo.DicomTag.NumberOfStudyRelatedInstances,(string)null) ;
        }

        private static void FillSeriesLevel(fo.DicomDataset seriesDs)
        {
            seriesDs.Add<string>(fo.DicomTag.SpecificCharacterSet,(string)null) ;
            seriesDs.Add<string>(fo.DicomTag.Modality,(string)null) ;
            seriesDs.Add<string>(fo.DicomTag.TimezoneOffsetFromUTC,(string)null) ;
            seriesDs.Add<string>(fo.DicomTag.SeriesDescription,(string)null) ;
            seriesDs.Add<string>(fo.DicomTag.RetrieveURI,(string)null) ;
            seriesDs.Add<string>(fo.DicomTag.SeriesInstanceUID,(string)null) ;
            seriesDs.Add<string>(fo.DicomTag.SeriesNumber,(string)null) ;
            seriesDs.Add<string>(fo.DicomTag.NumberOfSeriesRelatedInstances,(string)null) ;
            seriesDs.Add<string>(fo.DicomTag.PerformedProcedureStepStartDate,(string)null) ;
            seriesDs.Add<string>(fo.DicomTag.PerformedProcedureStepStartTime,(string)null) ;
            //seriesDs.Add<object>(fo.DicomTag.RequestAttributesSequence,null) ; //Not supported yet


        }

        private static void FillInstsanceLevel(fo.DicomDataset instanceDs)
        {
            instanceDs.Add<string>(fo.DicomTag.SpecificCharacterSet,(string)null) ;
            instanceDs.Add<string>(fo.DicomTag.SOPClassUID,(string)null) ;
            instanceDs.Add<string>(fo.DicomTag.SOPInstanceUID,(string)null) ;
            instanceDs.Add<string>(fo.DicomTag.InstanceNumber,(string)null) ;

            instanceDs.Add<string>(fo.DicomTag.Rows, (string)null);
            instanceDs.Add<string>(fo.DicomTag.Columns, (string)null);
            instanceDs.Add<string>(fo.DicomTag.BitsAllocated, (string)null);
            instanceDs.Add<string>(fo.DicomTag.NumberOfFrames, (string)null);
        }
    }
}
