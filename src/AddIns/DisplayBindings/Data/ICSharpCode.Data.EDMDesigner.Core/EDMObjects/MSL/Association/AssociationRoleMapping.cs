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
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType;
using System.Collections.Specialized;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Common;
using System.Collections;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Association
{
    public class AssociationRoleMapping : EDMObjectBase, IMapping, IEnumerable<PropertyMapping>, INotifyCollectionChanged
    {
        #region Fields

        private Dictionary<ScalarProperty, SSDL.Property.Property> _mapping;

        #endregion

        #region Events

        public event Action<ScalarProperty, SSDL.Property.Property> Added;
        public event Action<ScalarProperty, SSDL.Property.Property, SSDL.Property.Property> Changed;
        public event Action<ScalarProperty, SSDL.Property.Property> Removed;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Property

        public NavigationProperty NavigationProperty { get; private set; }

        private Dictionary<ScalarProperty, SSDL.Property.Property> Mapping
        {
            get
            {
                if (_mapping == null)
                    _mapping = new Dictionary<ScalarProperty, SSDL.Property.Property>();
                return _mapping;
            }
        }

        public SSDL.Property.Property this[ScalarProperty scalarProperty]
        {
            get
            {
                if (_mapping.ContainsKey(scalarProperty))
                    return _mapping[scalarProperty];
                return null;
            }
            set
            {
                if (value == null)
                    Mapping.Remove(scalarProperty);
                else if (Mapping.ContainsKey(scalarProperty))
                    Mapping[scalarProperty] = value;
                else
                    Mapping.Add(scalarProperty, value);
            }
        }

        #endregion

        #region Constructor

        public AssociationRoleMapping(NavigationProperty navigationProperty)
        {
            NavigationProperty = navigationProperty;

            var associationMapping = navigationProperty.Association.Mapping;
            associationMapping.TableChanged += () =>
                {
                    var table = associationMapping.SSDLTableMapped;
                    foreach (var keyProp in NavigationProperty.EntityType.Keys.Where(k => !Mapping.ContainsKey(k)))
                    {
                        var column = table.Properties.FirstOrDefault(c => keyProp.Name == c.Name);
                        if (column != null)
                            AddMapping(keyProp, column);
                    }
                    var pms = this.ToList();
                    if (pms.Any())
                        foreach (var pm in pms)
                        {
                            var column = table.Properties.FirstOrDefault(c => pm.Property.Name == c.Name);
                            if (column != null)
                                ChangeMapping(pm.Property, column);
                            else
                                RemoveMapping(pm.Property);
                        }
                    else
                        OnCollectionChanged();
                };
        }

        #endregion

        #region Methods

        public IEnumerable<PropertyMapping> GetSpecificMappingForTable(SSDL.EntityType.EntityType table)
        {
            return this.Where(pm => pm.Column.EntityType == table);
        }

        public void AddMapping(ScalarProperty property, SSDL.Property.Property column)
        {
            if (property == null || column == null)
                throw new ArgumentNullException();

            if (Mapping.ContainsKey(property))
            {
                var oldColumn = Mapping[property];
                Mapping[property] = column;
                OnChanged(property, oldColumn, column);
            }
            else
            {
                Mapping.Add(property, column);
                OnAdded(property, column);
            }

            MappingAdded(column);
        }

        private void MappingAdded(SSDL.Property.Property column)
        {
            var associationMapping = NavigationProperty.Association.Mapping;
            associationMapping.OnIsCompletelyMappedChanged();
            if (NavigationProperty.Cardinality == Cardinality.ZeroToOne && !(associationMapping.MappingInit || associationMapping.ConditionsMapping.Any(ccm => ccm.Column == column && ccm.Operator == ConditionOperator.IsNotNull)))
                associationMapping.ConditionsMapping.Add(new ColumnConditionMapping { Column = column, Operator = ConditionOperator.IsNotNull });
        }

        public void ChangeMapping(ScalarProperty property, SSDL.Property.Property column)
        {
            if (property == null || column == null)
                throw new ArgumentNullException();

            if (!Mapping.ContainsKey(property))
                throw new InvalidOperationException();
            var oldColumn = Mapping[property];
            Mapping[property] = column;
            OnChanged(property, oldColumn, column);
            MappingAdded(column);
        }

        public void RemoveMapping(ScalarProperty property)
        {
            if (property == null)
                throw new ArgumentNullException();

            if (Mapping.ContainsKey(property))
            {
                var oldColumn = Mapping[property];
                Mapping.Remove(property);
                OnRemoved(property, oldColumn);
            }

            NavigationProperty.Association.Mapping.OnIsCompletelyMappedChanged();
        }

        void IMapping.RemoveMapping(ScalarProperty property, SSDL.EntityType.EntityType table)
        {
            RemoveMapping(property);
        }

        public IEnumerator<PropertyMapping> GetEnumerator()
        {
            return (Mapping.Keys.Select(k => new PropertyMapping { Property = k, Column = Mapping[k] })).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual void OnAdded(ScalarProperty property, SSDL.Property.Property column)
        {
            if (Added != null)
                Added(property, column);
            OnCollectionChanged();
        }

        protected virtual void OnChanged(ScalarProperty property, SSDL.Property.Property oldColumn, SSDL.Property.Property newColumn)
        {
            if (Changed != null)
                Changed(property, oldColumn, newColumn);
            OnCollectionChanged();
        }

        protected virtual void OnRemoved(ScalarProperty property, SSDL.Property.Property column)
        {
            if (Removed != null)
                Removed(property, column);
            OnCollectionChanged();
        }

        protected virtual void OnCollectionChanged()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion
    }
}
