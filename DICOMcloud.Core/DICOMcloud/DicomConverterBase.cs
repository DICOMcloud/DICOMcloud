using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Dicom;

namespace DICOMcloud
{
    public abstract partial class DicomConverterBase
    {
        protected virtual Dicom.IO.Buffer.IByteBuffer GetItemBuffer ( DicomItem item )
        {
            Dicom.IO.Buffer.IByteBuffer buffer;


            if ( item is DicomFragmentSequence )
            {
                var dicomfragmentSq = (DicomFragmentSequence) item;
                var sb = new StringBuilder ( );
                
                
                buffer = dicomfragmentSq.Fragments.Count == 1 ? dicomfragmentSq.Fragments[0] :
                                                                new Dicom.IO.Buffer.CompositeByteBuffer ( dicomfragmentSq.Fragments.ToArray ( ) );
            }
            else
            {
                var dicomElement = (DicomElement) item;


                buffer = dicomElement.Buffer;
            }

            return buffer;
        }
    
        public bool WriteInlineBinary
        {
            get;
            set;
        }

        //protected virtual void WriteVR_Binary ( DicomItem item )
        //{
        //    OnWriteBinary ( ) ;

        //    DoWriteVR_Binary ( item ) ;
        //}

        //protected abstract void OnWriteBinary ( ) ;

        //protected abstract void DoWriteVR_Binary ( DicomItem item ) ;
    }
}
