// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

        private List<string> _pkColumnNames = new List<string>();
        private string _pkTableName = string.Empty;
        private List<string> _fkColumnNames = new List<string>();
        private string _fkTableName = string.Empty;

        #endregion

        #region Properties

        public List<string> PKColumnNames 
        {
            get { return _pkColumnNames; }
            set
            {
                _pkColumnNames = value;
                OnPropertyChanged("PKColumnNames");
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

        public List<string> FKColumnNames 
        {
            get { return _fkColumnNames; }
            set
            {
                _fkColumnNames = value;
                OnPropertyChanged("FKColumnNames");
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

        public DatabaseObjectsCollection<IColumn> PKColumns
        {
            get
            {
                ITable table = PKTable;

                if (table != null)
                    return GetColumnsFromTableByName(table, PKColumnNames);
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
                IColumn fkColumn = FKColumns.First();

                if (!fkColumn.IsPrimaryKey && fkColumn.IsNullable)
                    return Cardinality.ZeroToOne;
                else if (!fkColumn.IsPrimaryKey && !fkColumn.IsNullable)
                    return Cardinality.One;
                else if (!fkColumn.IsPrimaryKey)
                    return Cardinality.Many;
                else
                    return Cardinality.One;
            }
        }

        public DatabaseObjectsCollection<IColumn> FKColumns
        {
            get 
            {
                ITable table = FKTable;

                if (table != null)
                    return GetColumnsFromTableByName(table, FKColumnNames);
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
                IColumn pkColumn = PKColumns.First();

                if (!FKTable.HasCompositeKey && FKColumns.FirstOrDefault(fkColumn => fkColumn.IsPrimaryKey && fkColumn.IsForeignKey) != null)
                    return Cardinality.ZeroToOne;
                else if (pkColumn.IsPrimaryKey)
                    return Cardinality.Many;
                else if (!pkColumn.IsPrimaryKey && !FKColumns.First().IsNullable)
                    return Cardinality.One;
                else
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

        private DatabaseObjectsCollection<IColumn> GetColumnsFromTableByName(ITable table, List<string> columnNames)
        {     
            return table.Items.Where(c => columnNames.Contains(c.Name)).ToDatabaseObjectsCollection();
        }

        #endregion
    }
}
