// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
