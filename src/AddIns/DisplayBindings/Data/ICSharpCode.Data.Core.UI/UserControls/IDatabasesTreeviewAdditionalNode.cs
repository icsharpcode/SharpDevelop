#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

#endregion

namespace ICSharpCode.Data.Core.UI.UserControls
{
    public interface IDatabasesTreeViewAdditionalNode
    {
        string Name { get; }
        IEnumerable Items { get; }
        string DataTemplate { get; }
    }
}
