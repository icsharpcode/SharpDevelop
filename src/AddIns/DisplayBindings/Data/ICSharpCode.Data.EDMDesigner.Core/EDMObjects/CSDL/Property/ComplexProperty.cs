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
using System.Collections;
using System.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property
{
    public class ComplexProperty : PropertyBase
    {
        #region Fields

        private ComplexType _complexType;

        #endregion

        #region Constructors

        public ComplexProperty(ComplexType complexType)
        {
            ComplexType = complexType;
        }

        internal ComplexProperty(string complexTypeName)
        {
            ComplexTypeName = complexTypeName;
        }

        #endregion

        #region Properties

        protected override Func<TypeBase, IList> GetPropertyCollection
        {
            get { return entityType => entityType.ComplexProperties; }
        }

        public ComplexType ComplexType
        {
            get
            {
                if (_complexType == null)
                    _complexType = EntityType.Container.ComplexTypes.FirstOrDefault(ct => ct.Name == ComplexTypeName);
                return _complexType;
            }
            private set { _complexType = value; }
        }

        internal string ComplexTypeName { get; private set; }

        #endregion

        #region Methods

        protected override PropertyBase Create()
        {
            return new ComplexProperty(ComplexType);
        }

        #endregion
    }
}
