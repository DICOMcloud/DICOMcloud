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
            studyDs.Add<string>(fo.DicomTag.SpecificCharacterSet) ;
            studyDs.Add<DateTime>(fo.DicomTag.StudyDate) ;
            studyDs.Add<DateTime>(fo.DicomTag.StudyTime) ;
            studyDs.Add<string>(fo.DicomTag.StudyDescription) ;
            studyDs.Add<string>(fo.DicomTag.AccessionNumber) ;
            studyDs.Add<string>(fo.DicomTag.InstanceAvailability) ;
            studyDs.Add<string>(fo.DicomTag.ModalitiesInStudy) ;
            studyDs.Add<string>(fo.DicomTag.ReferringPhysicianName) ;
            studyDs.Add<string>(fo.DicomTag.TimezoneOffsetFromUTC) ;
            studyDs.Add<string>(fo.DicomTag.RetrieveURI, string.Empty) ;
            studyDs.Add<string>(fo.DicomTag.PatientName) ;
            studyDs.Add<string>(fo.DicomTag.PatientID) ;
            studyDs.Add<DateTime>(fo.DicomTag.PatientBirthDate) ;
            studyDs.Add<string>(fo.DicomTag.PatientSex) ;
            studyDs.Add<string>(fo.DicomTag.StudyInstanceUID) ;
            studyDs.Add<string>(fo.DicomTag.StudyID) ;
            studyDs.Add<string>(fo.DicomTag.NumberOfStudyRelatedSeries) ;
            studyDs.Add<string>(fo.DicomTag.NumberOfStudyRelatedInstances) ;
        }

        private static void FillSeriesLevel(fo.DicomDataset seriesDs)
        {
            seriesDs.Add<string>(fo.DicomTag.SpecificCharacterSet) ;
            seriesDs.Add<string>(fo.DicomTag.Modality) ;
            seriesDs.Add<string>(fo.DicomTag.TimezoneOffsetFromUTC) ;
            seriesDs.Add<string>(fo.DicomTag.SeriesDescription) ;
            seriesDs.Add<string>(fo.DicomTag.SeriesInstanceUID) ;
            seriesDs.Add<string>(fo.DicomTag.SeriesNumber) ;
            seriesDs.Add<string>(fo.DicomTag.NumberOfSeriesRelatedInstances) ;
            seriesDs.Add<string>(fo.DicomTag.PerformedProcedureStepStartDate) ;
            seriesDs.Add<string>(fo.DicomTag.PerformedProcedureStepStartTime) ;
            //seriesDs.Add<object>(fo.DicomTag.RequestAttributesSequence,null) ; //Not supported yet
        }

        private static void FillInstsanceLevel(fo.DicomDataset instanceDs)
        {
            instanceDs.Add<string>(fo.DicomTag.SpecificCharacterSet) ;
            instanceDs.Add<string>(fo.DicomTag.SOPClassUID) ;
            instanceDs.Add<string>(fo.DicomTag.SOPInstanceUID) ;
            instanceDs.Add<string>(fo.DicomTag.InstanceNumber) ;

            instanceDs.Add<ushort>(fo.DicomTag.Rows);
            instanceDs.Add<ushort>(fo.DicomTag.Columns);
            instanceDs.Add<ushort>(fo.DicomTag.BitsAllocated);
            instanceDs.Add<string>(fo.DicomTag.NumberOfFrames);
        }
    }
}
