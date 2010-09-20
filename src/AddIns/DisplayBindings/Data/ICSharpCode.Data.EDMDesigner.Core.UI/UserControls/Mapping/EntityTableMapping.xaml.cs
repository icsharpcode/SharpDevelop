// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Mapping
{
    public partial class EntityTableMapping : TableMapping, INotifyPropertyChanged
    {
        public EntityTableMapping(EntityType entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException();
            EntityType = entityType;
            InitializeComponent();
            tableNullValues.Collection = NullValue.GetValues("<Delete>");
        }

        protected override void SetTablesValues(IEnumerable<ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType> tables)
        {
            tablesValues.Collection = tables;
        }

        private EntityType _entityType;
        public EntityType EntityType
        {
            get { return _entityType; }
            private set
            {
                _entityType = value;
                if (_entityType != null)
                {
                    _entityType.Mapping.ConditionsMapping.CollectionChanged +=
                        delegate
                        {
                            columnConditionsGrid.ItemsSource = ColumnConditionsMapping;
                            propertyConditionsGrid.ItemsSource = PropertyConditionsMapping;
                        };
                    foreach (var propertyConditionMaping in PropertyConditionsMapping)
                        propertyConditionMaping.PropertyChanged += () => propertiesMappingControl.Mappings = PropertiesMapping;
                }
            }
        }

        private bool? _tpc;
        public bool TPC
        {
            get 
            {
                if (!_tpc.HasValue)
                {
                    _tpc = EntityType.Mapping.IsTPC;
                    Loaded += delegate { TPC = _tpc.Value; };
                }
                return _tpc.Value; 
            }
            set
            {
                _tpc = value;
                _propertiesMapping.TPC = value;
                propertiesMappingControl.TPC = value;
                complexPropertiesMappingControl.TPC = value;
                complexPropertiesMappingControl.GetBindingExpression(UserControl.VisibilityProperty).UpdateTarget();
            }
        }

        public bool HES { get; set; }

        protected override void TableValueChange(ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType table)
        {
            var oldTable = Table;
            base.TableValueChange(table);
            OnTableChanged(oldTable, table);
        }

        private ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.EntityPropertiesMapping _propertiesMapping;
        public ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.EntityPropertiesMapping PropertiesMapping
        {
            get
            {
                if (_propertiesMapping == null)
                    _propertiesMapping = new ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.EntityPropertiesMapping(EntityType, Table);
                return _propertiesMapping;
            }
        }

        private IEnumerable<ConditionMapping> ConditionsMapping
        {
            get { return EntityType.Mapping.ConditionsMapping.Where(ccm => ccm.Table == Table); }
        }
        public IEnumerable<ColumnConditionMapping> ColumnConditionsMapping
        {
            get
            {
                return ConditionsMapping.OfType<ColumnConditionMapping>();
            }
        }
        public bool ColumnConditionMappingVisible
        {
            get { return ColumnConditionsMapping.Any(); }
        }

        public IEnumerable<PropertyConditionMapping> PropertyConditionsMapping
        {
            get
            {
                return ConditionsMapping.OfType<PropertyConditionMapping>();
            }
        }
        public bool PropertyConditionMappingVisible
        {
            get { return HES && PropertyConditionsMapping.Any(); }
        }

        public override ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType Table
        {
            get { return base.Table; }
            set
            {
                if (value == null && Table != null)
                    EntityType.Mapping.SSDLTables.Remove(Table);
                base.Table = value;
                if (value != null)
                {
                    if (!EntityType.Mapping.SSDLTables.Any(ssdlTable => ssdlTable == value))
                        EntityType.Mapping.SSDLTables.Add(value);
                    _propertiesMapping = null;
                    _complexMappings = null;
                    propertiesMappingControl.GetBindingExpression(EntityPropertiesMapping.MappingsProperty).UpdateTarget();
                    complexPropertiesMappingControl.GetBindingExpression(ComplexPropertiesMapping.MappingsProperty).UpdateTarget();
                }
                OnPropertyChanged("Table");
                if (!HES)
                {
                    HES = EntityType.Mapping.ConditionsMapping.OfType<PropertyConditionMapping>().Any();
                    OnPropertyChanged("HES");
                }
            }
        }

        private void AddColumnConditionButton_Click(object sender, RoutedEventArgs e)
        {
            EntityType.Mapping.ConditionsMapping.Add(new ColumnConditionMapping { Table = Table });
            columnConditionsGrid.GetBindingExpression(Grid.VisibilityProperty).UpdateTarget();
        }

        private void AddPropertyConditionButton_Click(object sender, RoutedEventArgs e)
        {
            var propertyConditionMapping = new PropertyConditionMapping { Table = Table };
            propertyConditionMapping.PropertyChanged += () => propertiesMappingControl.Mappings = PropertiesMapping;
            EntityType.Mapping.ConditionsMapping.Add(propertyConditionMapping);
            propertyConditionsGrid.GetBindingExpression(Grid.VisibilityProperty).UpdateTarget();
        }

        public bool HasComplexMappings
        {
            get { return TPC ? EntityType.AllComplexProperties.Any() : EntityType.ComplexProperties.Any(); }
        }

        private ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.ComplexPropertiesMapping _complexMappings;
        public ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.ComplexPropertiesMapping ComplexMappings
        {
            get 
            { 
                if (_complexMappings == null)
                    _complexMappings = new ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.ComplexPropertiesMapping(EntityType, EntityType.Mapping, Table) { TPC = TPC };
                return _complexMappings;
            }
        }

        private void ColumnConditionMapping_ConditionDeleted(ColumnConditionMapping condition)
        {
            EntityType.Mapping.ConditionsMapping.Remove(condition);
            columnConditionsGrid.GetBindingExpression(Grid.VisibilityProperty).UpdateTarget();
        }

        private void PropertyConditionComboBox_Deleted(PropertyConditionMapping condition)
        {
            EntityType.Mapping.ConditionsMapping.Remove(condition);
            propertyConditionsGrid.GetBindingExpression(Grid.VisibilityProperty).UpdateTarget();
        }

        protected virtual void OnTableChanged(ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType oldTable, ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType newTable)
        {
            if (TableChanged != null)
                TableChanged(oldTable, newTable);
        }
        public event Action<ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType, ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType> TableChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
