// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.CUDFunction
{
    public class CUDFunctionParametersMapping : IEnumerable<KeyValuePair<ScalarProperty, FunctionParameterMapping>>
    {
        private Dictionary<ScalarProperty, FunctionParameterMapping> _parametersmapping;
        private Dictionary<ScalarProperty, FunctionParameterMapping> ParametersMapping
        {
            get
            {
                if (_parametersmapping == null)
                    _parametersmapping = new Dictionary<ScalarProperty, FunctionParameterMapping>();
                return _parametersmapping;
            }
        }
        public FunctionParameterMapping this[ScalarProperty scalarProperty]
        {
            get
            {
                if (ParametersMapping.ContainsKey(scalarProperty))
                    return ParametersMapping[scalarProperty];
                return null;
            }
            set
            {
                if (value == null)
                    ParametersMapping.Remove(scalarProperty);
                else if (ParametersMapping.ContainsKey(scalarProperty))
                    ParametersMapping[scalarProperty] = value;
                else
                    ParametersMapping.Add(scalarProperty, value);
            }
        }

        public IEnumerator<KeyValuePair<ScalarProperty, FunctionParameterMapping>> GetEnumerator()
        {
            return ParametersMapping.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
