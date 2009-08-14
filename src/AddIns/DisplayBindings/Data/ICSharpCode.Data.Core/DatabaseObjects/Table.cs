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
