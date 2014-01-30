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
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Common;
using System.ComponentModel;
using System.Collections.ObjectModel;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type
{
    public class TypeBase : EDMObjectBase
    {
        #region Fields

        private Visibility _visibility;
        private EventedObservableCollection<ScalarProperty> _scalarProperties;
        private EventedObservableCollection<ComplexProperty> _complexProperties;

        #endregion

        #region Properties

        [DisplayName("Access")]
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
            }
        }

        public EventedObservableCollection<ScalarProperty> ScalarProperties
        {
            get
            {
                if (_scalarProperties == null)
                {
                    _scalarProperties = new EventedObservableCollection<ScalarProperty>();
                    _scalarProperties.ItemAdded += scalarProperty => scalarProperty.EntityType = this;
                    _scalarProperties.ItemRemoved += scalarProperty => OnScalarPropertyRemoved(scalarProperty);
                }
                return _scalarProperties;
            }
        }

        public EventedObservableCollection<ComplexProperty> ComplexProperties
        {
            get
            {
                if (_complexProperties == null)
                {
                    _complexProperties = new EventedObservableCollection<ComplexProperty>();
                    _complexProperties.ItemAdded += complexProperty => complexProperty.EntityType = this;
                }
                return _complexProperties;
            }
        }

        public virtual IEnumerable<ScalarProperty> AllScalarProperties
        {
            get
            {
                return ScalarProperties;
            }
        }

        public virtual IEnumerable<ComplexProperty> AllComplexProperties
        {
            get
            {
                return ComplexProperties;
            }
        }

        internal CSDLContainer Container { get; set; }

        #endregion

        #region Constructor

        internal TypeBase()
        {
        }

        #endregion

        #region Methods

        public virtual PropertyBase DuplicateProperty(PropertyBase property)
        {
            var propertyBase = property.Duplicate();
            propertyBase.AddToType(this);
            return propertyBase;
        }

        protected virtual void OnScalarPropertyRemoved(ScalarProperty scalarProperty)
        {
        }

        #endregion
    }
}
