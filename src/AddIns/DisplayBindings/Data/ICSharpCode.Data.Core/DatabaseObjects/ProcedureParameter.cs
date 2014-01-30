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
