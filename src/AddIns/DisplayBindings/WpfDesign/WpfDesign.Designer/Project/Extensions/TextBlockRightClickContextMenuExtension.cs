using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
    [ExtensionServer(typeof (OnlyOneItemSelectedExtensionServer))]
    [ExtensionFor(typeof (TextBlock))]
    public class TextBlockRightClickContextMenuExtension : PrimarySelectionAdornerProvider
    {
        DesignPanel panel;
        ContextMenu contextMenu;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            contextMenu = new TextBlockRightClickContextMenu(ExtendedItem);
            panel = ExtendedItem.Context.Services.DesignPanel as DesignPanel;
            panel.AddContextMenu(contextMenu);            
        }

        protected override void OnRemove()
        {
            panel.RemoveContextMenu(contextMenu);

            base.OnRemove();
        }
    }
}
