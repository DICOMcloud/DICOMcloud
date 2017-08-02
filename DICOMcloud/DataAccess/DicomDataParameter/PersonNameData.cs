namespace DICOMcloud.DataAccess
{
    public class PersonNameData
    {
        public string LastName { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
    
        public string[] ToArray ( )
        { 
            return new string[] { LastName, GivenName, MiddleName, Prefix, Suffix } ;
        }

        public void SetPart(PersonNameParts currentPart, string value)
        {
            switch ( currentPart )
            { 
                case PersonNameParts.Family:
                { 
                    LastName = value ;
                }
                break;

                case PersonNameParts.Given:
                { 
                    GivenName = value ;
                }
                break;

                case PersonNameParts.Middle:
                { 
                    MiddleName = value ;
                }
                break;

                case PersonNameParts.Prefix:
                { 
                    Prefix = value ;
                }
                break;

                case PersonNameParts.Suffix:
                { 
                    Suffix = value ;
                }
                break;
            }
        }

        public override string ToString()
        {
            return string.Format ( "{0}^{1}^{2}^{3}^{4}", LastName, GivenName, MiddleName, Prefix, Suffix ).TrimEnd ( new char [] {'^'} ) ;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode ( ) ;
        }
    }
}
