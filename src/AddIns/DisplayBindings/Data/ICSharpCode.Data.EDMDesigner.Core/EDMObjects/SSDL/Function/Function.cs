// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
