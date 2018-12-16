using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud
{
    public enum ObjectQueryLevel
    {
        Study,
        Series,
        Instance,
        Unknown
    }

    public abstract class ObjectQueryLevelConstants
    { 
        public static readonly string Study = Enum.GetName(typeof(ObjectQueryLevel), ObjectQueryLevel.Study);
        public static readonly string Series = Enum.GetName(typeof(ObjectQueryLevel), ObjectQueryLevel.Series);
        public static readonly string Instance = Enum.GetName(typeof(ObjectQueryLevel), ObjectQueryLevel.Instance);
    }
}
