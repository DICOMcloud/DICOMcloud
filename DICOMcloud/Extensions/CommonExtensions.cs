using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DICOMcloud.Extensions
{
    public static class CommonExtensions
    {
        public static string ToJson ( this object me, bool camelCase = false  )
        {
            if ( camelCase ) 
            {
                JsonSerializerSettings settings = new JsonSerializerSettings ( ) ;

                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                return Newtonsoft.Json.JsonConvert.SerializeObject (me, settings) ;
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject (me) ;
            }
        }

        public static dynamic FromJson ( this string me )
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic> (me) ;
        }

        public static T FromJson<T> ( this string me )
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T> ( me) ;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) 
        {
            return enumerable == null || !enumerable.Any();
        }

        //http://stackoverflow.com/questions/7265315/replace-multiple-characters-in-a-string
        public static string Replace(this string s, char[] separators, string newVal)
        {
           string[] temp;

           temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
           return String.Join( newVal, temp );
        }
    }
}
