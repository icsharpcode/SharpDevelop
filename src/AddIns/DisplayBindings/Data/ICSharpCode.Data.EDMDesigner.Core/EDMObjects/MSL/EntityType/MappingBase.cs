// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType;
using System.ComponentModel;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType
{
    public abstract class MappingBase : EDMObjectBase, IMapping, IEnumerable<KeyValuePair<SSDL.EntityType.EntityType, IEnumerable<PropertyMapping>>>, INotifyPropertyChanged
    {
        #region Fields

        private Dictionary<ScalarProperty, Dictionary<SSDL.EntityType.EntityType, SSDL.Property.Property>> _mapping;
        private Dictionary<ComplexProperty, ComplexPropertyMapping> _complexMapping;

        #endregion

        #region Constructor

        public MappingBase(CSDL.Type.EntityType entityType)
        {
            EntityType = entityType;
        }

        #endregion

        #region Properties

        public CSDL.Type.EntityType EntityType { get; private set; }

        internal Dictionary<ScalarProperty, Dictionary<SSDL.EntityType.EntityType, SSDL.Property.Property>> Mapping
        {
            get
            {
                if (_mapping == null)
                    _mapping = new Dictionary<ScalarProperty, Dictionary<SSDL.EntityType.EntityType, SSDL.Property.Property>>();
                return _mapping;
            }
        }

        internal Dictionary<SSDL.EntityType.EntityType, SSDL.Property.Property> this[ScalarProperty scalarProperty]
        {
            get
            {
                if (Mapping.ContainsKey(scalarProperty))
                    return Mapping[scalarProperty];
                return GetBaseMapping(scalarProperty);
            }
        }

        public SSDL.Property.Property this[ScalarProperty scalarProperty, SSDL.EntityType.EntityType table]
        {
            get
            {
                var columns = this[scalarProperty];
                if (columns == null)
                    return null;
                if (columns.ContainsKey(table))
                    return columns[table];
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (Mapping.ContainsKey(scalarProperty))
                        Mapping[scalarProperty].Remove(table);
                }
                else if (Mapping.ContainsKey(scalarProperty))
                {
                    var columns = Mapping[scalarProperty];
                    if (columns.ContainsKey(table))
                        columns[table] = value;
                    else
                    {
                        columns.Add(table, value);
                        AddTableMapped(table);
                    }
                }
                else
                {
                    Mapping.Add(scalarProperty, new Dictionary<SSDL.EntityType.EntityType, SSDL.Property.Property>() { { table, value } });
                    AddTableMapped(table);
                }
                EntityType.Mapping.OnPropertyChanged("IsCompletlyMapped");
            }
        }

        public IEnumerable<PropertyMapping> this[SSDL.EntityType.EntityType table]
        {
            get
            {
                return from csdlProp in Mapping.Keys
                       let ssdlProp = Mapping[csdlProp].FirstOrDefault(ssdlI => ssdlI.Key == table).Value
                       where ssdlProp != null
                       select new PropertyMapping { Property = csdlProp, Column = ssdlProp };
            }
        }

        protected internal Dictionary<ComplexProperty, ComplexPropertyMapping> ComplexMapping
        {
            get
            {
                if (_complexMapping == null)
                    _complexMapping = new Dictionary<ComplexProperty, ComplexPropertyMapping>();
                return _complexMapping;
            }
        }

        public ComplexPropertyMapping this[ComplexProperty complexProperty]
        {
            get
            {
                var value = GetMapping(complexProperty);
                if (value != null)
                    return value;
                value = new ComplexPropertyMapping(EntityType, complexProperty);
                ComplexMapping.Add(complexProperty, value);
                EntityType.Mapping.OnPropertyChanged("IsCompletlyMapped");
                return value;
            }
        }

        #endregion

        #region Methods

        protected virtual Dictionary<SSDL.EntityType.EntityType, SSDL.Property.Property> GetBaseMapping(ScalarProperty scalarProperty)
        {
            return null;
        }

        protected virtual void AddTableMapped(SSDL.EntityType.EntityType table)
        {
            EntityType.Mapping.OnPropertyChanged("IsCompletlyMapped");
        }

        #endregion


        internal virtual ComplexPropertyMapping GetMapping(ComplexProperty complexProperty)
        {
            return GetSpecificMapping(complexProperty);
        }
        public virtual ComplexPropertyMapping GetSpecificMapping(ComplexProperty complexProperty)
        {
            if (ComplexMapping.ContainsKey(complexProperty))
                return ComplexMapping[complexProperty];
            return null;
        }
        public virtual ComplexPropertyMapping GetSpecificMappingCreateIfNull(ComplexProperty complexProperty)
        {
            var value = GetSpecificMapping(complexProperty);
            if (value != null)
                return value;
            value = new ComplexPropertyMapping(EntityType, complexProperty);
            ComplexMapping.Add(complexProperty, value);
            EntityType.Mapping.OnPropertyChanged("IsCompletlyMapped");
            return value;
        }

        public IEnumerable<PropertyMapping> GetSpecificMappingForTable(SSDL.EntityType.EntityType table)
        {
            return from mapping in Mapping
                   where mapping.Value.Keys.Any(key => key == table)
                   select new PropertyMapping { Property = mapping.Key, Column = mapping.Value[table] };
        }
        public ComplexPropertyMapping GetEntityTypeSpecificComplexPropertyMapping(ComplexProperty complexProperty)
        {
            return ComplexMapping.ContainsKey(complexProperty) ? ComplexMapping[complexProperty] : null;
        }

        public void AddMapping(ScalarProperty property, SSDL.Property.Property column)
        {
            if (property == null || column == null)
                throw new ArgumentNullException();

            if (Mapping.ContainsKey(property))
            {
                var propertyMapping = Mapping[property];
                if (propertyMapping.ContainsKey(column.EntityType))
                    propertyMapping[column.EntityType] = column;
                else
                    propertyMapping.Add(column.EntityType, column);
            }
            else
                Mapping.Add(property, new Dictionary<SSDL.EntityType.EntityType, SSDL.Property.Property>() { { column.EntityType, column } });
            EntityType.Mapping.OnPropertyChanged("IsCompletlyMapped");
        }
        public void ChangeMapping(ScalarProperty property, SSDL.Property.Property column)
        {
            if (property == null || column == null)
                throw new ArgumentNullException();

            Dictionary<SSDL.EntityType.EntityType, SSDL.Property.Property> propertyMapping;
            if (!(Mapping.ContainsKey(property) && (propertyMapping = Mapping[property]).ContainsKey(column.EntityType)))
                throw new InvalidOperationException();
            propertyMapping[column.EntityType] = column;
            EntityType.Mapping.OnPropertyChanged("IsCompletlyMapped");
        }
        internal void RemoveMapping(ScalarProperty property)
        {
            if (property == null)
                throw new ArgumentNullException();

            if (!Mapping.ContainsKey(property))
                return;
            Mapping.Remove(property);
            EntityType.Mapping.OnPropertyChanged("IsCompletlyMapped");
        }
        public void RemoveMapping(ScalarProperty property, SSDL.EntityType.EntityType table)
        {
            if (property == null)
                throw new ArgumentNullException();

            if (!Mapping.ContainsKey(property))
                return;
            var propertyMapping = Mapping[property];
            switch (propertyMapping.Count)
            {
                case 0:
                    return;
                case 1:
                    if (propertyMapping.Keys.First() == table)
                        Mapping.Remove(property);
                    break;
                default:
                    if (propertyMapping.ContainsKey(table))
                        propertyMapping.Remove(table);
                    break;
            }
            EntityType.Mapping.OnPropertyChanged("IsCompletlyMapped");
        }

        public IEnumerable<SSDL.EntityType.EntityType> MappedSSDLTables
        {
            get
            {
                return Mapping.Values.Select(ssdlMappingsInfo => ssdlMappingsInfo.Keys).SelectMany(key => key).Distinct();
            }
        }

        public IEnumerable<PropertyMapping> GetMappingForTable(SSDL.EntityType.EntityType table)
        {
            var value = GetSpecificMappingForTable(table);
            var baseMapping = BaseMapping;
            if (baseMapping != null)
                value.Union(baseMapping.GetMappingForTable(table));
            return value;
        }
        protected abstract MappingBase BaseMapping { get; }

        public IEnumerator<KeyValuePair<SSDL.EntityType.EntityType, IEnumerable<PropertyMapping>>> GetEnumerator()
        {
            return (from table in MappedSSDLTables
                    select new KeyValuePair<SSDL.EntityType.EntityType, IEnumerable<PropertyMapping>>(table, GetMappingForTable(table))).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void OnIsCompletelyMappedChanged()
        {
            OnPropertyChanged("IsCompletlyMapped");
        }
    }
}
