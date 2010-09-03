// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
