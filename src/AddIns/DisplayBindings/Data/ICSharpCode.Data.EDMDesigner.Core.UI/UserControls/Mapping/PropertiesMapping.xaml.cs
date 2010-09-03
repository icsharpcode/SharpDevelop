// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Mapping
{
    public partial class PropertiesMapping : UserControl
    {
        public PropertiesMapping()
        {
            InitializeComponent();
        }

        public ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.PropertiesMapping Mappings
        {
            get { return (ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.PropertiesMapping)GetValue(MappingsProperty); }
            set 
            { 
                SetValue(MappingsProperty, value);
                MappingEnumerable = value.Mappings;
            }
        }
        public static readonly DependencyProperty MappingsProperty =
            DependencyProperty.Register("Mappings", typeof(ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.PropertiesMapping), typeof(PropertiesMapping), new UIPropertyMetadata(null,
                (sender, e) =>
                {
                    var propertiesMapping = (PropertiesMapping)sender;
                    propertiesMapping.MappingEnumerable = ((ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.PropertiesMapping)e.NewValue).Mappings;
                }));

        public IEnumerable<ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.PropertyMapping> MappingEnumerable
        {
            get { return (IEnumerable<ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.PropertyMapping>)GetValue(MappingEnumerableProperty); }
            set 
            { 
                SetValue(MappingEnumerableProperty, value);
                propertiesGrid.ItemsSource = value;
            }
        }
        public static readonly DependencyProperty MappingEnumerableProperty =
            DependencyProperty.Register("MappingEnumerable", typeof(IEnumerable<ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.PropertyMapping>), typeof(PropertiesMapping), new UIPropertyMetadata(null, (sender, e) => ((PropertiesMapping)sender).propertiesGrid.ItemsSource = (IEnumerable)e.NewValue)); 
    }
}
