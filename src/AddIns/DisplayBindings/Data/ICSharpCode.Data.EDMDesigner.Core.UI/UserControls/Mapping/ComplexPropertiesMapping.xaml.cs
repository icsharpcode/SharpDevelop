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

using System.Linq;
using System.Windows.Controls;
using System.Windows;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Mapping
{
    public partial class ComplexPropertiesMapping : UserControl
    {
        public ComplexPropertiesMapping()
        {
            InitializeComponent();
        }

        public ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.ComplexPropertiesMapping Mappings
        {
            get { return (ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.ComplexPropertiesMapping)GetValue(MappingsProperty); }
            set { SetValue(MappingsProperty, value); }
        }
        public static readonly DependencyProperty MappingsProperty =
            DependencyProperty.Register("Mappings", typeof(ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL.ComplexPropertiesMapping), typeof(ComplexPropertiesMapping), new UIPropertyMetadata(null, (sender, e) => ((ComplexPropertiesMapping)sender).RefreshMappings())); 

        internal void RefreshMappings()
        {
            if (Mappings != null)
                complexPropertiesTreeViewItem.ItemsSource = Mappings.Mappings;
        }

        private bool _tpc;
        public bool TPC
        {
            get { return _tpc; }
            set
            {
                _tpc = value;
                Mappings.TPC = value;
                RefreshMappings();
            }
        }
    }
}
