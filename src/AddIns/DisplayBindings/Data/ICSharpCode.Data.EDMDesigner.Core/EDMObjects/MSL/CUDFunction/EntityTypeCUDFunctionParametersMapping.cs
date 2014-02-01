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

using System.Collections.Generic;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.CUDFunction
{
    public class EntityTypeCUDFunctionParametersMapping : CUDFunctionParametersMapping
    {
        #region Fields

        private EventedObservableCollection<KeyValuePair<ComplexProperty, EntityTypeCUDFunctionParametersMapping>> _complexPropertiesMapping;

        #endregion

        #region Properties

        public EventedObservableCollection<KeyValuePair<ComplexProperty, EntityTypeCUDFunctionParametersMapping>> ComplexPropertiesMapping
        {
            get
            {
                if (_complexPropertiesMapping == null)
                    _complexPropertiesMapping = new EventedObservableCollection<KeyValuePair<ComplexProperty, EntityTypeCUDFunctionParametersMapping>>();
                return _complexPropertiesMapping;
            }
        }

        #endregion
    }
}
