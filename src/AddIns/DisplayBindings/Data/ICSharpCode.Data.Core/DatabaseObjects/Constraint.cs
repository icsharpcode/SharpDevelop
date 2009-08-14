#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.Core.Interfaces;
using ICSharpCode.Data.Core.Enums;

#endregion

namespace ICSharpCode.Data.Core.DatabaseObjects
{
    public class Constraint : DatabaseObjectBase, IConstraint
    {
        #region Fields

        private string _pkColumnName = string.Empty;
        private string _pkTableName = string.Empty;
        private string _fkColumnName = string.Empty;
        private string _fkTableName = string.Empty;

        #endregion

        #region Properties

        public string PKColumnName 
        {
            get { return _pkColumnName; }
            set
            {
                _pkColumnName = value;
                OnPropertyChanged("PKColumnName");
            }
        }

        public string PKTableName 
        {
            get { return _pkTableName; }
            set
            {
                _pkTableName = value;
                OnPropertyChanged("PKTableName");
            }
        }

        public string FKColumnName 
        {
            get { return _fkColumnName; }
            set
            {
                _fkColumnName = value;
                OnPropertyChanged("FKColumnName");
            }
        }

        public string FKTableName 
        {
            get { return _fkTableName; }
            set
            {
                _fkTableName = value;
                OnPropertyChanged("FKTableName");
            }
        }

        public IColumn PKColumn 
        {
            get
            {
                ITable table = PKTable;

                if (table != null)
                    return GetColumnFromTableByName(table, PKColumnName);
                else
                    return null;
            }
        }

        public ITable PKTable 
        {
            get { return GetTableByName(PKTableName); }
        }

        public Cardinality PKCardinality
        {
            get
            {
                if (!FKColumn.IsPrimaryKey && FKColumn.IsNullable)
                    return Cardinality.ZeroToOne;
                else if (!FKColumn.IsPrimaryKey && !FKColumn.IsNullable)
                    return Cardinality.One;
                else if (!FKColumn.IsPrimaryKey)
                    return Cardinality.Many;
                else
                    return Cardinality.One;
            }
        }

        public IColumn FKColumn 
        {
            get 
            {
                ITable table = FKTable;

                if (table != null)
                    return GetColumnFromTableByName(table, FKColumnName);
                else
                    return null;
            }
        }

        public ITable FKTable 
        {
            get { return GetTableByName(FKTableName); }
        }

        public Cardinality FKCardinality
        {
            get
            {
                if (PKColumn.IsPrimaryKey)
                    return Cardinality.Many;
                else if (!PKColumn.IsPrimaryKey && !FKColumn.IsNullable)
                    return Cardinality.One;
                else // !PKColumn.IsPrimaryKey && FKColumn.IsNullable
                    return Cardinality.ZeroToOne;
            }
        }

        #endregion

        #region Methods

        private ITable GetTableByName(string name)
        { 
            if (Parent is ITable)
            {
                ITable table = Parent as ITable;
                
                if (table.Parent is IDatabase)
                {
                    IDatabase database = table.Parent as IDatabase;

                    return database.Tables.FirstOrDefault(t => t.TableName == name);
                }
            }

            return null;
        }

        private IColumn GetColumnFromTableByName(ITable table, string columnName)
        {
            return table.Items.FirstOrDefault(c => c.Name == columnName);
        }

        #endregion
    }
}
