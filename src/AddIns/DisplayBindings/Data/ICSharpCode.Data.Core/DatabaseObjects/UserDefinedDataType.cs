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
    public class UserDefinedDataType : DatabaseObjectBase, IUserDefinedDataType
    {
        #region Fields

        private string _systemType = string.Empty;
        private int _length = 0;
        private bool _isNullable = false;

        #endregion

        #region Properties

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
            get { return _isNullable; }
            set
            {
                _isNullable = value;
                OnPropertyChanged("IsNullable");
            }
        }

        #endregion
    }
}
