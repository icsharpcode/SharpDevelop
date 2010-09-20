// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Mapping
{
    public partial class Mapping : UserControl
    {
        public Mapping()
        {
            InitializeComponent();
        }

        public EDM EDM
        {
            get { return (EDM)GetValue(EDMProperty); }
            set { SetValue(EDMProperty, value); }
        }
        public static readonly DependencyProperty EDMProperty =
            DependencyProperty.Register("EDM", typeof(EDM), typeof(Mapping), new UIPropertyMetadata(null));

        private EntityType _entityType;
        public EntityType EntityType
        {
            get { return _entityType; }
            private set 
            { 
                _entityType = value;
                mappingTreeView.GetBindingExpression(Button.VisibilityProperty).UpdateTarget();
            }
        }

        public Association Association { get; private set; }

        public void BindEntityType(EntityType entityType)
        {
            EntityType = entityType;
            Association = null;
            if (entityType == null)
            {
                Visibility = Visibility.Collapsed;
                return;
            }
            nameTextBlock.Text = entityType.Name;
            Clear();
            Visibility = Visibility.Visible;
            foreach (var tableMapped in entityType.Mapping.MappedSSDLTables.ToList())
            {
                var tableMapping = AddNewEntityTableMapping();
                tableMapping.Tables = PossiblesTables.Union(new[] { tableMapped });
                tableMapping.TableComboBoxValue.ComboSelectedValue = tableMapped;
            }
        }

        public void BindAssociation(Association association)
        {
            Association = association;
            EntityType = null;
            if (association == null)
            {
                Visibility = Visibility.Collapsed;
                return;
            }
            nameTextBlock.Text = association.Name;
            Clear();
            Visibility = Visibility.Visible;
            var associationMapping = new AssociationMapping(association) { Tables = EDM.SSDLContainer.EntityTypes };
            associationMapping.TableComboBoxValue.ComboSelectedValue = association.Mapping.SSDLTableMapped;
            stackPanel.Children.Add(associationMapping);
        }

        private void Clear()
        {
            foreach (var uc in tablesTreeViewItem.Items.OfType<UserControl>().ToList())
                tablesTreeViewItem.Items.Remove(uc);
            foreach (var uc in stackPanel.Children.OfType<UserControl>().ToList())
                stackPanel.Children.Remove(uc);
        }

        public IEnumerable<ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType> PossiblesTables
        {
            get
            {
                return EDM.SSDLContainer.EntityTypes.Except(EntityType.Mapping.MappedSSDLTables);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var tableMapping = AddNewEntityTableMapping();
            tableMapping.Tables = PossiblesTables;
            tablesTreeViewItem.SizeChanged += delegate { tableMapping.Focus(); };
        }

        private EntityTableMapping AddNewEntityTableMapping()
        {
            var tableMapping = new EntityTableMapping(EntityType) { Focusable = false };
            tablesTreeViewItem.Items.Insert(tablesTreeViewItem.Items.Count - 1, tableMapping);
            tableMapping.TableChanged +=
                (oldTable, newTable) =>
                {
                    if (newTable == null)
                        tablesTreeViewItem.Items.Remove(tableMapping);
                };
            return tableMapping;
        }
    }
}
