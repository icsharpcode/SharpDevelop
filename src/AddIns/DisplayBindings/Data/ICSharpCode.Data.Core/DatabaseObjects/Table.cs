// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
