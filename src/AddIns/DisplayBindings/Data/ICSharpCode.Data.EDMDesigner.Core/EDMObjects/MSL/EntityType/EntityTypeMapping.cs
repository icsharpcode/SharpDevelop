// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.CUDFunction;
using System.Collections.ObjectModel;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType
{
    public class EntityTypeMapping : MappingBase
    {
         public EntityTypeMapping(CSDL.Type.EntityType entityType)
            : base(entityType)
        {
        }

        private bool _init = true;
        internal bool Init
        {
            get { return _init; }
            set
            {
                if (_init == value)
                    return;
                _init = value;
                if (_init)
                {
                    _ssdlTables.ItemRemoved -= RemoveSSDLTable;
                    _ssdlTables.ItemAdded -= AddSSDLTable;
                }
                else
                {
                    _ssdlTables.ItemRemoved += RemoveSSDLTable;
                    _ssdlTables.ItemAdded += AddSSDLTable;
                }
            }
        }

        protected override Dictionary<SSDL.EntityType.EntityType, SSDL.Property.Property> GetBaseMapping(ScalarProperty scalarProperty)
        {
            var value = base.GetBaseMapping(scalarProperty);
            if (value != null)
                return value;
            if (EntityType.BaseType != null)
                return EntityType.BaseType.Mapping[scalarProperty];
            return null;
        }
        protected override void AddTableMapped(SSDL.EntityType.EntityType table)
        {
            base.AddTableMapped(table);
            if (!SSDLTables.Contains(table))
                SSDLTables.Add(table);
        }
        internal override ComplexPropertyMapping GetMapping(ComplexProperty complexProperty)
        {
            var value = GetSpecificMapping(complexProperty);
            if (value != null)
                return value;
            return EntityType.BaseType == null ? null : EntityType.BaseType.Mapping.GetMapping(complexProperty);
        }

        public CUDFunctionMapping InsertFunctionMapping { get; set; }
        public CUDFunctionMapping UpdateFunctionMapping { get; set; }
        public CUDFunctionMapping DeleteFunctionMapping { get; set; }

        private EventedObservableCollection<ConditionMapping> _conditionsMapping;
        public EventedObservableCollection<ConditionMapping> ConditionsMapping
        {
            get
            {
                if (_conditionsMapping == null)
                    _conditionsMapping = new EventedObservableCollection<ConditionMapping>();
                return _conditionsMapping;
            }
        }

        private EventedObservableCollection<SSDL.EntityType.EntityType> _ssdlTables;
        public EventedObservableCollection<SSDL.EntityType.EntityType> SSDLTables
        {
            get
            {
                if (_ssdlTables == null)
                {
                    _ssdlTables = new EventedObservableCollection<SSDL.EntityType.EntityType>();
                    _ssdlTables.ItemAdded += ssdlTable => AddSSDLTable(ssdlTable);
                    _ssdlTables.ItemRemoved += ssdlTable => RemoveSSDLTable(ssdlTable);
                }
                return _ssdlTables;
            }
        }

        protected override MappingBase BaseMapping
        {
            get
            {
                if (EntityType.BaseType == null)
                    return null;
                return EntityType.BaseType.Mapping;
            }
        }

        private void AddSSDLTable(SSDL.EntityType.EntityType ssdlTable)
        {
            if (MappingInit || ssdlTable == null)
                return;

            foreach (var column in ssdlTable.Properties)
            {
                var prop = EntityType.ScalarProperties.Union(EntityType.Keys).FirstOrDefault(p => p.Name == column.Name);
                if (prop == null)
                {
                    foreach (var complexProp in EntityType.ComplexProperties)
                        if (TryToAddSSDLColumnToComplexProperty(complexProp, 
                            () =>
                            {
                                if (ComplexMapping.ContainsKey(complexProp))
                                    return ComplexMapping[complexProp];
                                else
                                {
                                    var subComplexMapping = new ComplexPropertyMapping(EntityType, complexProp);
                                    ComplexMapping.Add(complexProp, subComplexMapping);
                                    return subComplexMapping;
                                }

                            }, column))
                            break;
                }
                else
                    AddMapping(prop, column);
            }
            OnPropertyChanged("IsCompletlyMapped");
        }
        private bool TryToAddSSDLColumnToComplexProperty(ComplexProperty complexProperty, Func<MappingBase> mapping, SSDL.Property.Property column)
        {
            var prop = complexProperty.ComplexType.ScalarProperties.FirstOrDefault(p => p.Name == column.Name);
            if (prop != null)
            {
                mapping().AddMapping(prop, column);
                return true;
            }
            foreach (var complexProp in complexProperty.ComplexType.ComplexProperties)
                if (TryToAddSSDLColumnToComplexProperty(complexProp, () =>
                    {
                        var complexMapping = mapping().ComplexMapping;
                        if (complexMapping.ContainsKey(complexProp))
                            return complexMapping[complexProp];
                        var complexPropMapping = new ComplexPropertyMapping(EntityType, complexProp);
                        complexMapping.Add(complexProp, complexPropMapping);
                        return complexPropMapping;
                    }, column))
                    return true;
            return false;
        }

        private void RemoveSSDLTable(SSDL.EntityType.EntityType ssdlTable)
        {
            if (MappingInit)
                return;

            foreach (var prop in Mapping.Keys.ToList())
            {
                var propMapping = Mapping[prop];
                if (propMapping.Count == 1)
                    Mapping.Remove(prop);
                else
                    propMapping.Remove(ssdlTable);
            }
            foreach (var complexProp in ComplexMapping.Keys.ToList())
                if (RemoveSSDLTableToComplexProperty(complexProp, ComplexMapping[complexProp], ssdlTable))
                    ComplexMapping.Remove(complexProp);

            OnPropertyChanged("IsCompletlyMapped");
        }

        private bool RemoveSSDLTableToComplexProperty(ComplexProperty complexProperty, MappingBase mapping, SSDL.EntityType.EntityType ssdlTable)
        {
            bool deleteAll = true;
            foreach (var prop in mapping.Mapping.Keys.ToList())
            {
                var propMapping = mapping.Mapping[prop];
                if (propMapping.Count == 1)
                    mapping.RemoveMapping(prop);
                else
                {
                    propMapping.Remove(ssdlTable);
                    deleteAll = false;
                }
            }
            foreach (var complexProp in mapping.ComplexMapping.Keys.ToList())
                if (RemoveSSDLTableToComplexProperty(complexProp, mapping.ComplexMapping[complexProp], ssdlTable))
                    mapping.ComplexMapping.Remove(complexProp);
                else
                    deleteAll = false;
            return deleteAll;
        }

        private bool MappingInit { get; set; }
        internal void BeginInit()
        {
            MappingInit = true;
        }
        internal void EndInit()
        {
            MappingInit = false;
        }

        public bool? IsCompletlyMapped
        {
            get
            {
                Func<MappingBase, TypeBase, bool> isCompletlyMapped = null;
                isCompletlyMapped = (mapping, type) =>
                    {
                        ComplexPropertyMapping complexMapping;
                        return type.ScalarProperties.All(sp => (mapping.Mapping.ContainsKey(sp) && mapping.Mapping[sp] != null) || ConditionsMapping.OfType<PropertyConditionMapping>().Any(pcm => pcm.CSDLProperty == sp)) && type.ComplexProperties.All(cp => mapping.ComplexMapping.ContainsKey(cp) && (complexMapping = mapping.ComplexMapping[cp]) != null && isCompletlyMapped(complexMapping, cp.ComplexType));
                    };
                return EntityType.Mapping.SSDLTables.Any() ? isCompletlyMapped(this, EntityType) : (EntityType.Abstract ? (bool?)null : false);
            }
        }

        public bool IsTPC
        {
            get 
            {
                if (EntityType.BaseType == null)
                    return false;
                var value = Mapping.Keys.Any(sp => !sp.IsKey && EntityType.BaseType.AllScalarProperties.Any(esp => esp == sp));
                if (value)
                    return true;
                foreach (var complexPropMapping in ComplexMapping.Values)
                    if (IsComplexTPC(complexPropMapping))
                        return true;
                return false;
            }
        }
        private bool IsComplexTPC(ComplexPropertyMapping complexPropertyMapping)
        {
            if (EntityType.BaseType == null)
                return false;
            if (EntityType.BaseType.AllComplexProperties.Any(ecp => ecp == complexPropertyMapping.ComplexProperty))
                return true;
            return false;
        }

        public void RemoveTPCMapping()
        {
            foreach (var scalarProp in Mapping.Keys.Where(sp => !sp.IsKey).Except(EntityType.ScalarProperties).ToList())
                Mapping.Remove(scalarProp);
            foreach (var complexProp in ComplexMapping.Keys.Except(EntityType.ComplexProperties).ToList())
                ComplexMapping.Remove(complexProp);
        }
    }
}
