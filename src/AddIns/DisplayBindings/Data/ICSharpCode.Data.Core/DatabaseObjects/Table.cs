// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.Core.Interfaces;
using System.ComponentModel;
using System.Collections.ObjectModel;

#endregion

namespace ICSharpCode.Data.Core.DatabaseObjects
{
    public class Table : DatabaseObjectBase<IColumn>, ITable
    {
        #region Fields

        private string _schemaName = string.Empty;
        private string _tableName = string.Empty;
        private DatabaseObjectsCollection<IConstraint> _constraints = null;

        #endregion

        #region Properties

        public string SchemaName
        {
            get { return _schemaName; }
            set 
            {
                _schemaName = value;
                OnPropertyChanged("SchemaName");
                OnPropertyChanged("Name");
            }
        }

        public string TableName
        {
            get { return _tableName; }
            set 
            { 
                _tableName = value;
                OnPropertyChanged("TableName");
                OnPropertyChanged("Name");
            }
        }

        public DatabaseObjectsCollection<IConstraint> Constraints
        {
            get { return _constraints; }
            set 
            {
                _constraints = value;
                OnPropertyChanged("Constraints");
            }
        }

        public new string Name
        {
            get { return string.Format("[{0}].[{1}]", _schemaName, _tableName); }
        }

        public bool HasKeyDefined
        {
            get 
            {
                if (Items.FirstOrDefault(column => column.IsPrimaryKey) != null)
                    return true;
                else
                    return false;
            }
        }

        public bool HasCompositeKey
        {
            get
            {
                if (Items.Count(column => column.IsPrimaryKey) > 1)
                    return true;
                else
                    return false;
            }
        }

        public override bool IsSelected
        {
            get
            {
                if (HasKeyDefined || GetType().GetInterface("IView") != null)
                    return base.IsSelected;
                else
                    return false;
            }
            set { base.IsSelected = value; }
        }

        #endregion

        #region IDatabaseObjectBase Members

        string IDatabaseObjectBase.Name
        {
            get
            {
                return Name;
            }
            set
            {
            }
        }

        #endregion
    }
}
