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
