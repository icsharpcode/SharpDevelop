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
    public class ProcedureParameter : DatabaseObjectBase, IProcedureParameter
    {
        #region Fields

        private string _dataType = string.Empty;
        private int _length = 0;
        private ParameterMode _parameterMode;

        #endregion

        #region Properties

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

        public ParameterMode ParameterMode
        {
            get { return _parameterMode; }
            set 
            {
                _parameterMode = value;
                OnPropertyChanged("ParameterMode");
            }
        }

        #endregion
    }
}
