using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.ShortcutsManagement.Data
{
    public interface IShortcutTreeEntry : IComparable, ICloneable
    {
        string Name
        {
            get; set;
        }

        bool IsVisible
        {
            get; set;
        }

        void SortSubEntries();

        Shortcut FindShortcut(string shortcutId);
    }
}
