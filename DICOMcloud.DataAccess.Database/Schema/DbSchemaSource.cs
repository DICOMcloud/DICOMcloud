using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DICOMcloud.DataAccess.Database.Schema
{
    public class DbSchemaSource
    {
        public ConcurrentDictionary<string,TableKey> Tables { get; protected set; }
        public ConcurrentDictionary<string, ColumnInfo> Columns { get; protected set; }
        public ConcurrentDictionary<uint, IList<ColumnInfo>> Tags { get; protected set; }

        public DbSchemaSource ( )
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "DICOMcloud.DataAccess.Database.DatabaseSchema.xml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();

                Init ( XDocument.Parse ( result ) )  ;
            }
        }

        public DbSchemaSource ( string schemaPath )
        {
            Init (  XDocument.Load ( schemaPath ) ) ;
        }

        public string GetColumnKey ( string tableName, string columnName )
        {
            return string.Format ( "{0}.{1}", tableName, columnName ) ;
        }

        private void Init ( XDocument schemaDoc )
        {
            Tables  = new ConcurrentDictionary<string, TableKey>   ( ) ;
            Columns = new ConcurrentDictionary<string, ColumnInfo> ( ) ;
            Tags    = new ConcurrentDictionary<uint, IList<ColumnInfo>>   ( ) ;

            DbSchema = schemaDoc ;
            
            foreach ( var parentElement in schemaDoc.Root.Elements ( ) )
            {
                if ( parentElement.Name == "tables" )
                {
                    CreateTables ( parentElement ) ;
                }
                else if ( parentElement.Name == "columns" )
                {
                    CreateColumnes ( parentElement ) ;
                }
            }
        }

        private void CreateColumnes(XElement parentElement)
        {
            foreach ( var childElement in parentElement.Elements ( ) )
            {
                ColumnInfo column = new ColumnInfo ( ) ;

                column.Name         = childElement.Attribute ("name").Value ;
                column.Table        = Tables[childElement.Attribute ( "table" ).Value] ;
                column.Defenition   = childElement.Attribute ("defenition").Value ;
                column.IsForeign    = GetIsForeign  ( childElement ) ;
                column.IsKey        = GetIsKey      ( childElement ) ;
                column.IsNumber     = GetIsNumber   ( childElement ) ;
                column.IsData       = GetIsData     ( childElement ) ;
                column.IsDateTime   = GetIsDateTime ( childElement ) ; 
                column.IsModelKey   = GetIsModelKey ( childElement ) ;

                column.Table.Columns.Add ( column ) ;

                if ( column.IsKey )
                { 
                    column.Table.KeyColumn = column ;
                }

                if ( column.IsForeign )
                {
                    column.Table.ForeignColumn = column ;
                }

                if ( column.IsModelKey )
                { 
                    column.Table.ModelKeyColumns.Add ( column ) ;
                }

                var tags =  childElement.Attribute("tag").Value ;

                if ( !string.IsNullOrWhiteSpace ( tags ) )
                { 
                    column.Tags = Array.ConvertAll ( tags.Split ( new char[] {','}, 
                                                     StringSplitOptions.RemoveEmptyEntries), uint.Parse ) ;
                }
                else
                { 
                    column.Tags = new uint[0] ;
                }

                Columns[ GetColumnKey (column.Table.Name, column.Name) ] = column ;

                foreach ( uint tagValue in column.Tags )
                {
                    if ( !Tags.ContainsKey(tagValue) )
                    {
                        Tags[tagValue] = new List<ColumnInfo> ( ) ;
                    }

                    Tags[tagValue].Add ( column ) ;
                }
            }
        }

        private bool GetIsDateTime(XElement childElement)
        {
            return ReadBoolValue (childElement, "isDate") ;
        }

        private bool GetIsNumber(XElement childElement)
        {
            return ReadBoolValue ( childElement, "isNum") ;
        }

        private bool GetIsData(XElement childElement)
        {
            return ReadBoolValue ( childElement, "isData") ;
        }

        private bool GetIsKey(XElement childElement)
        {
            var keyAttrib = childElement.Attribute ( "key" ) ;

            if ( null != keyAttrib )
            { 
                bool result ;
                
                if ( bool.TryParse ( keyAttrib.Value, out  result ) )
                { 
                    return result ;
                }
            }

            return false ;
        }

        private bool GetIsForeign ( XElement childElement )
        {
            return ReadBoolValue ( childElement, "foreign" ) ;
        }

        private bool GetIsModelKey ( XElement childElement )
        { 
            return ReadBoolValue ( childElement, "modelKey" ) ;        
        }

        private void CreateTables ( XElement parentElement )
        {
            foreach ( var childElement in parentElement.Elements ( ) )
            {
                string tableName = childElement.Attribute ( "name" ).Value ;

                TableKey table = Tables.GetOrAdd ( tableName, ( string key  ) => 
                                                                {
                                                                    return new TableKey ( ) { Name = tableName } ;
                                                                } ) ;

                table.OrderValue = ushort.Parse ( childElement.Attribute ( "order" ).Value ) ;

                var parent = childElement.Attribute("parent" ).Value ;

                if ( !string.IsNullOrWhiteSpace ( parent ) )
                {
                     table.Parent = Tables [parent] ;
                }

                table.IsSequence    = ReadIsSequence    ( childElement ) ;
                table.IsMultiValue  = ReadIsMultiValue  ( childElement ) ;
                table.ParentElement = ReadParentElement ( childElement ) ;
            }
        }

        private uint ReadParentElement ( XElement childElement )
        {
            var tagAttrib = childElement.Attribute ( "tag" ) ;

            return ( null != tagAttrib ) ? uint.Parse ( tagAttrib.Value ) : 0 ;
        }

        private bool ReadIsMultiValue(XElement childElement)
        {
            return ReadBoolValue ( childElement, "multi" ) ;
        }

        private bool ReadIsSequence(XElement childElement)
        {
            return ReadBoolValue ( childElement, "seq" ) ;
        }

        private static bool ReadBoolValue ( XElement childElement, string attribName )
        {
            var multiAttrib = childElement.Attribute ( attribName ) ;

            return (null != multiAttrib) ? bool.Parse(multiAttrib.Value) : false ;
        }

        public virtual XDocument DbSchema
        {
            get; 
            set;
        }

        private static string PathToBin()
        {
            return Path.GetDirectoryName( Assembly.GetExecutingAssembly().GetName().CodeBase);
        }
    }
}
