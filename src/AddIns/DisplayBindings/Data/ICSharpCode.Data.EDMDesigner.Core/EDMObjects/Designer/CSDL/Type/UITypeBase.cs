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
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type
{
    public abstract class UITypeBase<Type> : UIBusinessType<Type>, IUIType where Type : TypeBase
    {
        private IndexableUIBusinessTypeObservableCollection<PropertyBase, UIProperty> _properties;
        private CSDLView _view;

        protected UITypeBase(CSDLView view, Type type)
            : base(type)
        {
            View = view;
        }

        public CSDLView View
        {
            get { return _view; }
            set
            {
                _view = value;
                CSDL = _view.CSDL;
            }
        }
        protected CSDLContainer CSDL { get; set; }

        protected virtual IEnumerable<PropertyBase> BusinessProperties
        {
            get
            {
                return BusinessInstance.ScalarProperties.Cast<PropertyBase>().Union(BusinessInstance.ComplexProperties.Cast<PropertyBase>());
            }
        }

        public IndexableUIBusinessTypeObservableCollection<PropertyBase, UIProperty> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = new IndexableUIBusinessTypeObservableCollection<PropertyBase, UIProperty>();
                    InitProperties();
                }
                return _properties;
            }
        }

        protected virtual void InitProperties()
        {
            foreach (var property in BusinessInstance.ScalarProperties)
                Properties.Add(new UIProperty(this, property));
            BusinessInstance.ScalarProperties.ItemAdded += newProperty => Properties.Add(new UIProperty(this, newProperty));
            BusinessInstance.ScalarProperties.ItemRemoved += oldProperty => RemoveProperty(oldProperty);
            foreach (var property in BusinessInstance.ComplexProperties)
                Properties.Add(new UIRelatedProperty(this, property));
            BusinessInstance.ComplexProperties.ItemAdded += newProperty => Properties.Add(new UIRelatedProperty(this, newProperty));
            BusinessInstance.ComplexProperties.ItemRemoved += oldProperty => RemoveProperty(oldProperty);
        }
        protected void RemoveProperty(PropertyBase property)
        {
            var uiProperty = Properties[property];
            Properties.Remove(uiProperty);
            var uiRelatedProperty = uiProperty as UIRelatedProperty;
            if (uiRelatedProperty != null)
                OnRelatedPropertyDeleted(uiRelatedProperty);
        }

        public bool IsComplexType
        {
            get
            {
                return BusinessInstance is ComplexType;
            }
        }

        public UIProperty AddScalarProperty(string propertyName, ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property.PropertyType propertyType)
        {
            var scalarProperty = new ScalarProperty { Name = propertyName, Type = propertyType };
            BusinessInstance.ScalarProperties.Add(scalarProperty);
            return Properties[scalarProperty];
        }

        public UIRelatedProperty AddComplexProperty(string propertyName, ComplexType complexType)
        {
            var complexProperty = new ComplexProperty(complexType) { Name = propertyName };
            BusinessInstance.ComplexProperties.Add(complexProperty);
            return Properties[complexProperty] as UIRelatedProperty;
        }

        public virtual void DeleteProperty(UIProperty property)
        {
            var scalarProperty = property.BusinessInstance as ScalarProperty;
            if (scalarProperty != null)
            {
                BusinessInstance.ScalarProperties.Remove(scalarProperty);
                return;
            }
            var complexProperty = property.BusinessInstance as ComplexProperty;
            if (complexProperty != null)
            {
                BusinessInstance.ComplexProperties.Remove(complexProperty);
                return;
            }
        }

        TypeBase IUIType.BusinessInstance
        {
            get { return BusinessInstance; }
        }

        protected virtual void OnRelatedPropertyDeleted(UIRelatedProperty relatedProperty)
        {
            if (RelatedPropertyDeleted != null)
                RelatedPropertyDeleted(relatedProperty);
        }

        public event Action<UIRelatedProperty> RelatedPropertyDeleted;
    }
}
