// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Data.EDMDesigner.Core.UI.Helpers;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls
{
    public class ToolBoxExpander : Expander
    {
        static ToolBoxExpander()
        {
            ResourceDictionaryLoader.LoadResourceDictionary("/UserControls/ToolBoxExpanderResourceDictionary.xaml");
        }
        
        private RowDefinition _rowDefinition;
        private GridLength? _rowHeight;

        private RowDefinition RowDefinition
        {
            get
            {
                if (_rowDefinition == null)
                {
                    FrameworkElement parent = this;
                    while ((parent = parent.Parent as FrameworkElement) != null)
                    {
                        var grid = parent as Grid;
                        if (grid != null && grid.RowDefinitions.Any())
                        {
                            _rowDefinition = grid.RowDefinitions[Grid.GetRow(this)];
                            break;
                        }
                    }
                }
                return _rowDefinition;
            }
        }

        protected override void OnCollapsed()
        {
            _rowHeight = RowDefinition.Height;
            RowDefinition.Height = GridLength.Auto;
            base.OnCollapsed();
        }

        protected override void OnExpanded()
        {
            base.OnExpanded();
            if (_rowHeight.HasValue)
                RowDefinition.Height = _rowHeight.Value;
        }
    }
}
