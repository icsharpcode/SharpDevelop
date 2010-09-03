// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Windows;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer
{
    public interface ITypeDesigner
    {
        IUIType UIType { get; set; }
        double Left { get; set; }
        double Top { get; set; }
        bool IsExpanded { get; set; }

        event RoutedEventHandler Loaded;
    }
}
