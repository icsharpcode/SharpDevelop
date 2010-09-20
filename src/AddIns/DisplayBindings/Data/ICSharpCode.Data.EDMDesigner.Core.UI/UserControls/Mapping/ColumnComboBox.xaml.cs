// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Mapping
{
    public abstract partial class ColumnComboBox : UserControl
    {
        public ColumnComboBox()
        {
            InitializeComponent();
            columnNullValues.Collection = NullValue.GetValues(NullValueText);
        }

        protected virtual string NullValueText
        {
            get { return "(none)"; }
        }

        private ComboBoxSelectedValueBindingWithNull<Property> _columnComboBoxValue;
        public ComboBoxSelectedValueBindingWithNull<Property> ColumnComboBoxValue
        {
            get
            {
                if (_columnComboBoxValue == null)
                    _columnComboBoxValue = new ComboBoxSelectedValueBindingWithNull<Property>(Column, column => OnColumnComboBoxValueChanged(column));
                return _columnComboBoxValue;
            }
        }

        protected abstract void OnColumnComboBoxValueChanged(Property column);

        public EntityType Table
        {
            get { return (EntityType)GetValue(TableProperty); }
            set { SetValue(TableProperty, value); }
        }
        public static readonly DependencyProperty TableProperty =
            DependencyProperty.Register("Table", typeof(EntityType), typeof(ColumnComboBox), new UIPropertyMetadata(null,
                (sender, e) =>
                {
                    var columnMapping = (ColumnComboBox)sender;
                    columnMapping.columns.Collection = ((EntityType)e.NewValue).Properties;
                    columnMapping.columnComboBox.GetBindingExpression(ComboBoxEditableWhenFocused.SelectedValueProperty).UpdateTarget();
                }));

        public Property Column
        {
            get { return (Property)GetValue(ColumnProperty); }
            set { SetValue(ColumnProperty, value); }
        }
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.Register("Column", typeof(Property), typeof(ColumnComboBox), new UIPropertyMetadata(null,
                (sender, e) =>
                {
                    var columnMapping = (ColumnComboBox)sender;
                    columnMapping._columnComboBoxValue = null;
                    columnMapping.columnComboBox.GetBindingExpression(ComboBoxEditableWhenFocused.SelectedValueProperty).UpdateTarget();
                }));
    }
}
