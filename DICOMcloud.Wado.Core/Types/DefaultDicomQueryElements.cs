using Dicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado
{
    public class DefaultDicomQueryElements
    {
        static DicomDataset _studyDs = new DicomDataset ( ).NotValidated();
        static DicomDataset _seriesDs = new DicomDataset ( ).NotValidated();
        static DicomDataset _instanceDs = new DicomDataset ( ).NotValidated();

        public virtual DicomDataset GetStudyQuery ( ) 
        {
            return GetDefaultStudyQuery ( ) ;
        }

        public virtual DicomDataset GetSeriesQuery ( ) 
        {
            return GetDefaultSeriesQuery ( ) ;
        }

        public virtual DicomDataset GetInstanceQuery ( ) 
        {
            return GetDefaultInstanceQuery ( ) ;
        }


        public static DicomDataset GetDefaultStudyQuery ( ) 
        {
            DicomDataset ds = new DicomDataset ( ).NotValidated();
            
            _studyDs.CopyTo ( ds ) ; 

            return ds ;
        }

        public static DicomDataset GetDefaultSeriesQuery ( ) 
        {
            DicomDataset ds = new DicomDataset ( ).NotValidated();
            
            _seriesDs.CopyTo ( ds ) ;

            return ds ;
        }

        public static DicomDataset GetDefaultInstanceQuery ( ) 
        {
            DicomDataset ds = new DicomDataset ( ).NotValidated();

            _instanceDs.CopyTo ( ds ) ;

            return ds ;
        }

        static DefaultDicomQueryElements ( ) 
        {
            
            FillStudyLevel     ( _studyDs ) ;
            FillSeriesLevel    ( _seriesDs ) ;
            FillInstsanceLevel ( _instanceDs ) ;

        }

        private static void FillStudyLevel(DicomDataset studyDs)
        {
            studyDs.Add<string>(DicomTag.SpecificCharacterSet) ;
            studyDs.Add<DateTime>(DicomTag.StudyDate) ;
            studyDs.Add<DateTime>(DicomTag.StudyTime) ;
            studyDs.Add<string>(DicomTag.StudyDescription) ;
            studyDs.Add<string>(DicomTag.AccessionNumber) ;
            studyDs.Add<string>(DicomTag.InstanceAvailability) ;
            studyDs.Add<string>(DicomTag.ModalitiesInStudy) ;
            studyDs.Add<string>(DicomTag.ReferringPhysicianName) ;
            studyDs.Add<string>(DicomTag.TimezoneOffsetFromUTC) ;
            studyDs.Add<string>(DicomTag.RetrieveURI, string.Empty) ;
            studyDs.Add<string>(DicomTag.PatientName) ;
            studyDs.Add<string>(DicomTag.PatientID) ;
            studyDs.Add<DateTime>(DicomTag.PatientBirthDate) ;
            studyDs.Add<string>(DicomTag.PatientSex) ;
            studyDs.Add<string>(DicomTag.StudyInstanceUID) ;
            studyDs.Add<string>(DicomTag.StudyID) ;
            studyDs.Add<string>(DicomTag.NumberOfStudyRelatedSeries) ;
            studyDs.Add<string>(DicomTag.NumberOfStudyRelatedInstances) ;
        }

        private static void FillSeriesLevel(DicomDataset seriesDs)
        {
            seriesDs.Add<string>(DicomTag.SpecificCharacterSet) ;
            seriesDs.Add<string>(DicomTag.Modality) ;
            seriesDs.Add<string>(DicomTag.TimezoneOffsetFromUTC) ;
            seriesDs.Add<string>(DicomTag.SeriesDescription) ;
            seriesDs.Add<string>(DicomTag.SeriesInstanceUID) ;
            seriesDs.Add<string>(DicomTag.SeriesNumber) ;
            seriesDs.Add<string>(DicomTag.NumberOfSeriesRelatedInstances) ;
            seriesDs.Add<string>(DicomTag.PerformedProcedureStepStartDate) ;
            seriesDs.Add<string>(DicomTag.PerformedProcedureStepStartTime) ;
            //seriesDs.Add<object>(DicomTag.RequestAttributesSequence,null) ; //Not supported yet
        }

        private static void FillInstsanceLevel(DicomDataset instanceDs)
        {
            instanceDs.Add<string>(DicomTag.SpecificCharacterSet) ;
            instanceDs.Add<string>(DicomTag.SOPClassUID) ;
            instanceDs.Add<string>(DicomTag.SOPInstanceUID) ;
            instanceDs.Add<string>(DicomTag.InstanceNumber) ;

            instanceDs.Add<ushort>(DicomTag.Rows);
            instanceDs.Add<ushort>(DicomTag.Columns);
            instanceDs.Add<ushort>(DicomTag.BitsAllocated);
            instanceDs.Add<string>(DicomTag.NumberOfFrames);
        }
    }
}
