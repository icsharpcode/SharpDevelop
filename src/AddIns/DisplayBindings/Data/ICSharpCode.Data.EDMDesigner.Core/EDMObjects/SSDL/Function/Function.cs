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
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Function
{
    public class Function : EDMObjectBase
    {
        #region Fields

        private string _returnType;
        private string _commandText;
        private List<FunctionParameter> _parameters;

        #endregion

        #region Properties

        public SSDLContainer Container { get; internal set; }

        public bool? Aggregate { get; set; }
        public bool? BuiltIn { get; set; }
        public bool? IsComposable { get; set; }
        public bool? NiladicFunction { get; set; }
        public string Schema { get; set; }
        public string StoreName { get; set; }
        public string StoreSchema { get; set; }
        public string StoreType { get; set; }
        public string StoreFunctionName { get; set; }
        public ParameterTypeSemantics? ParameterTypeSemantics { get; set; }

        public string ReturnType
        {
            get { return _returnType; }
            set
            {
                _returnType = value;
                OnPropertyChanged("ReturnType");
            }
        }

        public string CommandText
        {
            get { return _commandText; }
            set
            {
                _commandText = value;
                OnPropertyChanged("CommandText");
            }
        }

        public List<FunctionParameter> Parameters
        {
            get
            {
                if (_parameters == null)
                    _parameters = new List<FunctionParameter>();
                return _parameters;
            }
        }

        #endregion
    }
}
