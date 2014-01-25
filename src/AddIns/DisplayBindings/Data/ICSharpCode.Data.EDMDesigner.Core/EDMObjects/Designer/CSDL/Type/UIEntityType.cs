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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type
{
    public class UIEntityType : UITypeBase<EntityType>
    {
        private ObservableCollection<UIEntityType> _subEntityTypes;

        internal UIEntityType(CSDLView view, EntityType entityType)
            : base(view, entityType)
        {
            BusinessInstance.BaseTypeChanged += BusinessInstanceBaseTypeChanged;
            BusinessInstance.EntitySetNameChanged += () => view.EntityTypesPropertyChanged();
        }

        private void BusinessInstanceBaseTypeChanged(EntityType et)
        {
            OnPropertyChanged("SubEntityTypes");
            OnPropertyChanged("EntitySetName");
            OnPropertyChanged("Bold");
            View.EntityTypesPropertyChanged();
            OnBaseTypeChanged();
        }

        internal UIEntityType(CSDLView view, EntityType entityType, UIEntityType subEntitiType)
            : this(view, entityType)
        {
            InternalSubEntityTypes.Add(subEntitiType);
        }


        private ObservableCollection<UIEntityType> InternalSubEntityTypes
        {
            get
            {
                if (_subEntityTypes == null)
                    _subEntityTypes = new ObservableCollection<UIEntityType>();
                return _subEntityTypes;
            }
        }
        public ObservableCollection<UIEntityType> SubEntityTypes
        {
            get
            {
                if (InternalSubEntityTypes.Count == 0)
                    foreach (var subEntityType in BusinessInstance.SubEntityTypes.OrderBy(set => set.Name))
                        AddSubEntityType(subEntityType);
                BusinessInstance.SubEntityTypeAdded +=
                    (et, subET) =>
                    {
                        View.EntityTypes.GetAndAddIfNotExist(subET, set => new UIEntityType(View, set));
                        AddSubEntityType(subET);
                    };
                return InternalSubEntityTypes;
            }
        }

        private void AddSubEntityType(EntityType subEntityType)
        {
            InternalSubEntityTypes.Add(View.EntityTypes[subEntityType]);
            subEntityType.BaseTypeChanged +=
                et =>
                {
                    InternalSubEntityTypes.Remove(InternalSubEntityTypes.First(uiet => uiet.BusinessInstance == et));
                    BusinessInstanceBaseTypeChanged(et);
                };
        }

        public string EntitySetName
        {
            get { return BusinessInstance.EntitySetName; }
            set { BusinessInstance.EntitySetName = value; }
        }

        public bool Bold { get; internal set; }

        protected override IEnumerable<PropertyBase> BusinessProperties
        {
            get
            {
                return base.BusinessProperties.Union(BusinessInstance.NavigationProperties.Cast<PropertyBase>());
            }
        }

        protected override void InitProperties()
        {
            base.InitProperties();
            foreach (var property in BusinessInstance.NavigationProperties)
                Properties.Add(new UIRelatedProperty(this, property));
            BusinessInstance.NavigationProperties.ItemAdded += newProperty => Properties.Add(new UIRelatedProperty(this, newProperty));
            BusinessInstance.NavigationProperties.ItemRemoved += oldProperty => RemoveProperty(oldProperty);
        }

        public override void DeleteProperty(UIProperty property)
        {
            base.DeleteProperty(property);
            var navigationProperty = property.BusinessInstance as NavigationProperty;
            if (navigationProperty != null)
                BusinessInstance.NavigationProperties.Remove(navigationProperty);
        }

        public event Action AbstractChanged
        {
            add { BusinessInstance.AbstractChanged += value; }
            remove { BusinessInstance.AbstractChanged -= value; }
        }

        protected virtual void OnBaseTypeChanged()
        {
            if (BaseTypeChanged != null)
                BaseTypeChanged();
        }
        public event Action BaseTypeChanged;
    }
}
