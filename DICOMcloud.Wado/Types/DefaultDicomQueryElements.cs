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
            studyDs.Add<object>(fo.DicomTag.SpecificCharacterSet,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.StudyDate,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.StudyTime,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.StudyDescription,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.AccessionNumber,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.InstanceAvailability,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.ModalitiesInStudy,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.ReferringPhysicianName,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.TimezoneOffsetFromUTC,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.RetrieveURI,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.PatientName,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.PatientID,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.PatientBirthDate,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.PatientSex,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.StudyInstanceUID,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.StudyID,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.NumberOfStudyRelatedSeries,(object)null) ;
            studyDs.Add<object>(fo.DicomTag.NumberOfStudyRelatedInstances,(object)null) ;
        }

        private static void FillSeriesLevel(fo.DicomDataset seriesDs)
        {
            seriesDs.Add<object>(fo.DicomTag.SpecificCharacterSet,(object)null) ;
            seriesDs.Add<object>(fo.DicomTag.Modality,(object)null) ;
            seriesDs.Add<object>(fo.DicomTag.TimezoneOffsetFromUTC,(object)null) ;
            seriesDs.Add<object>(fo.DicomTag.SeriesDescription,(object)null) ;
            seriesDs.Add<object>(fo.DicomTag.RetrieveURI,(object)null) ;
            seriesDs.Add<object>(fo.DicomTag.SeriesInstanceUID,(object)null) ;
            seriesDs.Add<object>(fo.DicomTag.SeriesNumber,(object)null) ;
            seriesDs.Add<object>(fo.DicomTag.NumberOfSeriesRelatedInstances,(object)null) ;
            seriesDs.Add<object>(fo.DicomTag.PerformedProcedureStepStartDate,(object)null) ;
            seriesDs.Add<object>(fo.DicomTag.PerformedProcedureStepStartTime,(object)null) ;
            //seriesDs.Add<object>(fo.DicomTag.RequestAttributesSequence,null) ; //Not supported yet


        }

        private static void FillInstsanceLevel(fo.DicomDataset instanceDs)
        {
            instanceDs.Add<object>(fo.DicomTag.SpecificCharacterSet,(object)null) ;
            instanceDs.Add<object>(fo.DicomTag.SOPClassUID,(object)null) ;
            instanceDs.Add<object>(fo.DicomTag.SOPInstanceUID,(object)null) ;
            instanceDs.Add<object>(fo.DicomTag.InstanceNumber,(object)null) ;

            instanceDs.Add<object>(fo.DicomTag.Rows, (object)null);
            instanceDs.Add<object>(fo.DicomTag.Columns, (object)null);
            instanceDs.Add<object>(fo.DicomTag.BitsAllocated, (object)null);
            instanceDs.Add<object>(fo.DicomTag.NumberOfFrames, (object)null);
        }
    }
}
