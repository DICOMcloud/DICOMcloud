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
            studyDs.Add<object>(fo.DicomTag.SpecificCharacterSet,null) ;
            studyDs.Add<object>(fo.DicomTag.StudyDate,null) ;
            studyDs.Add<object>(fo.DicomTag.StudyTime,null) ;
            studyDs.Add<object>(fo.DicomTag.StudyDescription,null) ;
            studyDs.Add<object>(fo.DicomTag.AccessionNumber,null) ;
            studyDs.Add<object>(fo.DicomTag.InstanceAvailability,null) ;
            studyDs.Add<object>(fo.DicomTag.ModalitiesInStudy,null) ;
            studyDs.Add<object>(fo.DicomTag.ReferringPhysicianName,null) ;
            studyDs.Add<object>(fo.DicomTag.TimezoneOffsetFromUTC,null) ;
            studyDs.Add<object>(fo.DicomTag.RetrieveURI,null) ;
            studyDs.Add<object>(fo.DicomTag.PatientName,null) ;
            studyDs.Add<object>(fo.DicomTag.PatientID,null) ;
            studyDs.Add<object>(fo.DicomTag.PatientBirthDate,null) ;
            studyDs.Add<object>(fo.DicomTag.PatientSex,null) ;
            studyDs.Add<object>(fo.DicomTag.StudyInstanceUID,null) ;
            studyDs.Add<object>(fo.DicomTag.StudyID,null) ;
            studyDs.Add<object>(fo.DicomTag.NumberOfStudyRelatedSeries,null) ;
            studyDs.Add<object>(fo.DicomTag.NumberOfStudyRelatedInstances,null) ;
        }

        private static void FillSeriesLevel(fo.DicomDataset seriesDs)
        {
            seriesDs.Add<object>(fo.DicomTag.SpecificCharacterSet,null) ;
            seriesDs.Add<object>(fo.DicomTag.Modality,null) ;
            seriesDs.Add<object>(fo.DicomTag.TimezoneOffsetFromUTC,null) ;
            seriesDs.Add<object>(fo.DicomTag.SeriesDescription,null) ;
            seriesDs.Add<object>(fo.DicomTag.RetrieveURI,null) ;
            seriesDs.Add<object>(fo.DicomTag.SeriesInstanceUID,null) ;
            seriesDs.Add<object>(fo.DicomTag.SeriesNumber,null) ;
            seriesDs.Add<object>(fo.DicomTag.NumberOfSeriesRelatedInstances,null) ;
            seriesDs.Add<object>(fo.DicomTag.PerformedProcedureStepStartDate,null) ;
            seriesDs.Add<object>(fo.DicomTag.PerformedProcedureStepStartTime,null) ;
            //seriesDs.Add<object>(fo.DicomTag.RequestAttributesSequence,null) ; //Not supported yet


        }

        private static void FillInstsanceLevel(fo.DicomDataset instanceDs)
        {
            instanceDs.Add<object>(fo.DicomTag.SpecificCharacterSet,null) ;
            instanceDs.Add<object>(fo.DicomTag.SOPClassUID,null) ;
            instanceDs.Add<object>(fo.DicomTag.SOPInstanceUID,null) ;
            instanceDs.Add<object>(fo.DicomTag.InstanceNumber,null) ;

            instanceDs.Add<object>(fo.DicomTag.Rows, null);
            instanceDs.Add<object>(fo.DicomTag.Columns, null);
            instanceDs.Add<object>(fo.DicomTag.BitsAllocated, null);
            instanceDs.Add<object>(fo.DicomTag.NumberOfFrames, null);
        }
    }
}
