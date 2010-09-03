// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
