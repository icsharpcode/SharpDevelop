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
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Mapping
{
    public partial class PropertyConditionComboBox : UserControl
    {
        public PropertyConditionComboBox()
        {
            InitializeComponent();
            propertyNullValues.Collection = NullValue.GetValues("<Delete>");
        }

        private ComboBoxSelectedValueBindingWithNull<ScalarProperty> _propertyComboBoxValue;
        public ComboBoxSelectedValueBindingWithNull<ScalarProperty> PropertyComboBoxValue
        {
            get
            {
                if (_propertyComboBoxValue == null)
                    _propertyComboBoxValue = new ComboBoxSelectedValueBindingWithNull<ScalarProperty>(Property,
                        property =>
                        {
                            ((PropertyConditionMapping)DataContext).CSDLProperty = property;
                            Property = property;
                            if (property == null)
                                OnDelete();
                        });
                return _propertyComboBoxValue;
            }
        }

        public EntityType EntityType
        {
            get { return (EntityType)GetValue(EntityTypeProperty); }
            set { SetValue(EntityTypeProperty, value); }
        }
        public static readonly DependencyProperty EntityTypeProperty =
            DependencyProperty.Register("EntityType", typeof(EntityType), typeof(PropertyConditionComboBox), new UIPropertyMetadata(null,
                (sender, e) =>
                {
                    var propertyConditionMapping = (PropertyConditionComboBox)sender;
                    propertyConditionMapping.properties.Collection = ((EntityType)e.NewValue).ScalarProperties;
                    propertyConditionMapping.propertyComboBox.GetBindingExpression(ComboBoxEditableWhenFocused.SelectedValueProperty).UpdateTarget();
                }));

        public ScalarProperty Property
        {
            get { return (ScalarProperty)GetValue(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }
        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.Register("Property", typeof(ScalarProperty), typeof(PropertyConditionComboBox), new UIPropertyMetadata(null,
                (sender, e) =>
                {
                    var propertyConditionMapping = (PropertyConditionComboBox)sender;
                    propertyConditionMapping._propertyComboBoxValue = null;
                    propertyConditionMapping.propertyComboBox.GetBindingExpression(ComboBoxEditableWhenFocused.SelectedValueProperty).UpdateTarget();
                }));

        protected virtual void OnDelete()
        {
            if (Deleted != null)
                Deleted((PropertyConditionMapping)DataContext);
        }
        public event Action<PropertyConditionMapping> Deleted;
    }
}
