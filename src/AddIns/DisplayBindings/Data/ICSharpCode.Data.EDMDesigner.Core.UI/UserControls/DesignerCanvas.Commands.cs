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

using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ICSharpCode.Data.EDMDesigner.Core.UI.Windows;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;
using System.Windows.Media;
using ICSharpCode.Data.EDMDesigner.Core.UI.DisplayBinding;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls
{
    partial class DesignerCanvas
    {
        private const string CONTEXTMENU_RESOURCE_NAME = "ContextMenu";
        private const string ZOOM_STACKPANEL = "zoomStackPanel";
        private const string ZOOM_TEXTBOX = "zoomTextBox";

        private static RoutedCommand _addEntityTypeCommand;
        public static RoutedCommand AddEntityTypeCommand
        {
            get
            {
                if (_addEntityTypeCommand == null)
                    _addEntityTypeCommand = new RoutedCommand();
                return _addEntityTypeCommand;
            }
        }
        public static string AddEntityTypeText
        {
            get
            {
                return "Add entity type";
            }
        }

        private static RoutedCommand _addComplexTypeCommand;
        public static RoutedCommand AddComplexTypeCommand
        {
            get
            {
                if (_addComplexTypeCommand == null)
                    _addComplexTypeCommand = new RoutedCommand();
                return _addComplexTypeCommand;
            }
        }
        public static string AddComplexTypeText
        {
            get
            {
                return "Add complex type";
            }
        }

        private static RoutedCommand _addAssociationCommand;
        public static RoutedCommand AddAssociationCommand
        {
            get
            {
                if (_addAssociationCommand == null)
                    _addAssociationCommand = new RoutedCommand();
                return _addAssociationCommand;
            }
        }
        public static string AddAssociationText
        {
            get
            {
                return "Add association";
            }
        }

        private static RoutedCommand _removeFromDesignerCommand;
        public static RoutedCommand RemoveFromDesignerCommand
        {
            get
            {
                if (_removeFromDesignerCommand == null)
                    _removeFromDesignerCommand = new RoutedCommand();
                return _removeFromDesignerCommand;
            }
        }
        public static string RemoveFromDesignerText
        {
            get
            {
                return "Remove from designer";
            }
        }

        private static RoutedCommand _selectAllCommand;
        public static RoutedCommand SelectAllCommand
        {
            get
            {
                if (_selectAllCommand == null)
                    _selectAllCommand = new RoutedCommand();
                return _selectAllCommand;
            }
        }
        public static string SelectAllText
        {
            get
            {
                return "Select all";
            }
        }

        public static string ZoomText
        {
            get
            {
                return "Zoom";
            }
        }
        public int Zoom
        {
            get { return (int)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }
        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(int), typeof(DesignerCanvas),
                new UIPropertyMetadata(100,
                    (sender, e) =>
                    {
                        var designerCanvas = (DesignerCanvas)sender;
                        if ((int)e.NewValue < 10)
                            designerCanvas.Zoom = 10;
                        else if ((int)e.NewValue > 1000)
                            designerCanvas.Zoom = 1000;
                        else
                        {
                            int zoom = (int)e.NewValue;
                            designerCanvas.DesignerView.Zoom = zoom;
                            var scale = (double)zoom / 100.0;
                            designerCanvas.LayoutTransform = new ScaleTransform(scale, scale);
                            designerCanvas.Resize();
                        }
                    }));

        public new InputBindingCollection InputBindings
        {
            get { return base.InputBindings; }
            set { }
        }
        public static readonly DependencyProperty InputBindingsProperty = DependencyProperty.RegisterAttached("InputBindings", typeof(InputBindingCollection), typeof(DesignerCanvas), new PropertyMetadata((sender, e) => ((DesignerCanvas)sender).InputBindings.AddRange((InputBindingCollection)e.NewValue)));

        private EDMDesignerViewContent _container;
        internal EDMDesignerViewContent Container
        {
            get
            {
                return _container;
            }
        }

        private new ContextMenu.ContextMenu ContextMenu
        {
            get
            {
                return Style == null ? (ContextMenu.ContextMenu)null : (ContextMenu.ContextMenu)Style.Resources.OfType<DictionaryEntry>().FirstOrDefault(de => ((string)de.Key) == "ContextMenu").Value;
            }
        }

        private void InitContextMenuCommandBindings()
        {
            Loaded +=
                delegate
                {
                    ContextMenu.ContextMenu contextMenu;
                    StackPanel stackPanel;
                    TextBox textBox;
                    if ((contextMenu = ContextMenu) != null && (stackPanel = contextMenu.Items.OfType<StackPanel>().FirstOrDefault(sp => sp.Name == ZOOM_STACKPANEL)) != null && (textBox = stackPanel.Children.OfType<TextBox>().FirstOrDefault(etb => etb.Name == ZOOM_TEXTBOX)) != null)
                        textBox.SetBinding(TextBox.TextProperty, new Binding { Source = this, Path = new PropertyPath("Zoom") });
                };

            CommandBindings.AddRange(
                new[]
                {
                    new CommandBinding(AddEntityTypeCommand, 
                        delegate
                        {
                            var csdlView = EDMView.CSDL;
                            var addEntityTypeWindow = new AddEntityTypeWindow(csdlView) { Owner = Container.Window };
                            switch (addEntityTypeWindow.ShowDialog())
                            {
                                case true:
                                    UITypeToAdd = csdlView.AddEntityType(addEntityTypeWindow.EntityTypeName, addEntityTypeWindow.BaseEntityType);
                                break;
                            }
                        }
                    ),
                    new CommandBinding(AddComplexTypeCommand, 
                        delegate
                        {
                            var addComplexTypeWindow = new AddComplexTypeWindow { Owner = Container.Window };
                            switch (addComplexTypeWindow.ShowDialog())
                            {
                                case true:
                                    UITypeToAdd = EDMView.CSDL.AddComplexType(addComplexTypeWindow.ComplexTypeName);
                                break;
                            }
                        }
                    ),
                    new CommandBinding(AddAssociationCommand, 
                        AddAssociationExecuted(this, () => EDMView.CSDL, () => DesignerView, null, null)
                    ),
                    new CommandBinding(RemoveFromDesignerCommand, 
                        delegate { RemoveFromDesigner(); },
                        (sender, e) => e.CanExecute = TypesSelected.Any()
                    ),
                    new CommandBinding(SelectAllCommand, 
                        delegate
                        {
                            foreach(var types in TypesVisibles)
                                types.IsSelected = true;
                        },
                        (sender, e) => e.CanExecute = TypesVisibles.Any()
                    )
                }
            );
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            if (e.LeftButton == MouseButtonState.Pressed)
                OnMouseLeftButtonDown(e);
            else
                OnMouseRightButtonDown(e);
            e.Handled = true;
        }

        public static ExecutedRoutedEventHandler AddAssociationExecuted(UIElement parent, Func<CSDLView> getCSDLView, Func<DesignerView> getDesignerView, UIEntityType defaultEntityType1, Action<UIAssociation> specialActionToDoWhenNewItemAdded)
        {
            return (object sender, ExecutedRoutedEventArgs e) =>
             {
                 var csdlView = getCSDLView();
                 var designerView = getDesignerView();
                 var addAssociationWindow = new AddAssociationWindow { Owner = VisualTreeHelperUtil.GetControlAscendant<Window>(parent), DataContext = csdlView, NavigationProperty1EntityType = defaultEntityType1 };
                 switch (addAssociationWindow.ShowDialog())
                 {
                     case true:
                         var uiAssociation = csdlView.AddAssociation(addAssociationWindow.AssociationName, addAssociationWindow.NavigationProperty1Name, addAssociationWindow.NavigationProperty1EntityType, addAssociationWindow.NavigationProperty1Cardinality.Value, addAssociationWindow.NavigationProperty2Name, addAssociationWindow.NavigationProperty2EntityType, addAssociationWindow.NavigationProperty2Cardinality.Value);
                         SizeChangedEventHandler actionToDoWhenNewItemAdded = null;
                         bool drawAssociation = false;
                         DesignerCanvas designerCanvas = parent as DesignerCanvas;
                         if (designerCanvas == null)
                             designerCanvas = VisualTreeHelperUtil.GetControlAscendant<DesignerCanvas>(parent);
                         var navigationProperty1EntityTypeDesigner = designerCanvas.GetTypeDesigner(uiAssociation.NavigationProperty1.ParentType);
                         var navigationProperty2EntityTypeDesigner = designerCanvas.GetTypeDesigner(uiAssociation.NavigationProperty2.ParentType);
                         actionToDoWhenNewItemAdded =
                             (object o, SizeChangedEventArgs scea) =>
                             {
                                 if (designerView.ContainsEntityType(uiAssociation.NavigationProperty1.ParentType) && designerView.ContainsEntityType(uiAssociation.NavigationProperty2.ParentType))
                                 {
                                     if (drawAssociation)
                                         navigationProperty1EntityTypeDesigner.DrawAssociation(navigationProperty2EntityTypeDesigner, uiAssociation.NavigationProperty1.BusinessInstance as NavigationProperty);
                                     else
                                         drawAssociation = true;
                                 }
                                 if (specialActionToDoWhenNewItemAdded != null)
                                     specialActionToDoWhenNewItemAdded(uiAssociation);
                                 ((ListView)o).SizeChanged -= actionToDoWhenNewItemAdded;
                             };
                         navigationProperty1EntityTypeDesigner.propertiesListView.SizeChanged += actionToDoWhenNewItemAdded;
                         if (navigationProperty2EntityTypeDesigner != null)
                             navigationProperty2EntityTypeDesigner.propertiesListView.SizeChanged += actionToDoWhenNewItemAdded;
                         break;
                 }
             };
        }
    }
}
