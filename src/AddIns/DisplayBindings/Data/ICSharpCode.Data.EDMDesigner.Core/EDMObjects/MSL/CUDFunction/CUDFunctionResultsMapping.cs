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
