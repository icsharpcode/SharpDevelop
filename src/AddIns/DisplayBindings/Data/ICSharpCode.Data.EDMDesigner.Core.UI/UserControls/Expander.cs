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

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;
using System.Windows;
using System.Windows.Automation.Peers;
using ICSharpCode.Data.EDMDesigner.Core.UI.Helpers;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls
{
    public class Expander : System.Windows.Controls.Expander
    {
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (VisualTreeHelperUtil.GetControlAscendant<TextBox>(e.OriginalSource) == null || VisualTreeHelperUtil.GetControlAscendant<EditableTextBlock>(e.OriginalSource) == null)
            {
                if (!(e.OriginalSource is FrameworkElement))
                    return;

                FrameworkElement control = (FrameworkElement)e.OriginalSource;
                while ((control = VisualTreeHelperUtil.GetControlAscendant<Grid>(control)) != null)
                    if (control.Name == "grdHeader")
                    {
                        ClickOnTheHeader();
                        break;
                    }
            }
        }

        protected virtual void ClickOnTheHeader()
        {
            DesignerCanvas designerCanvas = VisualHelper.GetVisualParent<DesignerCanvas>(this);

            if (designerCanvas != null)
                designerCanvas.Container.Selection = this.DataContext;
        }
    }
}
