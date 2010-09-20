// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
