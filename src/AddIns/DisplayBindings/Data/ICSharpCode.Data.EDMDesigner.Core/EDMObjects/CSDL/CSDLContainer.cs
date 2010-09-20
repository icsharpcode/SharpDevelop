// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Function;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL
{
    public class CSDLContainer
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }

        public IEnumerable<EntityType> EntitySets
        {
            get
            {
                return EntityTypes.Where(entityType => entityType.IsEntitySet);
            }
        }

        private EventedObservableCollection<EntityType> _entityTypes;
        public EventedObservableCollection<EntityType> EntityTypes
        {
            get
            {
                if (_entityTypes == null)
                {
                    _entityTypes = new EventedObservableCollection<EntityType>();
                    _entityTypes.ItemAdded += entityType => entityType.Container = this;
                    _entityTypes.ItemRemoved +=
                        entityType =>
                        {
                            foreach (var navigationProperty in entityType.NavigationProperties.ToList())
                                entityType.NavigationProperties.Remove(navigationProperty);
                        };
                }
                return _entityTypes;
            }
        }

        public EntityType AddEntityType(string typeName, string entitySetName, EntityType baseType)
        {
            var value = new EntityType { Name = typeName, EntitySetName = entitySetName, Container = this, BaseType = baseType };
            EntityTypes.Add(value);
            return value;
        }

        public ComplexType AddComplexType(string typeName)
        {
            var value = new ComplexType { Name = typeName };
            ComplexTypes.Add(value);
            return value;
        }

        private EventedObservableCollection<ComplexType> _complexTypes;
        public EventedObservableCollection<ComplexType> ComplexTypes
        {
            get
            {
                if (_complexTypes == null)
                {
                    _complexTypes = new EventedObservableCollection<ComplexType>();
                    _complexTypes.ItemAdded += complextype => complextype.Container = this;
                    _complexTypes.ItemRemoved +=
                        complexType =>
                        {
                            foreach (var entityType in EntitySets)
                            {
                                foreach (var complexProperty in entityType.ComplexProperties.Where(cp => cp.ComplexType == complexType).ToList())
                                    entityType.ComplexProperties.Remove(complexProperty);
                            }
                            foreach (var otherComplexType in ComplexTypes)
                            {
                                foreach (var complexProperty in otherComplexType.ComplexProperties.Where(cp => cp.ComplexType == complexType).ToList())
                                    otherComplexType.ComplexProperties.Remove(complexProperty);
                            }
                        };
                }
                return _complexTypes;
            }
        }

        public IEnumerable<Association.Association> Associations
        {
            get
            {
                return EntityTypes.SelectMany(et => et.NavigationProperties).Select(np => np.Association).Distinct();
            }
        }
        public Association.Association AddAssociation(string associationName, string navigationProperty1Name, EntityType navigationProperty1EnityType, Cardinality navigationProperty1Cardinality, string navigationProperty2Name, EntityType navigationProperty2EnityType, Cardinality navigationProperty2Cardinality)
        {
            var association = new Association.Association { Container = this, Name = associationName };
            var navigationProperty1 = new NavigationProperty(association) { Name = navigationProperty1Name, Cardinality = navigationProperty1Cardinality };
            navigationProperty1EnityType.NavigationProperties.Add(navigationProperty1);
            association.PropertyEnd1 = navigationProperty1;
            var navigationProperty2 = new NavigationProperty(association) { Name = navigationProperty2Name, Cardinality = navigationProperty2Cardinality };
            navigationProperty2EnityType.NavigationProperties.Add(navigationProperty2);
            association.PropertyEnd2 = navigationProperty2;
            return association;
        }
        private EventedObservableCollection<Association.Association> _associationsCreated;
        internal EventedObservableCollection<Association.Association> AssociationsCreated
        {
            get
            {
                if (_associationsCreated == null)
                {
                    _associationsCreated = new EventedObservableCollection<Association.Association>();
                    _associationsCreated.ItemAdded += association => association.Container = this;
                }
                return _associationsCreated;
            }
        }

        private EventedObservableCollection<Function.Function> _functions;
        public EventedObservableCollection<Function.Function> Functions
        {
            get
            {
                if (_functions == null)
                    _functions = new EventedObservableCollection<Function.Function>();
                return _functions;
            }
        }
    }
}
