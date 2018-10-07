using DICOMcloud.DataAccess.Database.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using fo = Dicom;

namespace DICOMcloud.DataAccess.Database
{
    public partial class QueryResponseBuilder : IQueryResponseBuilder
    {
        private EntityReadData  CurrentData             = null ;
        private KeyToDataSetCollection CurrentResultSet = null ;
        private ResultSetCollection    ResultSets       = new ResultSetCollection ( ) ;
        private string CurrentResultSetName             = "" ;

        public string QueryLevelTableName { get; set; }

        public DbSchemaProvider SchemaProvider
        {
            get;
            protected set;
        }

        public QueryResponseBuilder ( DbSchemaProvider schemaProvider, string queryLevel )
        {
            SchemaProvider      = schemaProvider ;
            QueryLevelTableName = schemaProvider.GetQueryTable ( queryLevel ) ;
        }

        public virtual void BeginResultSet( string name )
        {
            if ( !ResultSets.TryGetValue ( name, out CurrentResultSet ) ) 
            {
                CurrentResultSet = new KeyToDataSetCollection ( ) ;
            }

            CurrentResultSetName = name ;
        }

        public virtual void EndResultSet ( )
        { 
            if ( !ResultSets.ContainsKey ( CurrentResultSetName ) )
            {
                ResultSets[CurrentResultSetName] = CurrentResultSet ;
            }
        }

        public virtual void BeginRead( )
        {
            CurrentData = new EntityReadData ( ) ;
        }

        public virtual void EndRead ( )
        {
            if ( !string.IsNullOrEmpty (CurrentData.KeyValue))
            {
                UpdateDsPersonName ( ) ;
        
                CurrentResultSet[CurrentData.KeyValue] = CurrentData.CurrentDs ;
            }
            
            CurrentData = null ;
        }

        public virtual bool ResultExists ( string table, object keyValue )
        {
            KeyToDataSetCollection resultSet ;
            

            if ( ResultSets.TryGetValue ( table, out resultSet ) )
            {
                return resultSet.Contains ( keyValue.ToString ( ) ) ;
            }

            return false ;
        }

        public virtual void ReadData ( string tableName, string columnName, object value )
        { 
            var column = SchemaProvider.GetColumn( tableName, columnName ) ;
            var dicomTags = column.Tags ;

            
            if ( IsDBNullValue ( value ))
            {
                return ;
            }

            if ( column.IsKey )
            { 
                CurrentData.KeyValue = value.ToString ( ) ;
            }
            
            if ( column.IsForeign )
            {
                string keyString = value.ToString ( ) ;

                KeyToDataSetCollection resultSet = null ;
                
                if ( ResultSets.TryGetValue ( column.Table.Parent, out resultSet ) )
                {             
                    fo.DicomDataset foreignDs = (fo.DicomDataset) resultSet[keyString] ;

                    if ( QueryLevelTableName == column.Table.Name )
                    { 
                        foreignDs.Merge ( CurrentData.CurrentDs ) ;

                        //resultSet[keyString] = CurrentData.CurrentDs ;
                    }
                    else
                    { 
                        if ( column.Table.IsSequence )
                        { 
                            fo.DicomSequence sq = (fo.DicomSequence) CurrentData.ForeignDs.Get<fo.DicomSequence> (CurrentData.ForeignTagValue) ;
                            fo.DicomDataset item = new fo.DicomDataset ( ) ;
                            
                            sq.Items.Add ( item ) ;

                            CurrentData.CurrentDs.Merge ( item ) ;

                            CurrentData.CurrentDs = item ; 
                        }
                        else if ( column.Table.IsMultiValue )
                        { 
                            CurrentData.CurrentDs = foreignDs ;
                        }
                        else
                        {
                            CurrentData.CurrentDs.Merge ( foreignDs ) ;

                            foreignDs.CopyTo ( CurrentData.CurrentDs ) ; //TODO: check if above merge is still necessary with this new CopyTo method
                        }
                    }
                }
            }

            if (null == dicomTags) { return;}

            ReadTags(columnName, value, dicomTags);
        }

        private void ReadTags(string columnName, object value, uint[] dicomTags)
        {
            foreach ( var dicomTag in dicomTags )
            {
                fo.DicomDictionaryEntry dicEntry = fo.DicomDictionary.Default[dicomTag];
                var vr = dicEntry.ValueRepresentations.First() ;
                Type valueType = value.GetType ( ) ;

                if ( vr == fo.DicomVR.PN )
                {
                    PersonNameParts currentPart = SchemaProvider.GetPNColumnPart ( columnName ) ;

                    if ( CurrentData.CurrentPersonNameData == null )
                    { 
                        CurrentData.CurrentPersonNameData = new PersonNameData ( ) ;
                        CurrentData.CurrentPersonNameTagValue  = (uint) dicEntry.Tag ;
                        CurrentData.CurrentPersonNames.Add ( CurrentData.CurrentPersonNameTagValue , CurrentData.CurrentPersonNameData ) ;
                    }
                    else
                    { 
                        if ( dicEntry.Tag != CurrentData.CurrentPersonNameTagValue )
                        { 
                            if ( CurrentData.CurrentPersonNames.TryGetValue ( (uint)dicEntry.Tag, out CurrentData.CurrentPersonNameData ) )
                            {
                                CurrentData.CurrentPersonNameTagValue = (uint) dicEntry.Tag ;
                            }
                            else
                            { 
                                CurrentData.CurrentPersonNameData = new PersonNameData ( ) ;
                                CurrentData.CurrentPersonNameTagValue  = (uint) dicEntry.Tag ;
                                CurrentData.CurrentPersonNames.Add ( CurrentData.CurrentPersonNameTagValue , CurrentData.CurrentPersonNameData ) ;
                            }
                        }
                    }

                    CurrentData.CurrentPersonNameData.SetPart ( currentPart, (string) value ) ;
                }
                    
                if (valueType == typeof(String)) //shortcut
                {
                    CurrentData.CurrentDs.AddOrUpdate<string>(dicomTag, (string) value);
                }
                else if (valueType == typeof(DateTime))
                {
                    CurrentData.CurrentDs.AddOrUpdate<DateTime>(dicomTag, (DateTime) value);
                }

                else if (valueType == typeof(Int32))
                {
                    CurrentData.CurrentDs.AddOrUpdate<Int32>(dicomTag, (Int32)value);
                }
                else if (valueType == typeof(Int64))
                {
                    CurrentData.CurrentDs.AddOrUpdate<Int64>(dicomTag, (Int64)value);
                }
                else
                {
                    CurrentData.CurrentDs.AddOrUpdate<string>(dicomTag, value as string);

                    System.Diagnostics.Debug.Assert(false, "Unknown element db value");
                }
            }
        }
        
        public virtual IEnumerable<fo.DicomDataset> GetResponse ( )
        { 
            if ( !ResultSets.ContainsKey ( QueryLevelTableName) )
            {
                return new KeyToDataSetCollection ( ).Values.OfType<fo.DicomDataset>() ;
            }

            return ResultSets[QueryLevelTableName].Values.OfType<fo.DicomDataset>() ;
        }

        private void UpdateDsPersonName()
        {
            if (null != CurrentData.CurrentPersonNames)
            {
                foreach (var personName in CurrentData.CurrentPersonNames)
                {
                    CurrentData.CurrentDs.AddOrUpdate( personName.Key, personName.Value.ToString());
                }
            }

            CurrentData.CurrentPersonNames.Clear();

            CurrentData.CurrentPersonNames = new Dictionary<uint, PersonNameData>();
        
            CurrentData.CurrentPersonNameData = null ;
        }

        private bool IsDBNullValue ( object value )
        {
            return DBNull.Value == value || value == null ;
        }
    }
}
