#region Usings

using System;
using System.Windows;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.ContextMenu
{
    public class ContextMenu : System.Windows.Controls.ContextMenu, IMenuContainer
    {
        public ContextMenu()
        {
            Loaded +=
                delegate
                {
                    foreach (var item in Items)
                        MenuContainerBase.AddChild(item, () => OnItemVisibleChanged());
                };
        }
        protected override void AddChild(object value)
        {
            base.AddChild(value);
            MenuContainerBase.AddChild(value, () => OnItemVisibleChanged());
        }
        protected virtual void OnItemVisibleChanged()
        {
            if (ItemVisibleChanged != null)
                ItemVisibleChanged();
        }
        public event Action ItemVisibleChanged;
    }
}
