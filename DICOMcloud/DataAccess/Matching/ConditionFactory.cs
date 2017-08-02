using fo = Dicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Matching
{
    public class ConditionFactory : DicomDataParameterFactory<IMatchingCondition>
    {
        private static RangeMatching      studyDateTime = new RangeMatching ( new List<uint> ( ), (uint) fo.DicomTag.StudyDate ) ;
        private static IMatchingCondition seqMatching = new SequenceMatching ( ) ;
        private static IMatchingCondition uidMatching = new ListofUIDMatching ( ) ;
        private static IMatchingCondition rngMatching = new RangeMatching ( ) ;
        private static IMatchingCondition wicMatching = new WildCardMatching ( ) ;
        private static IMatchingCondition sivMatching = new SingleValueMatching ( ) ;
        private static IMatchingCondition uniMatching = new UniversalMatching ( ) ;
        
        static ConditionFactory ( )
        {
            IList<uint> supportedTags = studyDateTime.SupportedTags ;

            supportedTags.Add ( (uint) fo.DicomTag.StudyDate);
            supportedTags.Add ( (uint) fo.DicomTag.StudyTime);
        }

        public ConditionFactory ( )
        {
        }

        protected override void PopulateTemplate ( List<IDicomDataParameter> parametersTemplate )
        {
            parametersTemplate.Add ( studyDateTime ) ;
            parametersTemplate.Add ( seqMatching ) ;
            parametersTemplate.Add ( uidMatching ) ;
            parametersTemplate.Add ( rngMatching ) ;
            parametersTemplate.Add ( wicMatching ) ;
            parametersTemplate.Add ( sivMatching ) ;
            parametersTemplate.Add ( uniMatching ) ;

            //C# is not accepting this
            //R matchingParam = new SequenceMatching ( ) ;
            //or this:
            //parametersTemplate.Add ( new SequenceMatching ( ) ) ;
        }

        //public virtual IEnumerable<IMatchingCondition> End ( )
        //{ 
        //    return (ICollection<IMatchingCondition>) InternalResult ;
        //}
    }
}
