// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
