// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Windows.Input;
using ICSharpCode.Data.EDMDesigner.Core.UI.DisplayBinding;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations
{
    partial class Association
    {
        private EDMDesignerViewContent Container
        {
            get
            {
                return FromTypeDesigner.Designer.Container;
            }
        }

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

        private static RoutedCommand _propertiesCommand;
        public static RoutedCommand PropertiesCommand
        {
            get
            {
                if (_propertiesCommand == null)
                    _propertiesCommand = new RoutedCommand();
                return _propertiesCommand;
            }
        }
        public static string PropertiesText
        {
            get
            {
                return "Properties";
            }
        }

        private static RoutedCommand _mappingCommand;
        public static RoutedCommand MappingCommand
        {
            get
            {
                if (_mappingCommand == null)
                    _mappingCommand = new RoutedCommand();
                return _mappingCommand;
            }
        }
        public static string MappingText
        {
            get
            {
                return "Mapping";
            }
        }

        private void InitContextMenuCommandBindings()
        {
            CommandBindings.AddRange(
                new[]
                {
                    new CommandBinding(DeleteCommand,
                        delegate 
                        {
                            CSDLAssociation.PropertyEnd1.EntityType.NavigationProperties.Remove(CSDLAssociation.PropertyEnd1);
                        }
                    ),
                    new CommandBinding(PropertiesCommand,
                        delegate
                        {
                            //Window.ShowPropertiesTab();
                        }
                    ),
                    new CommandBinding(MappingCommand,
                        delegate 
                        {
                            //Window.ShowMappingTab();
                        }
                    )
                });
        }
    }
}
