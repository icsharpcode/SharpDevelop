// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
