// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Association
{
    public class Association : EDMObjectBase
    {
        #region Fields

        private string _propertyEnd1Role;
        private string _propertyEnd2Role;

        private NavigationProperty _propertyEnd1;
        private NavigationProperty _propertyEnd2;

        private AssociationMapping _mapping;

        #endregion

        #region Properties

        public override string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;

                if (AssociationSetName == null)
                    AssociationSetName = _name;

                OnPropertyChanged("Name");
            }
        }

        public string AssociationSetName { get; internal set; }

        public NavigationProperty PropertyEnd1
        {
            get { return _propertyEnd1; }
            internal set
            {
                if (_propertyEnd1 != null)
                {
                    _propertyEnd1.CardinalityChanged -= ApplyCardinalityRules;
                    if (_propertyEnd1.Cardinality == Cardinality.ZeroToOne)
                        RemoveNavigationPropertyMappingListeners(_propertyEnd1);
                }
                _propertyEnd1 = value;
                ApplyCardinalityRules();
                if (_propertyEnd1 != null)
                {
                    _propertyEnd1.CardinalityChanged += ApplyCardinalityRules;
                    if (_propertyEnd1.Cardinality == Cardinality.ZeroToOne)
                        AddNavigationPropertyMappingListeners(_propertyEnd1);
                }
            }
        }

        public string PropertyEnd1Role
        {
            get
            {
                if (_propertyEnd1Role == null)
                    _propertyEnd1Role = PropertyEnd1 == null ? null : Property1Name;
                return _propertyEnd1Role;
            }
            internal set
            {
                _propertyEnd1Role = value;
            }
        }

        [DisplayName("Property 1")]
        public string Property1Name
        {
            get { return PropertyEnd1.Name; }
            set { PropertyEnd1.Name = value; }
        }
        [DisplayName("Property 1 cardinality")]
        public Cardinality Property1Cardinality
        {
            get { return PropertyEnd1.Cardinality; }
            set { PropertyEnd1.Cardinality = value; }
        }

        public NavigationProperty PropertyEnd2
        {
            get { return _propertyEnd2; }
            internal set
            {
                if (_propertyEnd2 != null)
                {
                    _propertyEnd2.CardinalityChanged -= ApplyCardinalityRules;
                    if (_propertyEnd2.Cardinality == Cardinality.ZeroToOne)
                        RemoveNavigationPropertyMappingListeners(_propertyEnd2);
                }
                _propertyEnd2 = value;
                ApplyCardinalityRules();
                if (_propertyEnd2 != null)
                {
                    _propertyEnd2.CardinalityChanged += ApplyCardinalityRules;
                    if (_propertyEnd2.Cardinality == Cardinality.ZeroToOne)
                        AddNavigationPropertyMappingListeners(_propertyEnd2);
                }
            }
        }

        public string PropertyEnd2Role
        {
            get
            {
                if (_propertyEnd2Role == null)
                    _propertyEnd2Role = PropertyEnd2 == null ? null : Property2Name;
                return _propertyEnd2Role;
            }
            internal set
            {
                _propertyEnd2Role = value;
            }
        }

        [DisplayName("Property 2")]
        public string Property2Name
        {
            get { return PropertyEnd2.Name; }
            set { PropertyEnd2.Name = value; }
        }

        [DisplayName("Property 2 cardinality")]
        public Cardinality Property2Cardinality
        {
            get { return PropertyEnd2.Cardinality; }
            set { PropertyEnd2.Cardinality = value; }
        }

        public IEnumerable<ScalarProperty> PrincipalProperties { get; internal set; }
        public string PrincipalRole { get; internal set; }

        public IEnumerable<ScalarProperty> DependentProperties { get; internal set; }
        public string DependentRole { get; internal set; }

        public IEnumerable<NavigationProperty> PropertiesEnd
        {
            get
            {
                yield return PropertyEnd1;
                yield return PropertyEnd2;
            }
        }

        public CSDLContainer Container { get; internal set; }

        public AssociationMapping Mapping
        {
            get
            {
                if (_mapping == null)
                    _mapping = new AssociationMapping(this);
                return _mapping;
            }
        }

        #endregion

        #region Methods

        private void ApplyCardinalityRules()
        {
            if (PropertyEnd1 == null || PropertyEnd2 == null)
                return;

            if (PropertyEnd1.Cardinality == Cardinality.ZeroToOne)
                AddNotNullCondition(PropertyEnd1);
            else
                RemoveNotNullCondition(PropertyEnd1);
            if (PropertyEnd2.Cardinality == Cardinality.ZeroToOne)
                AddNotNullCondition(PropertyEnd2);
            else
                RemoveNotNullCondition(PropertyEnd2);

            if (PropertyEnd1.EntityType != null && PropertyEnd2.EntityType != null)
            {
                if (PropertyEnd1.Cardinality == Cardinality.One)
                {
                    PrincipalRole = PropertyEnd1Role;
                    PrincipalProperties = PropertyEnd1.EntityType.Keys;
                    DependentRole = PropertyEnd2Role;
                    DependentProperties = PropertyEnd2.EntityType.Keys;
                }
                else if (PropertyEnd2.Cardinality == Cardinality.One)
                {
                    PrincipalRole = PropertyEnd2Role;
                    PrincipalProperties = PropertyEnd2.EntityType.Keys;
                    DependentRole = PropertyEnd1Role;
                    DependentProperties = PropertyEnd1.EntityType.Keys;
                }
                else
                {
                    PrincipalRole = null;
                    PrincipalProperties = Enumerable.Empty<ScalarProperty>();
                    DependentRole = null;
                    DependentProperties = Enumerable.Empty<ScalarProperty>();
                }
            }
        }

        private void AddNotNullCondition(NavigationProperty navigationProperty)
        {
            foreach (var column in navigationProperty.Mapping.Select(pm => pm.Column))
                if (!Mapping.ConditionsMapping.Any(ccm => ccm.Column == column && ccm.Operator == ConditionOperator.IsNotNull))
                    AddNotNullCondition(column);
            AddNavigationPropertyMappingListeners(navigationProperty);
        }

        private void AddNotNullCondition(SSDL.Property.Property column)
        {
            if (!Mapping.ConditionsMapping.Any(ccm => ccm.Column == column && ccm.Operator == ConditionOperator.IsNotNull))
                Mapping.ConditionsMapping.Add(new ColumnConditionMapping { Column = column, Operator = ConditionOperator.IsNotNull, Generated = true });
        }

        private void RemoveNotNullCondition(NavigationProperty navigationProperty)
        {
            foreach (var column in navigationProperty.Mapping.Select(pm => pm.Column))
                RemoveNotNullCondition(column);
            RemoveNavigationPropertyMappingListeners(navigationProperty);
        }

        private void RemoveNotNullCondition(SSDL.Property.Property column)
        {
            var columnConditionMapping = Mapping.ConditionsMapping.FirstOrDefault(ccm => ccm.Column == column && ccm.Operator == ConditionOperator.IsNotNull && ccm.Generated);
            if (columnConditionMapping != null)
                Mapping.ConditionsMapping.Remove(columnConditionMapping);
        }

        private void AddNavigationPropertyMappingListeners(NavigationProperty navigationProperty)
        {
            navigationProperty.Mapping.Added += (p, c) => AddNotNullCondition(c);
            navigationProperty.Mapping.Changed +=
                (p, oldC, newC) =>
                {
                    RemoveNotNullCondition(oldC);
                    AddNotNullCondition(newC);
                };
            navigationProperty.Mapping.Removed += (p, c) => RemoveNotNullCondition(c);
        }

        private void RemoveNavigationPropertyMappingListeners(NavigationProperty navigationProperty)
        {
        }

        public string GetRoleName(NavigationProperty navigationProperty)
        {
            if (navigationProperty == PropertyEnd1)
                return PropertyEnd1Role;
            if (navigationProperty == PropertyEnd2)
                return PropertyEnd2Role;
            throw new InvalidOperationException();
        }

        #endregion
    }
}
