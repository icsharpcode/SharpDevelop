// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
