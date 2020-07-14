using Dicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Matching
{
    public class MatchingBase : DicomDataParameter, IMatchingCondition
    {
        public MatchingBase ( ) : this ( null, 0 )
        {
            
        }

        public MatchingBase ( IList<uint> supportedTags, uint keyTag )
        {
            Elements      = new List<DicomItem> ( ) ;
            SupportedTags = supportedTags ?? new List<uint> ( );
            KeyTag        = keyTag ;

            ExactMatch = true ;
        }

        public virtual bool IsCaseSensitive { get;  protected set ; }

        public virtual bool ExactMatch { get;  protected set ;}

        public virtual bool SupportFuzzy { get; protected set ;}

        public virtual bool CanMatch ( DicomItem element )
        {
            if ( SupportedTags.Count > 0 ) 
            {
                return SupportedTags.Contains((uint) element.Tag ) ;
            }

            return true ;
        }

        public override bool IsSupported(DicomItem element)
        {
            return CanMatch ( element ) ;
        }

        protected virtual bool HasWildcardMatching(DicomItem item)
        {
            //if SQ casting will fail and be null
            DicomElement element = item as DicomElement;
            if (element == null) { return false; }
            string elementValue = element.Get<string>();

            return elementValue.Contains("*") || elementValue.Contains("?");
        }

    }

    public class SingleValueMatching : MatchingBase
    {
        public SingleValueMatching ( )
        {
            ExactMatch = true ;
            IsCaseSensitive = false ;
        }

        public override bool CanMatch ( DicomItem item )
        {
            //if SQ casting will fail and be null
            DicomElement element = item as DicomElement ;


            if ( element == null || element.Count == 0 ) { return false ;}

            string elementValue = element.Get<string>();

            if ( element.ValueRepresentation.Equals ( DicomVR.DA) || 
                 element.ValueRepresentation.Equals ( DicomVR.DT ) || 
                 element.ValueRepresentation.Equals ( DicomVR.TM ) )
            {
                if ( elementValue.Contains ( "-")){return false ;}
            }
            else
            {
                if ( HasWildcardMatching (item) )
                {
                    return false ;
                }
            }

            return base.CanMatch ( element ) ;
        }
    }

    public class ListofUIDMatching : MatchingBase
    {
        public override bool CanMatch(DicomItem element)
        {
            if ( !element.ValueRepresentation.Equals ( DicomVR.UI)) { return false ; }

            if ( ((DicomElement)element).Count <= 1 ) { return false ; }

            return base.CanMatch ( element ) ;
        }
    }

    public class UniversalMatching : MatchingBase
    {
        public override bool CanMatch(DicomItem item)
        {
            DicomElement element = item as DicomElement ;

            if ( null == element || element.Count > 0 ) { return false ; }

            return base.CanMatch ( element ) ;
        }
    }

    public class WildCardMatching : MatchingBase
    {

        private static List<DicomVR> _invliadVrs = new List<DicomVR> ( ) ;

        public WildCardMatching ( )
        {
            IsCaseSensitive = true ;
        }

        static WildCardMatching ( )
        {
            _invliadVrs.AddRange ( new DicomVR[] { DicomVR.SQ, DicomVR.DA, DicomVR.TM, DicomVR.DT, DicomVR.SL, DicomVR.SS,
                                                   DicomVR.US, DicomVR.UL, DicomVR.FL, DicomVR.FD, DicomVR.OB, DicomVR.OW,
                                                   DicomVR.UN, DicomVR.AT, DicomVR.DS, DicomVR.IS, DicomVR.AS, DicomVR.UI } ) ;
        }

        public override bool ExactMatch
        {
            get
            {
                return false ;
            }

            protected set
            {
                base.ExactMatch = value;
            }
        }

        public override bool CanMatch(DicomItem element)
        {
            if ( _invliadVrs.Contains ( element.ValueRepresentation ) ) { return false ; }

            if ( !HasWildcardMatching (element) ) { return false ; }

            return base.CanMatch ( element ) ;
        }
    }

    public class RangeMatching : MatchingBase
    {
        public RangeMatching ( ): base ()
        {

        }

        public RangeMatching ( IList<uint> supportedTags, uint keyTag ) : base(supportedTags, keyTag)
        {

        }

        public override bool CanMatch(DicomItem element)
        {
            //if ( IsRangeSupported && !DateTimeMatching.IsSupported(element.Tag.TagValue ) )
            //{
            //    return false ;
            //}

            if ( !MatchVr ( element ) )
            { 
                return false ;
            }

            return base.CanMatch ( element ) ;
        }

        private bool MatchVr(DicomItem element)
        {
            DicomVR elementVr = element.ValueRepresentation ;
            if ( !elementVr.Equals ( DicomVR.DA) && !elementVr.Equals ( DicomVR.TM ) && !elementVr.Equals ( DicomVR.DT)) { return false ; }

            if ( HasWildcardMatching (element)) { return false ; }

            return true ;
        }

        public override bool AllowExtraElement
        {
            get
            {
                if ( DateElement != null && TimeElement != null ) //we already have enough
                { 
                    return false ;
                }
                
                return true ;
            }
        }

        public override bool ExactMatch
        {
            get
            {
                return true ;
            }

            protected set
            {
                base.ExactMatch = value;
            }
        }

        public override void SetElement(DicomItem element)
        {
            base.SetElement ( element ) ;
        
            if ( element.ValueRepresentation == DicomVR.DA )
            { 
                DateElement = element ;
            }

            if ( element.ValueRepresentation == DicomVR.TM )
            { 
                TimeElement = element ;
            }
        }

        public DicomItem DateElement { get; protected set; }
        public DicomItem TimeElement { get; protected set; }

        //protected DateTimeElementsMatching DateTimeMatching { get; set; }
        
        //private bool IsRangeSupported
        //{
        //    get
        //    { 
        //        return ( DateTimeMatching != null ) ;
        //    }
        //}

        public override string[] GetValues()
        {
            //if ( DateElement != null )
            //{ 
            //    DateElement.GetDateTime()
            //}

            return base.GetValues();
        }

        public override IDicomDataParameter CreateParameter()
        {
            var rangeMatch = new RangeMatching ( SupportedTags, KeyTag ) ;

            rangeMatch.AllowExtraElement = AllowExtraElement ;
            rangeMatch.DateElement       = DateElement ;
            rangeMatch.TimeElement       = TimeElement ;
            rangeMatch.Elements          = Elements; 
            rangeMatch.ExactMatch        = ExactMatch ;
            rangeMatch.IsCaseSensitive   = IsCaseSensitive ;
            rangeMatch.SupportFuzzy      = SupportFuzzy ;
            rangeMatch.VR                = VR ;

            return rangeMatch ;
        }
    }

    public class SequenceMatching : MatchingBase
    {
        public override bool CanMatch(DicomItem element)
        {
            if ( !element.ValueRepresentation.Equals (DicomVR.SQ)) { return false; }

            return base.CanMatch ( element ) ;
        }
    }

    //public class DateTimeElementsMatching
    //{ 
    //    public uint MatchingDateTag { get; set; }
    //    public uint MatchingTimeTag { get; set; }        
    
    //    public bool IsSupported ( uint tagValue )
    //    { 
    //        return tagValue == MatchingDateTag || tagValue == MatchingTimeTag ;
    //    }
    //}

}
