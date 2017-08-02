using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using fo = Dicom;

namespace DICOMcloud
{
    public abstract partial class DicomConverterBase
    {
        protected virtual fo.IO.Buffer.IByteBuffer GetItemBuffer ( fo.DicomItem item )
        {
            fo.IO.Buffer.IByteBuffer buffer;


            if ( item is fo.DicomFragmentSequence )
            {
                var dicomfragmentSq = (fo.DicomFragmentSequence) item;
                var sb = new StringBuilder ( );
                
                
                buffer = dicomfragmentSq.Fragments.Count == 1 ? dicomfragmentSq.Fragments[0] :
                                                                new fo.IO.Buffer.CompositeByteBuffer ( dicomfragmentSq.Fragments.ToArray ( ) );
            }
            else
            {
                var dicomElement = (fo.DicomElement) item;


                buffer = dicomElement.Buffer;
            }

            return buffer;
        }
    
        public bool WriteInlineBinary
        {
            get;
            set;
        }

        //protected virtual void WriteVR_Binary ( fo.DicomItem item )
        //{
        //    OnWriteBinary ( ) ;

        //    DoWriteVR_Binary ( item ) ;
        //}

        //protected abstract void OnWriteBinary ( ) ;

        //protected abstract void DoWriteVR_Binary ( fo.DicomItem item ) ;
    }
}
