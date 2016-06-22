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
using System.ComponentModel;
using System.Collections.Specialized;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Attributes;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType;
using System.Collections.ObjectModel;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type
{
    public class EntityType : TypeBase
    {
        #region Fields

        private string _entitySetName;
        private bool _isEntitySet;
        private bool _abstract;
        private Visibility _entitySetVisibility;
        private EventedObservableCollection<NavigationProperty> _navigationProperties;
        private EntityType _baseType;
        private EntityTypeMapping _mapping;

        #endregion

        #region Events

        public event Action AbstractChanged;
        public event Action EntitySetNameChanged;
        public event Action<EntityType> BaseTypeChanged;
        public event Action<EntityType, EntityType> SubEntityTypeAdded;

        #endregion

        #region Constructor

        public EntityType()
        {
            NotifyCollectionChangedEventHandler propertiesChanged = delegate
            {
                Mapping.OnIsCompletelyMappedChanged();
            };

            ScalarProperties.CollectionChanged += propertiesChanged;
            ComplexProperties.CollectionChanged += propertiesChanged;
        }

        #endregion

        #region Properties

        [DisplayName("Entity Set Name")]
        [DisplayEnabledCondition("IsEntitySet")]
        public string EntitySetName
        {
            get
            {
                if (IsEntitySet && _entitySetName != null)
                    return _entitySetName;
                var entitySet = GetEntitySet();
                return entitySet == null ? null : entitySet.EntitySetName;
            }
            set
            {
                _entitySetName = value;
                if (!string.IsNullOrEmpty(value))
                    IsEntitySet = true;
                OnPropertyChanged("EntitySetName");
                OnEntitySetNameChanged();
            }
        }

        public bool NoBaseEntitySet
        {
            get
            {
                var entitySet = GetEntitySet();
                return entitySet == null || entitySet == this;
            }
        }

        [DisplayName("Is Entity Set")]
        [DisplayVisibleCondition("NoBaseEntitySet")]
        public bool IsEntitySet
        {
            get
            {
                if (NoBaseEntitySet)
                    return _isEntitySet;
                return false;
            }
            set
            {
                _isEntitySet = value;
                if (value && _entitySetName == null)
                    _entitySetName = string.Concat(Name, "Set");
                OnPropertyChanged("IsEntitySet");
                OnPropertyChanged("EntitySetName");
                OnEntitySetNameChanged();
            }
        }

        [DisplayName("Getter")]
        [DisplayVisibleCondition("IsEntitySet")]
        public Visibility EntitySetVisibility
        {
            get { return _entitySetVisibility; }
            set
            {
                _entitySetVisibility = value;
                OnPropertyChanged("EntitySetVisibility");
            }
        }

        public bool Abstract
        {
            get { return _abstract; }
            set
            {
                _abstract = value;
                OnAbstractChanged();
                OnPropertyChanged("Abstract");
                Mapping.OnIsCompletelyMappedChanged();
            }
        }

        [DisplayName("Base Type")]
        [ExcludeItselft]
        [AddNull]
        [ExcludeChildrenTypes]
        public EntityType BaseType
        {
            get { return _baseType; }
            set
            {
                if (_baseType == value)
                    return;
                if (value != null)
                {
                    if (value.IsEntitySet)
                        IsEntitySet = false;
                }
                else
                {
                    if (_baseType.IsEntitySet)
                        IsEntitySet = true;
                }
                _baseType = value;
                OnPropertyChanged("BaseType");
                OnPropertyChanged("EntitySetName");
                OnPropertyChanged("IsEntitySet");
                OnPropertyChanged("NoBaseEntitySet");
                OnBaseTypeChanged();
                if (value != null)
                    value.OnSubEntityTypeAdded(this);
            }
        }

        public bool IsBaseType(EntityType type)
        {
            var t = type;
            while ((t = t.BaseType) != null)
                if (t == this)
                    return true;
            return false;
        }

        public IEnumerable<EntityType> SubEntityTypes
        {
            get
            {
                return Container.EntityTypes.Where(et => et.BaseType == this);
            }
        }

        public EntityType GetEntitySet()
        {
            var value = this;
            do
            {
                if (value._isEntitySet)
                    return value;
                value = value.BaseType;
            } while (value != null);
            return null;
        }

        public EntityTypeMapping Mapping
        {
            get
            {
                if (_mapping == null)
                    _mapping = new EntityTypeMapping(this);
                return _mapping;
            }
        }

        public EventedObservableCollection<NavigationProperty> NavigationProperties
        {
            get
            {
                if (_navigationProperties == null)
                {
                    _navigationProperties = new EventedObservableCollection<NavigationProperty>();
                    _navigationProperties.ItemAdded += navigationProperty => navigationProperty.EntityType = this;
                    _navigationProperties.ItemRemoved += _navigationProperties_ItemRemoved;
                }

                return _navigationProperties;
            }
        }

        public override IEnumerable<ScalarProperty> AllScalarProperties
        {
            get
            {
                if (BaseType == null)
                    return ScalarProperties;
                return ScalarProperties.Union(BaseType.AllScalarProperties);
            }
        }

        public IEnumerable<NavigationProperty> AllNavigationProperties
        {
            get
            {
                if (BaseType == null)
                    return NavigationProperties;
                return NavigationProperties.Union(BaseType.NavigationProperties);
            }
        }

        public override IEnumerable<ComplexProperty> AllComplexProperties
        {
            get
            {
                if (BaseType == null)
                    return ComplexProperties;
                return ComplexProperties.Union(BaseType.ComplexProperties);
            }
        }

        public IEnumerable<ScalarProperty> SpecificKeys
        {
            get { return ScalarProperties.Where(sp => sp.IsKey); }
        }

        public IEnumerable<ScalarProperty> Keys
        {
            get { return AllScalarProperties.Where(sp => sp.IsKey); }
        }

        #endregion

        #region Methods

        protected override void OnScalarPropertyRemoved(ScalarProperty scalarProperty)
        {
            base.OnScalarPropertyRemoved(scalarProperty);
            Mapping.RemoveMapping(scalarProperty);
        }

        protected virtual void OnAbstractChanged()
        {
            if (AbstractChanged != null)
                AbstractChanged();
        }

        protected virtual void OnEntitySetNameChanged()
        {
            if (EntitySetNameChanged != null)
                EntitySetNameChanged();
        }



        protected virtual void OnBaseTypeChanged()
        {
            if (BaseTypeChanged != null)
                BaseTypeChanged(this);
        }


        protected virtual void OnSubEntityTypeAdded(EntityType entityType)
        {
            if (SubEntityTypeAdded != null)
                SubEntityTypeAdded(this, entityType);
        }

        private void _navigationProperties_ItemRemoved(NavigationProperty navigationProperty)
        {
            _navigationProperties.ItemRemoved -= _navigationProperties_ItemRemoved;
            var navigationProperty2 = navigationProperty.Association.PropertiesEnd.First(pe => pe != navigationProperty);
            if (!navigationProperty2.IsDeleted)
                navigationProperty2.EntityType.NavigationProperties.Remove(navigationProperty2);
            _navigationProperties.ItemRemoved += _navigationProperties_ItemRemoved;
        }
        
        #endregion
    }
}
