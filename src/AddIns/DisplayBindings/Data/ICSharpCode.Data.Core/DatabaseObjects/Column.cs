// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.Core.Interfaces;

#endregion

namespace ICSharpCode.Data.Core.DatabaseObjects
{
    public class Column : DatabaseObjectBase, IColumn
    {
        #region Fields

        private ITable _parentTable = null;
        private string _dataType = string.Empty;
        private string _systemType = string.Empty;
        private int _length = 0;
        private bool _allowsNull = false;
        private int _columnId = 0;
        private int _fullTextTypeColumn = 0;
        private bool _isComputed = false;
        private bool _isCursorType = false;
        private bool _isDeterministic = false;
        private bool _isFulltextIndexed = false;
        private bool _isIdentity = false;
        private bool _isIdNotForRepl = false;
        private bool _isIndexable = false;
        private bool _isOutParam = false;
        private bool _isPrecise = false;
        private bool _isPrimaryKey = false;
        private bool _isRowGuidCol = false;
        private bool _isSystemVerified = false;
        private bool _isXmlIndexable = false;
        private int _precision = 0;
        private int _scale = 0;
        private bool _systemDataAccess = false;
        private bool _userDataAccess = false;
        private bool _usesAnsiTrim = false;

        #endregion

        #region Constructor

        public Column(ITable parentTable)
        {
            _parentTable = parentTable;
        }

        #endregion

        #region Properties

        public string ColumnSummary
        {
            get 
            { 
                string allowsNull = string.Empty;

                if (IsNullable)
                    allowsNull = "null";
                else
                    allowsNull = "not null";

                bool isForeignKey = IsForeignKey;

                if (IsPrimaryKey && !isForeignKey)
                    return string.Format("{0} (PK, {1}, {2})", Name, DataType, allowsNull); 
                else if (!IsPrimaryKey && isForeignKey)
                    return string.Format("{0} (FK, {1}, {2})", Name, DataType, allowsNull); 
                else if (IsPrimaryKey && isForeignKey)
                    return string.Format("{0} (PK, FK, {1}, {2})", Name, DataType, allowsNull); 
                else
                    return string.Format("{0} ({1}, {2})", Name, DataType, allowsNull); 
            }
        }

        public string DataType
        {
            get { return _dataType; }
            set
            {
                _dataType = value;
                OnPropertyChanged("DataType");
            }
        }

        public string SystemType
        {
            get { return _systemType; }
            set
            {
                _systemType = value;
                OnPropertyChanged("SystemType");
            }
        }

        public int Length
        {
            get { return _length; }
            set
            {
                _length = value;
                OnPropertyChanged("Length");
            }
        }

        public bool IsNullable
        {
            get { return _allowsNull; }
            set
            {
                _allowsNull = value;
                OnPropertyChanged("AllowsNull");
            }
        }

        public int ColumnId
        {
            get { return _columnId; }
            set
            {
                _columnId = value;
                OnPropertyChanged("ColumnId");
            }
        }

        public int FullTextTypeColumn
        {
            get { return _fullTextTypeColumn; }
            set
            {
                _fullTextTypeColumn = value;
                OnPropertyChanged("FullTextTypeColumn");
            }
        }

        public bool IsComputed
        {
            get { return _isComputed; }
            set
            {
                _isComputed = value;
                OnPropertyChanged("IsComputed");
            }
        }

        public bool IsCursorType
        {
            get { return _isCursorType; }
            set
            {
                _isCursorType = value;
                OnPropertyChanged("IsCursorType");
            }
        }

        public bool IsDeterministic
        {
            get { return _isDeterministic; }
            set
            {
                _isDeterministic = value;
                OnPropertyChanged("IsDeterministic");
            }
        }

        public bool IsFulltextIndexed
        {
            get { return _isFulltextIndexed; }
            set
            {
                _isFulltextIndexed = value;
                OnPropertyChanged("IsFulltextIndexed");
            }
        }

        public bool IsIdentity
        {
            get { return _isIdentity; }
            set
            {
                _isIdentity = value;
                OnPropertyChanged("IsIdentity");
            }
        }

        public bool IsForeignKey
        {
            get
            {
                if (_parentTable.Constraints == null)
                    return false;
                
                IConstraint constraint = _parentTable.Constraints.FirstOrDefault(constr => constr.FKColumns.FirstOrDefault(column => column.ColumnId == ColumnId && column.Name == Name) != null);

                if (constraint == null)
                    return false;
                else
                    return true;
            }
        }

        public bool IsIdNotForRepl
        {
            get { return _isIdNotForRepl; }
            set
            {
                _isIdNotForRepl = value;
                OnPropertyChanged("IsIdNotForRepl");
            }
        }

        public bool IsIndexable
        {
            get { return _isIndexable; }
            set
            {
                _isIndexable = value;
                OnPropertyChanged("IsIndexable");
            }
        }

        public bool IsOutParam
        {
            get { return _isOutParam; }
            set
            {
                _isOutParam = value;
                OnPropertyChanged("IsOutParam");
            }
        }

        public bool IsPrecise
        {
            get { return _isPrecise; }
            set
            {
                _isPrecise = value;
                OnPropertyChanged("IsPrecise");
            }
        }

        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
            set
            {
                _isPrimaryKey = value;
                OnPropertyChanged("IsPrimaryKey");
            }
        }

        public bool IsRowGuidCol
        {
            get { return _isRowGuidCol; }
            set
            {
                _isRowGuidCol = value;
                OnPropertyChanged("IsRowGuidCol");
            }
        }

        public bool IsSystemVerified
        {
            get { return _isSystemVerified; }
            set
            {
                _isSystemVerified = value;
                OnPropertyChanged("IsSystemVerified");
            }
        }

        public bool IsUserDefinedDataType
        {
            get
            {
                if (string.IsNullOrEmpty(_systemType))
                    return false;
                else
                    return true;
            }
        }

        public bool IsXmlIndexable
        {
            get { return _isXmlIndexable; }
            set
            {
                _isXmlIndexable = value;
                OnPropertyChanged("IsXmlIndexable");
            }
        }

        public int Precision
        {
            get { return _precision; }
            set
            {
                _precision = value;
                OnPropertyChanged("Precision");
            }
        }

        public int Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                OnPropertyChanged("Scale");
            }
        }

        public bool SystemDataAccess
        {
            get { return _systemDataAccess; }
            set
            {
                _systemDataAccess = value;
                OnPropertyChanged("SystemDataAccess");
            }
        }

        public bool UserDataAccess
        {
            get { return _userDataAccess; }
            set
            {
                _userDataAccess = value;
                OnPropertyChanged("UserDataAccess");
            }
        }

        public bool UsesAnsiTrim
        {
            get { return _usesAnsiTrim; }
            set
            {
                _usesAnsiTrim = value;
                OnPropertyChanged("UsesAnsiTrim");
            }
        }

        #endregion
    }
}
