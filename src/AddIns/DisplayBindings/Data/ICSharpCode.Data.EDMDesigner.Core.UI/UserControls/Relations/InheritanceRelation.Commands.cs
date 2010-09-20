// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Windows.Input;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations
{
    partial class InheritanceRelation
    {
        private static RoutedCommand _deleteCommand;
        public static RoutedCommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                    _deleteCommand = new RoutedCommand();
                return _deleteCommand;
            }
        }
        public static string DeleteText
        {
            get
            {
                return "Delete";
            }
        }

        private void InitContextMenuCommandBindings()
        {
            CommandBindings.Add(
                new CommandBinding(DeleteCommand,
                    delegate 
                    {
                        ((EntityType)FromTypeDesigner.UIType.BusinessInstance).BaseType = null;
                    }
                ));
        }

    }
}
