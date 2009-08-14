#region Usings

using System.Windows.Controls;
using System.Windows.Data;
using ICSharpCode.Data.EDMDesigner.Core.UI.Converters;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.ContextMenu
{
    public class MenuSeparator : Separator
    {
        internal void SetVisibilityBinding()
        {
            var parent = Parent as IMenuContainer;
            if (parent != null)
                parent.ItemVisibleChanged += () => GetBindingExpression(VisibilityProperty).UpdateTarget();
            SetBinding(VisibilityProperty, new Binding { Source = this, Converter = new MenuSeparatorToMenuSeparatorVisibilityConverter() });
        }
    }
}
