#region Usings

using System;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.ContextMenu
{
    public class MenuItem : System.Windows.Controls.MenuItem, IMenuContainer
    {
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
