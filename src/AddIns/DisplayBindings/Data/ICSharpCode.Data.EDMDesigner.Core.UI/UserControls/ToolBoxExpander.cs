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
