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
    public class Procedure : DatabaseObjectBase<IProcedureParameter>, IProcedure
    {
        #region Fields

        private string _schemaName = string.Empty;
        private string _dataType = null;
        private int _length = 0;
        private ProcedureType _procedureType;

        #endregion

        #region Properties

        public string SchemaName 
        {
            get { return _schemaName; }
            set
            {
                _schemaName = value;
                OnPropertyChanged("SchemaName");
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

        public int Length
        {
            get { return _length; }
            set
            {
                _length = value;
                OnPropertyChanged("Length");
            }
        }

        public ProcedureType ProcedureType
        {
            get { return _procedureType; }
            set
            {
                _procedureType = value;
                OnPropertyChanged("ProcedureType");
            }
        }

        #endregion
    }
}
