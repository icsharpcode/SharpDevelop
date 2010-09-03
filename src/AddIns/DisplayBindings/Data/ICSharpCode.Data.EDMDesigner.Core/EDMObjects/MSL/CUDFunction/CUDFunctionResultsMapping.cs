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
    public class CUDFunctionResultsMapping : IEnumerable<KeyValuePair<ScalarProperty, string>>
    {
        private Dictionary<ScalarProperty, string> _resultsmapping;
        private Dictionary<ScalarProperty, string> ResultsMapping
        {
            get
            {
                if (_resultsmapping == null)
                    _resultsmapping = new Dictionary<ScalarProperty, string>();
                return _resultsmapping;
            }
        }
        public string this[ScalarProperty scalarProperty]
        {
            get
            {
                if (ResultsMapping.ContainsKey(scalarProperty))
                    return ResultsMapping[scalarProperty];
                return null;
            }
            set
            {
                if (value == null)
                    ResultsMapping.Remove(scalarProperty);
                else if (ResultsMapping.ContainsKey(scalarProperty))
                    ResultsMapping[scalarProperty] = value;
                else
                    ResultsMapping.Add(scalarProperty, value);
            }
        }

        public IEnumerator<KeyValuePair<ScalarProperty, string>> GetEnumerator()
        {
            return ResultsMapping.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
