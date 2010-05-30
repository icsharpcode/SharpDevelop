// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
    [ExtensionFor(typeof(FrameworkElement))]
    [ExtensionServer(typeof(PrimarySelectionExtensionServer))]
    public class MarginHandleExtension : AdornerProvider
    {
        MarginHandle leftHandle, topHandle, rightHandle, bottomHandle;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (this.ExtendedItem.Parent != null)
            {
                if (this.ExtendedItem.Parent.ComponentType == typeof(Grid)){
                    FrameworkElement extendedControl = (FrameworkElement)this.ExtendedItem.Component;
                    AdornerPanel adornerPanel = new AdornerPanel();
                    
                    // If the Element is rotated/skewed in the grid, then margin handles do not appear
                    if (extendedControl.LayoutTransform.Value == Matrix.Identity && extendedControl.RenderTransform.Value == Matrix.Identity)
                    {
                        MarginHandle leftHandle = new MarginHandle(this.ExtendedItem, adornerPanel, HandleOrientation.Left);
                        MarginHandle topHandle = new MarginHandle(this.ExtendedItem, adornerPanel, HandleOrientation.Top);
                        MarginHandle rightHandle = new MarginHandle(this.ExtendedItem, adornerPanel, HandleOrientation.Right);
                        MarginHandle bottomHandle = new MarginHandle(this.ExtendedItem, adornerPanel, HandleOrientation.Bottom);
                    }
                    
                    if (adornerPanel != null)
                        this.Adorners.Add(adornerPanel);
                }
            }
        }
    }
}
   
