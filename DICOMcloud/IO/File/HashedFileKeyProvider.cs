using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    /// <summary>
    /// Returns information about the storage files based on a key.
    /// This class produces an MD5 hash value of the key parts if it is longer than 32 characters
    /// This can be used when storing files on a Windows machine that can't handle file path longer than 256 characters
    /// </summary>
    public class HashedFileKeyProvider : FileKeyProvider
    {
        public override string GetStorageKey(IMediaId id)
        {
            return Path.Combine ( id.GetIdParts( ).Select ( GetPartKey ).ToArray ( ) ) ;
        }

        private string GetPartKey ( string partId )
        {
            if ( partId.Length > 32 )
            {
                return CalculateMD5Hash ( partId ) ;
            }

            return partId ;
        }

        //https://blogs.msdn.microsoft.com/csharpfaq/2006/10/09/how-do-i-calculate-a-md5-hash-from-a-string/
        private static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
     }
}
