using System;
using System.Collections.Generic;
using System.Linq;
using fo = Dicom;

namespace DICOMcloud.DataAccess
{
    public class DicomDataParameter : IDicomDataParameter
    {
        public virtual uint        KeyTag            { get;  set ; }
        public virtual fo.DicomVR  VR                { get; set ; }
        public virtual bool        AllowExtraElement {  get; set ;}
        public         IList<uint> SupportedTags     { get ; protected set; }

        public DicomDataParameter() : this (null ) {
        
        }

        public DicomDataParameter(IList<uint> supportedTags ) {
            Elements = new List<fo.DicomItem> ( ) ;
            SupportedTags = supportedTags ?? new List<uint> ( ) ;
        }

        public virtual string[] GetValues ( ) 
        {
            if ( Elements.Count == 1 )
            { 
                var item = Elements.First ( ) as fo.DicomElement ;
                var value = "" ;


                if (item.Length > 0)
                {
                    value = item.Get<string>().TrimEnd('\0');
                }

                if ( item.Tag.DictionaryEntry.ValueMultiplicity.Maximum > 1 ) //better way to find if has multi value
                {
                    return value.Split ( '/' ) ;
                }
                else
                {
                    return new string[] { value } ;
                }
            }

            return null ;
        }
        public virtual void SetElement ( fo.DicomItem element )
        {
            Elements.Add ( element ) ;

            if ( KeyTag == 0 )
            { 
                KeyTag = (uint) element.Tag ;
            }

            if ( VR == null )
            { 
                VR = element.ValueRepresentation ;
            }
        }

        public virtual bool IsSupported(fo.DicomItem element) {

            if ( null != SupportedTags && SupportedTags.Count > 0 )
            {
                return SupportedTags.Contains ( (uint) element.Tag ) ;
            }

            return true ;
        }
        
        public virtual IDicomDataParameter CreateParameter ( )
        { 
            IDicomDataParameter dicomParam = (IDicomDataParameter) Activator.CreateInstance (this.GetType ( ) ) ;

            dicomParam.KeyTag            = KeyTag ;
            dicomParam.VR                = VR ;
            dicomParam.AllowExtraElement = AllowExtraElement ;

            foreach (var tag in SupportedTags)
            {
                dicomParam.SupportedTags.Add ( tag ) ;    
            }
            
            return dicomParam ;
        }
        
        public virtual List<PersonNameData> GetPNValues ( )
        {
            if ( VR != fo.DicomVR.PN) {return null ; }

            List<PersonNameData> result   = new List<PersonNameData> ( ) ;
            string[]             pnValues = GetValues ( ) ;

            foreach ( string pnValue in pnValues )
            { 
                PersonNameData pnData = new PersonNameData ( ) ;
                string[] pnParts = pnValue.Split ( '^') ;
                int length = pnParts.Length ;

                if ( length > 0 )
                { 
                    pnData.LastName = pnParts [0] ;
                }
                if ( length > 1 )
                { 
                    pnData.GivenName = pnParts [ 1 ] ;
                }
                if ( length > 2 )
                { 
                    pnData.MiddleName = pnParts [ 2 ] ;
                }
                if ( length > 3 )
                { 
                    pnData.Prefix = pnParts [ 3 ] ;
                }
                if ( length > 4 )
                { 
                    pnData.Suffix = pnParts [ 4 ] ;
                }

                result.Add ( pnData ) ;
            }

            return result ;
        }

        public IList<fo.DicomItem> Elements { get; set ; }
    }

    public class StoreParameter : DicomDataParameter
    {
        public StoreParameter() : base ( ){ }  
        public StoreParameter(IList<uint> supportedTags) : base ( supportedTags ) { }
        
        public override string[] GetValues()
        {
            if ( Elements.Count == 0 )
            { 
                return base.GetValues();
            }

            if ( Elements.Count > 1 )
            {
                if ( VR == fo.DicomVR.TM || VR == fo.DicomVR.DA )
                {
                    List<string> values = new List<string> ( ) ;

                    for ( int index = 0; index < Elements.Count; index+=2)
                    {
                        fo.DicomItem dateElement = null ;
                        fo.DicomItem timeElement = null ;


                        for ( int localIndex = index; localIndex < index + 2 && localIndex < Elements.Count; localIndex++)
                        {
                            var element = Elements[localIndex] ;

                            if ( element.ValueRepresentation == fo.DicomVR.DA )
                            {
                                dateElement = element ;
                            }
                            else if ( element.ValueRepresentation == fo.DicomVR.TM )
                            {
                                timeElement = element ;
                            }
                        }

                        if ( null != dateElement )
                        {
                            
                        }


                    }
                }
            }

            return base.GetValues ( ) ;
        }

        public override bool AllowExtraElement
        {
            get
            {
                if ( SupportedTags.Count > 0 )
                { 
                    return Elements.Count < SupportedTags.Count ;
                }

                return base.AllowExtraElement ;
            }
            set
            {
                base.AllowExtraElement = value;
            }
        }
    }
}
