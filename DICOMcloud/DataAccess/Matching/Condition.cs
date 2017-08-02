using fo = Dicom;
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
            Elements      = new List<fo.DicomItem> ( ) ;
            SupportedTags = supportedTags ?? new List<uint> ( );
            KeyTag        = keyTag ;

            ExactMatch = true ;
        }

        public virtual bool IsCaseSensitive { get;  protected set ; }

        public virtual bool ExactMatch { get;  protected set ;}

        public virtual bool SupportFuzzy { get; protected set ;}

        public virtual bool CanMatch ( fo.DicomItem element )
        {
            if ( SupportedTags.Count > 0 ) 
            {
                return SupportedTags.Contains((uint) element.Tag ) ;
            }

            return true ;
        }

        public override bool IsSupported(fo.DicomItem element)
        {
            return CanMatch ( element ) ;
        }

        protected virtual bool HasWildcardMatching ( string elementValue )
        {
            return elementValue.Contains ( "*") || elementValue.Contains ( "?") ;
        }
    
    }

    public class SingleValueMatching : MatchingBase
    {
        public SingleValueMatching ( )
        {
            ExactMatch = true ;
            IsCaseSensitive = false ;
        }

        public override bool CanMatch ( fo.DicomItem item )
        {
            //if SQ casting will fail and be null
            fo.DicomElement element = item as fo.DicomElement ;


            if ( element == null || element.Count == 0 ) { return false ;}

            string elementValue = element.ToString ( ) ;

            if ( element.ValueRepresentation.Equals ( fo.DicomVR.DA) || 
                 element.ValueRepresentation.Equals ( fo.DicomVR.DT ) || 
                 element.ValueRepresentation.Equals ( fo.DicomVR.TM ) )
            {
                if ( elementValue.Contains ( "-")){return false ;}
            }
            else
            {
                if ( HasWildcardMatching (elementValue) )
                {
                    return false ;
                }
            }

            return base.CanMatch ( element ) ;
        }
    }

    public class ListofUIDMatching : MatchingBase
    {
        public override bool CanMatch(fo.DicomItem element)
        {
            if ( !element.ValueRepresentation.Equals ( fo.DicomVR.UI)) { return false ; }

            if ( ((fo.DicomElement)element).Count <= 1 ) { return false ; }

            return base.CanMatch ( element ) ;
        }
    }

    public class UniversalMatching : MatchingBase
    {
        public override bool CanMatch(fo.DicomItem item)
        {
            fo.DicomElement element = item as fo.DicomElement ;

            if ( null == element || element.Count > 0 ) { return false ; }

            return base.CanMatch ( element ) ;
        }
    }

    public class WildCardMatching : MatchingBase
    {

        private static List<fo.DicomVR> _invliadVrs = new List<fo.DicomVR> ( ) ;

        public WildCardMatching ( )
        {
            IsCaseSensitive = true ;
        }

        static WildCardMatching ( )
        {
            _invliadVrs.AddRange ( new fo.DicomVR[] { fo.DicomVR.SQ, fo.DicomVR.DA, fo.DicomVR.TM, fo.DicomVR.DT, fo.DicomVR.SL, fo.DicomVR.SS,
                                                   fo.DicomVR.US, fo.DicomVR.UL, fo.DicomVR.FL, fo.DicomVR.FD, fo.DicomVR.OB, fo.DicomVR.OW,
                                                   fo.DicomVR.UN, fo.DicomVR.AT, fo.DicomVR.DS, fo.DicomVR.IS, fo.DicomVR.AS, fo.DicomVR.UI } ) ;
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

        public override bool CanMatch(fo.DicomItem element)
        {
            if ( _invliadVrs.Contains ( element.ValueRepresentation ) ) { return false ; }

            if ( !HasWildcardMatching (element.ToString ( )) ) { return false ; }

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

        public override bool CanMatch(fo.DicomItem element)
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

        private bool MatchVr(fo.DicomItem element)
        {
            fo.DicomVR elementVr = element.ValueRepresentation ;
            if ( !elementVr.Equals ( fo.DicomVR.DA) && !elementVr.Equals ( fo.DicomVR.TM ) && !elementVr.Equals ( fo.DicomVR.DT)) { return false ; }

            if ( HasWildcardMatching (element.ToString ( ) )) { return false ; }

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

        public override void SetElement(fo.DicomItem element)
        {
            base.SetElement ( element ) ;
        
            if ( element.ValueRepresentation == fo.DicomVR.DA )
            { 
                DateElement = element ;
            }

            if ( element.ValueRepresentation == fo.DicomVR.TM )
            { 
                TimeElement = element ;
            }
        }

        public fo.DicomItem DateElement { get; protected set; }
        public fo.DicomItem TimeElement { get; protected set; }

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
        public override bool CanMatch(fo.DicomItem element)
        {
            if ( !element.ValueRepresentation.Equals (fo.DicomVR.SQ)) { return false; }

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
