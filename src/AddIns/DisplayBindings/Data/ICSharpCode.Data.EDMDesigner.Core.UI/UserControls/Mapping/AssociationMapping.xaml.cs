// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Mapping
{
    public partial class AssociationMapping : TableMapping
    {
        public AssociationMapping(Association association)
        {
            if (association == null)
                throw new ArgumentNullException();
            Association = association;
            InitializeComponent();
        }

        private Association _association;
        public Association Association
        {
            get { return _association; }
            private set
            {
                _association = value;
                if (_association != null)
                    _association.Mapping.ConditionsMapping.CollectionChanged +=
                        delegate { columnConditionsGrid.GetBindingExpression(DataGrid.VisibilityProperty).UpdateTarget(); };
            }
        }

        protected override void SetTablesValues(IEnumerable<EntityType> tables)
        {
            tablesComboBox.ItemsSource = tables;
        }

        public IEnumerable<ColumnConditionMapping> ColumnConditionsMapping
        {
            get
            {
                return Association.Mapping.ConditionsMapping;
            }
        }
        public bool ColumnConditionMappingVisible
        {
            get { return ColumnConditionsMapping.Any(); }
        }

        private void AddColumnConditionButton_Click(object sender, RoutedEventArgs e)
        {
            Association.Mapping.ConditionsMapping.Add(new ColumnConditionMapping { Table = Table });
            columnConditionsGrid.GetBindingExpression(Grid.VisibilityProperty).UpdateTarget();
        }

        private void ColumnConditionMapping_ConditionDeleted(ColumnConditionMapping condition)
        {
            Association.Mapping.ConditionsMapping.Remove(condition);
            columnConditionsGrid.GetBindingExpression(Grid.VisibilityProperty).UpdateTarget();
        }

        private AssociationPropertiesMapping _propertiesMapping1;
        public AssociationPropertiesMapping PropertiesMapping1
        {
            get
            {
                if (_propertiesMapping1 == null)
                {
                    _propertiesMapping1 = new AssociationPropertiesMapping(Association.PropertyEnd1, Table);
                    Association.PropertyEnd1.Mapping.CollectionChanged += delegate
                    {
                        _propertiesMapping1 = null;
                        propertiesMappingNavigationProperty1.GetBindingExpression(EntityPropertiesMapping.MappingsProperty).UpdateTarget();
                    };
                }
                return _propertiesMapping1;
            }
        }

        private AssociationPropertiesMapping _propertiesMapping2;
        public AssociationPropertiesMapping PropertiesMapping2
        {
            get
            {
                if (_propertiesMapping2 == null)
                {
                    _propertiesMapping2 = new AssociationPropertiesMapping(Association.PropertyEnd2, Table);
                    Association.PropertyEnd2.Mapping.CollectionChanged += delegate
                    {
                        _propertiesMapping2 = null;
                        propertiesMappingNavigationProperty2.GetBindingExpression(EntityPropertiesMapping.MappingsProperty).UpdateTarget();
                    };
                }
                return _propertiesMapping2;
            }
        }

        protected override void TableValueChange(EntityType table)
        {
            base.TableValueChange(table);
            Association.Mapping.SSDLTableMapped = table;
        }
    }
}
