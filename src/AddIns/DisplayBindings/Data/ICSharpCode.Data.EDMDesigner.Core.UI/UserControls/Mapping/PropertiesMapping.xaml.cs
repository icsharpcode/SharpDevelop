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
