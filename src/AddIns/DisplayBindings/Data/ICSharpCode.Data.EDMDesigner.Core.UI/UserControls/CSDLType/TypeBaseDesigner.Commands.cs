// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.ContextMenu;
using ICSharpCode.Data.EDMDesigner.Core.UI.Windows;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Association;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType
{
    partial class TypeBaseDesigner
    {
        private const string CONTEXTMENU_RESOURCE_NAME = "ContextMenu";
        private const string MENUITEM_ADD = "addMenuItem";
        private const string MENUITEM_ADD_SCALAR_PROPERTY = "addScalarPropertyMenuItem";
        private const string MENUITEM_ADD_COMPLEX_PROPERTY = "addComplexPropertyMenuItem";
        private const string MENUITEM_ADD_NAVIGATION_PROPERTY = "addNavigationPropertyMenuItem";
        private const string MENUITEM_RENAME = "renameMenuItem";
        private const string MENUITEM_DELETE = "deleteMenuItem";
        private const string MENUITEM_REMOVE_FROM_DESIGNER = "removeFromDesignerMenuItem";
        private const string MENUITEM_CUT = "cutMenuItem";
        private const string MENUITEM_COPY = "copyMenuItem";
        private const string MENUITEM_PASTE = "pasteMenuItem";
        private const string MENUITEM_PROPERTIES = "propertiesMenuItem";
        private const string MENUITEM_MAPPING = "mappingMenuItem";
        private const string MENUITEM_SHOW_BASE_TYPE = "showBaseMenuItem";
        private const string MENUITEM_SHOW_RELATED_TYPE = "showRelatedMenuItem";
        private const string MENUITEM_SHOW_OTHER_TABS = "showOtherTabsMenuItem";

        public static string AddPropertyText
        {
            get
            {
                return "Add";
            }
        }

        private static RoutedCommand _addScalarPropertyCommand;
        public static RoutedCommand AddScalarPropertyCommand
        {
            get
            {
                if (_addScalarPropertyCommand == null)
                    _addScalarPropertyCommand = new RoutedCommand();
                return _addScalarPropertyCommand;
            }
        }
        public static string AddScalarPropertyText
        {
            get
            {
                return "Add scalar property";
            }
        }

        private static RoutedCommand _addComplexPropertyCommand;
        public static RoutedCommand AddComplexPropertyCommand
        {
            get
            {
                if (_addComplexPropertyCommand == null)
                    _addComplexPropertyCommand = new RoutedCommand();
                return _addComplexPropertyCommand;
            }
        }
        public static string AddComplexPropertyText
        {
            get
            {
                return "Add complex property";
            }
        }

        private static RoutedCommand _addNavigationPropertyCommand;
        public static RoutedCommand AddNavigationPropertyCommand
        {
            get
            {
                if (_addNavigationPropertyCommand == null)
                    _addNavigationPropertyCommand = new RoutedCommand();
                return _addNavigationPropertyCommand;
            }
        }
        public static string AddNavigationPropertyText
        {
            get
            {
                return "Add navigation property";
            }
        }

        private static RoutedCommand _renameCommand;
        public static RoutedCommand RenameCommand
        {
            get
            {
                if (_renameCommand == null)
                    _renameCommand = new RoutedCommand();
                return _renameCommand;
            }
        }
        public static string RenameText
        {
            get
            {
                return "Rename";
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

        private static RoutedCommand _cutCommand;
        public static RoutedCommand CutCommand
        {
            get
            {
                if (_cutCommand == null)
                    _cutCommand = new RoutedCommand();
                return _cutCommand;
            }
        }
        public static string CutText
        {
            get
            {
                return "Cut";
            }
        }

        private static RoutedCommand _copyCommand;
        public static RoutedCommand CopyCommand
        {
            get
            {
                if (_copyCommand == null)
                    _copyCommand = new RoutedCommand();
                return _copyCommand;
            }
        }
        public static string CopyText
        {
            get
            {
                return "Copy";
            }
        }

        private static RoutedCommand _pasteCommand;
        public static RoutedCommand PasteCommand
        {
            get
            {
                if (_pasteCommand == null)
                    _pasteCommand = new RoutedCommand();
                return _pasteCommand;
            }
        }
        public static string PasteText
        {
            get
            {
                return "Paste";
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

        private static RoutedCommand _showBaseTypeCommand;
        public static RoutedCommand ShowBaseTypeCommand
        {
            get
            {
                if (_showBaseTypeCommand == null)
                    _showBaseTypeCommand = new RoutedCommand();
                return _showBaseTypeCommand;
            }
        }
        public static string ShowBaseText
        {
            get
            {
                return "Show base type";
            }
        }

        private static RoutedCommand _showRelatedTypeCommand;
        public static RoutedCommand ShowRelatedTypeCommand
        {
            get
            {
                if (_showRelatedTypeCommand == null)
                    _showRelatedTypeCommand = new RoutedCommand();
                return _showRelatedTypeCommand;
            }
        }
        public static string ShowRelatedText
        {
            get
            {
                return "Show related type";
            }
        }

        private static RoutedCommand _showOtherTabsCommand;
        public static RoutedCommand ShowOtherTabsCommand
        {
            get
            {
                if (_showOtherTabsCommand == null)
                    _showOtherTabsCommand = new RoutedCommand();
                return _showOtherTabsCommand;
            }
        }
        public static string ShowOtherTabsText
        {
            get
            {
                return "Show other tabs";
            }
        }

        private static RoutedCommand _showOtherTabsItemCommand;
        public static RoutedCommand ShowOtherTabsItemCommand
        {
            get
            {
                if (_showOtherTabsItemCommand == null)
                    _showOtherTabsItemCommand = new RoutedCommand();
                return _showOtherTabsItemCommand;
            }
        }

        private bool _groupRightClick;

        private new ContextMenu.ContextMenu ContextMenu
        {
            get
            {
                return FindResource(CONTEXTMENU_RESOURCE_NAME) as ContextMenu.ContextMenu;
            }
        }

        private void InitContextMenuCommandBindings()
        {
            CommandBindings.AddRange(
                new[]
                {
                    new CommandBinding(AddScalarPropertyCommand,
                        delegate
                        {
                            var addScalarPropertyWindow = new AddScalarPropertyWindow{ Owner = Application.Current.MainWindow};
                            switch (addScalarPropertyWindow.ShowDialog())
                            {
                                case true:
                                    var uiScalarProperty = UIType.AddScalarProperty(addScalarPropertyWindow.PropertyName, addScalarPropertyWindow.PropertyType.Value);
                                    SizeChangedEventHandler actionToDoWhenNewItemAdded = null;
                                    actionToDoWhenNewItemAdded = 
                                        delegate
                                        {
                                            propertiesListView.SelectedValue = uiScalarProperty;
                                            GetListViewItem(uiScalarProperty.BusinessInstance as ScalarProperty).Focus();
                                            propertiesListView.SizeChanged -= actionToDoWhenNewItemAdded;
                                        };
                                    propertiesListView.SizeChanged += actionToDoWhenNewItemAdded;
                                    break;
                            }
                        },
                        (sender, e) => e.CanExecute = ApplyNotVisibleIfMoreThanOneTypeSelected(ContextMenu.GetMenuItem(MENUITEM_ADD, MENUITEM_ADD_SCALAR_PROPERTY))
                    ),
                    new CommandBinding(AddComplexPropertyCommand,
                        delegate
                        {
                            var addComplexPropertyWindow = new AddComplexPropertyWindow(ComplexTypesAllowedForComplexProperty) { Owner = Application.Current.MainWindow, DataContext = UIType.View};
                            switch (addComplexPropertyWindow.ShowDialog())
                            {
                                case true:
                                    var uiComplexType = addComplexPropertyWindow.PropertyType;
                                    var businessComplexType = uiComplexType.BusinessInstance;
                                    var uiComplexProperty = UIType.AddComplexProperty(addComplexPropertyWindow.PropertyName, businessComplexType);
                                    DrawComplexPropertyRelationAfterAddedComplexProperty(uiComplexType, uiComplexProperty);
                                    break;
                            }
                        },
                        (sender, e) => e.CanExecute = ApplyNotVisibleIfMoreThanOneTypeSelected(ContextMenu.GetMenuItem(MENUITEM_ADD, MENUITEM_ADD_COMPLEX_PROPERTY)) && ComplexTypesAllowedForComplexProperty.Any()
                    ),
                    new CommandBinding(RenameCommand,
                        (sender, e) => Rename(e.OriginalSource),
                        (sender, e) =>
                        {
                            var renameMenuItem = ContextMenu.GetMenuItem(MENUITEM_RENAME);
                            if (SetVisibility(renameMenuItem, () => ApplyNotVisibleIfMoreThanOneTypeSelected(renameMenuItem) && ! _groupRightClick))
                                e.CanExecute = (e.OriginalSource == this || propertiesListView.SelectedItems.Count == 1);
                            else
                                e.CanExecute = false;
                        }
                    ),
                    new CommandBinding(DeleteCommand,
                        (sender, e) => Delete(e.OriginalSource),
                        (sender, e) => 
                        {
                            var deleteMenuItem = ContextMenu.GetMenuItem(MENUITEM_DELETE);
                            if (SetVisibility(deleteMenuItem, () => AllowDelete(deleteMenuItem)))
                                e.CanExecute = true;
                            else
                                e.CanExecute = false;
                        }
                    ),
                    new CommandBinding(DesignerCanvas.RemoveFromDesignerCommand, 
                        delegate { Designer.RemoveFromDesigner(); },
                        (sender, e) => e.CanExecute = SetVisibility(ContextMenu.GetMenuItem(MENUITEM_REMOVE_FROM_DESIGNER), () => ! PropertiesTreeViewFocused)
                    ),
                    new CommandBinding(CutCommand,
                        delegate { CutSelectedProperties(); },
                        (sender, e) => e.CanExecute = ApplyCutCopyVisible(ContextMenu.GetMenuItem(MENUITEM_CUT))
                    ),
                    new CommandBinding(CopyCommand,
                        delegate { CopySelectedProperties(); },
                        (sender, e) => e.CanExecute = ApplyCutCopyVisible(ContextMenu.GetMenuItem(MENUITEM_COPY))
                    ),
                    new CommandBinding(PasteCommand,
                        delegate 
                        {
                            PasteClipboardProperties();
                        },
                        (sender, e) => e.CanExecute = ApplyPasteVisible() && CanPaste()
                    ),
                    //new CommandBinding(PropertiesCommand,
                    //    delegate
                    //    {
                    //        var focusedObject = VisualTreeHelperUtil.GetFocusedElement(this);
                    //        if (VisualTreeHelperUtil.IsAscendant(propertiesListView, focusedObject))
                    //            Designer.Container.ShowPropertiesTab((UIProperty)propertiesListView.SelectedItem);
                    //        else
                    //            Designer.Container.ShowPropertiesTab(UIType);
                    //    },
                    //    (sender, e) =>
                    //    {
                    //        var propertiesMenuItem = ContextMenu.GetMenuItem(MENUITEM_PROPERTIES);
                    //        if (SetVisibility(propertiesMenuItem, () => ApplyNotVisibleIfMoreThanOneTypeSelected(propertiesMenuItem) && !_groupRightClick && (!PropertiesTreeViewFocused || propertiesListView.SelectedItems.Count == 1)))
                    //            e.CanExecute = true;
                    //        else
                    //            e.CanExecute = false;
                    //    }
                    //),
                    new CommandBinding(MappingCommand,
                        delegate 
                        {
                            var focusedObject = VisualTreeHelperUtil.GetFocusedElement(this);
                            Designer.Container.ShowMappingTab(UIType);
                        },
                        (sender, e) =>
                        {
                            var mappingMenuItem = ContextMenu.GetMenuItem(MENUITEM_MAPPING);
                            if (SetVisibility(mappingMenuItem, () => ApplyNotVisibleIfMoreThanOneTypeSelected(mappingMenuItem) && !_groupRightClick && (!PropertiesTreeViewFocused || propertiesListView.SelectedItems.Count == 1)))
                                e.CanExecute = true;
                            else
                                e.CanExecute = false;
                        }
                    ),
                    new CommandBinding(ShowBaseTypeCommand,
                        delegate { Designer.UITypeToAdd = Designer.EDMView.CSDL.EntityTypes[((EntityType)UIType.BusinessInstance).BaseType]; },
                        (sender, e) =>
                        {
                            var showBaseMenuItem = ContextMenu.GetMenuItem(MENUITEM_SHOW_BASE_TYPE);
                            var entityType = UIType.BusinessInstance as EntityType;
                            if (SetVisibility(showBaseMenuItem, () => ApplyNotVisibleIfMoreThanOneTypeSelected(showBaseMenuItem) && !_groupRightClick && (!PropertiesTreeViewFocused || propertiesListView.SelectedItems.Count == 1) && entityType != null && entityType.BaseType != null))
                                e.CanExecute = ! Designer.DesignerView.ContainsEntityType(entityType.BaseType);
                            else
                                e.CanExecute = false;
                        }
                    ),
                    new CommandBinding(ShowRelatedTypeCommand,
                        delegate { Designer.UITypeToAdd = ((UIRelatedProperty)propertiesListView.SelectedItem).RelatedType; },
                        (sender, e) =>
                        {
                            var showRelatedTypeMenuItem = ContextMenu.GetMenuItem(MENUITEM_SHOW_RELATED_TYPE);
                            if (SetVisibility(showRelatedTypeMenuItem, () => ApplyNotVisibleIfMoreThanOneTypeSelected(showRelatedTypeMenuItem) && propertiesListView.SelectedItems.Count == 1 && propertiesListView.SelectedItem is UIRelatedProperty && !_groupRightClick))
                                e.CanExecute = !Designer.DesignerView.ContainsEntityType(((UIRelatedProperty)propertiesListView.SelectedItem).RelatedType);
                            else
                                e.CanExecute = false;
                        }
                    ),
                    new CommandBinding(ShowOtherTabsCommand,
                        delegate { },
                        (sender, e) =>
                        {
                            var showOtherTabsMenuItem = ContextMenu.GetMenuItem(MENUITEM_SHOW_OTHER_TABS);
                            var designerViews = Designer.EDMView.DesignerViews.Except(new []{ Designer.DesignerView}).Where(dv => dv.TypeDesignersLocations.Any(tdl => tdl.UIType.BusinessInstance == UIType.BusinessInstance));
                            e.CanExecute = designerViews.Any();
                            if (e.CanExecute)
                                ShowDesignerCanvasPreviews(Designer.EDMView, UIType, showOtherTabsMenuItem, designerViews);
                            else
                                showOtherTabsMenuItem.Items.Clear();
                        }
                    )
                    //new CommandBinding(ShowOtherTabsItemCommand, (sender, e) => ((IEDMDesignerWindow)VisualTreeHelperUtil.GetControlAscendant<Window>(this)).CurrentDesignerView = (DesignerView)e.Parameter)
                }
            );
            var uiEntityType = UIType as UIEntityType;
            if (uiEntityType == null)
            {
                MenuItem addNavigationPropertyMenuItem = ContextMenu.GetMenuItem(MENUITEM_ADD, MENUITEM_ADD_NAVIGATION_PROPERTY);
                if (addNavigationPropertyMenuItem != null)
                    addNavigationPropertyMenuItem.Visibility = Visibility.Collapsed;
            }
            else
            {
                CommandBindings.Add(new CommandBinding(AddNavigationPropertyCommand,
                    DesignerCanvas.AddAssociationExecuted(this, () => UIType.View, () => Designer.DesignerView, uiEntityType,
                        (UIAssociation uiAssociation) =>
                        {
                            if (uiAssociation.NavigationProperty1.ParentType == UIType)
                            {
                                propertiesListView.SelectedValue = uiAssociation.NavigationProperty1;
                                GetListViewItem(uiAssociation.NavigationProperty1.BusinessInstance).Focus();
                            }
                        }
                    ),
                    (sender, e) => e.CanExecute = ApplyNotVisibleIfMoreThanOneTypeSelected(ContextMenu.GetMenuItem(MENUITEM_ADD, MENUITEM_ADD_NAVIGATION_PROPERTY))
                ));
            }

            ContextMenuOpening +=
                (sender, e) =>
                {
                    if (MoreThanOneTypeSelected && (_groupRightClick || PropertiesTreeViewFocused))
                        e.Handled = true;
                    ApplyNotVisibleIfMoreThanOneTypeSelected(ContextMenu.GetMenuItem(MENUITEM_ADD));
                };
        }

        public static void ShowDesignerCanvasPreviews(EDMView edmView, IUIType uiType, MenuItem showOtherTabsMenuItem, IEnumerable<DesignerView> designerViews)
        {
            if (showOtherTabsMenuItem.ItemsSource == null)
                showOtherTabsMenuItem.Items.Add(new MenuItem());
            var designerCanvasPreviews = new List<DesignerCanvasPreview>();
            Action<DesignerCanvasPreview> designerCanvasPreviewCreated = dcp => designerCanvasPreviews.Add(dcp);
            showOtherTabsMenuItem.SubmenuOpened += delegate
            {
                if (!designerCanvasPreviews.Any())
                    DesignerCanvasPreview.DesignerCanvasPreviewCreated += designerCanvasPreviewCreated;
                if (showOtherTabsMenuItem.ItemsSource == null)
                    showOtherTabsMenuItem.Items.Clear();
                showOtherTabsMenuItem.ItemsSource = designerViews;
                Init = true;
                DesignerCanvasPreview.EDMView = edmView;
                DesignerCanvasPreview.UIType = uiType;
            };
            showOtherTabsMenuItem.SubmenuClosed += delegate
            {
                Init = false;
                DesignerCanvasPreview.DesignerCanvasPreviewCreated -= designerCanvasPreviewCreated;
            };
        }

        private IEnumerable<UIComplexType> ComplexTypesAllowedForComplexProperty
        {
            get
            {
                IEnumerable<UIComplexType> value = UIType.View.ComplexTypes;
                UIComplexType uiTypeAsComplexType;
                if ((uiTypeAsComplexType = UIType as UIComplexType) != null)
                    value = value.Except(new[] { uiTypeAsComplexType });
                return value;
            }
        }

        private void DrawComplexPropertyRelationAfterAddedComplexProperty(UIComplexType uiComplexType, UIRelatedProperty uiComplexProperty)
        {
            SizeChangedEventHandler actionToDoWhenNewItemAdded = null;
            actionToDoWhenNewItemAdded =
                delegate
                {
                    var complexProperty = uiComplexProperty.BusinessInstance as ComplexProperty;
                    if (Designer.DesignerView.ContainsEntityType(uiComplexType))
                        DrawComplexPropertyRelation(GetRelatedTypeDesigner(uiComplexProperty), uiComplexProperty);
                    propertiesListView.SelectedValue = uiComplexProperty;
                    GetListViewItem(complexProperty).Focus();
                    propertiesListView.SizeChanged -= actionToDoWhenNewItemAdded;
                };
            propertiesListView.SizeChanged += actionToDoWhenNewItemAdded;
        }

        private bool MoreThanOneTypeSelected
        {
            get { return Designer.TypesSelected.Skip(1).Any(); }
        }

        private bool SetVisibility(MenuItem menuItem, Func<bool> visibilityFunc)
        {
            if (visibilityFunc())
            {
                menuItem.Visibility = Visibility.Visible;
                return true;
            }
            menuItem.Visibility = Visibility.Collapsed;
            return false;
        }

        private bool ApplyNotVisibleIfMoreThanOneTypeSelected(MenuItem menuItem)
        {
            var value = !MoreThanOneTypeSelected;
            menuItem.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            return value;
        }

        private bool PropertiesTreeViewFocused
        {
            get { return VisualTreeHelperUtil.IsAscendant(propertiesListView, VisualTreeHelperUtil.GetFocusedElement(this)); }
        }

        public bool CanCutCopy()
        {
            return !(MoreThanOneTypeSelected || propertiesListView.SelectedItems.OfType<UIRelatedProperty>().Any(uire => uire.BusinessInstance is NavigationProperty)) && PropertiesTreeViewFocused;
        }

        public bool CanPaste()
        {
            return !MoreThanOneTypeSelected && Clipboard.ContainsText() && Clipboard.GetText() == _clipBoardCache.Key.ToString();
        }

        private bool ApplyCutCopyVisible(MenuItem menuItem)
        {
            if (ApplyNotVisibleIfMoreThanOneTypeSelected(menuItem))
            {
                var value = !_groupRightClick && CanCutCopy();
                menuItem.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                return value;
            }
            return false;
        }
        private bool ApplyPasteVisible()
        {
            var pasteMenuItem = ContextMenu.GetMenuItem(MENUITEM_PASTE);
            if (ApplyNotVisibleIfMoreThanOneTypeSelected(pasteMenuItem))
            {
                var value = !_groupRightClick;
                pasteMenuItem.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                return value;
            }
            return false;
        }

        protected virtual void Rename(object originalSource)
        {
            if (_groupRightClick)
                return;
            var editableTextBlock = originalSource as EditableTextBlock;
            if (editableTextBlock != null)
            {
                editableTextBlock.Edit();
                return;
            }
            if (originalSource == this)
            {
                if (propertiesListView.SelectedItem != null)
                {
                    var listViewItem = GetListViewItem(((UIProperty)propertiesListView.SelectedItem).BusinessInstance);
                    if (listViewItem.IsFocused && RenameProperty(listViewItem))
                        return;
                }
                var selectedControl = VisualTreeHelperUtil.GetFocusedElement(this);
                editableTextBlock = selectedControl as EditableTextBlock;
                if (editableTextBlock != null)
                {
                    editableTextBlock.Edit();
                    return;
                }
                if (selectedControl is System.Windows.Controls.ListViewItem && RenameProperty(selectedControl))
                    return;
                entityHeaderEditableTextBlock.Edit();
                return;
            }
            if (originalSource is System.Windows.Controls.ListViewItem)
                RenameProperty(originalSource);
        }

        private bool RenameProperty(object listViewItem)
        {
            var editableTextBlock = VisualTreeHelperUtil.GetControlsDecendant<EditableTextBlock>(listViewItem).First();
            if (editableTextBlock != null)
            {
                editableTextBlock.Edit();
                return true;
            }
            return false;
        }

        private bool AllowDelete()
        {
            return AllowDelete(ContextMenu.GetMenuItem(MENUITEM_DELETE));
        }
        private bool AllowDelete(MenuItem deleteMenuItem)
        {
            return !_groupRightClick && (!PropertiesTreeViewFocused || ApplyNotVisibleIfMoreThanOneTypeSelected(deleteMenuItem));
        }
        protected virtual void Delete(object originalSource)
        {
            if (!AllowDelete())
                return;
            if (originalSource == this)
                Designer.DeleteSelectedTypes();
            else
                DeleteSelectedProperties();
        }

        private void DeleteSelectedProperties()
        {
            foreach (var uiProperty in propertiesListView.SelectedItems.OfType<UIProperty>().ToList())
            {
                var uiRelatedProperty = uiProperty as UIRelatedProperty;
                if (uiRelatedProperty != null && RelationsContenerByRelatedProperty.ContainsKey(uiRelatedProperty))
                {
                    var relationContener = RelationsContenerByRelatedProperty[uiRelatedProperty];
                    Designer.Children.Remove(relationContener);
                    relationContener.OnRemove();
                }
                UIType.DeleteProperty(uiProperty);
            }
        }

        private static KeyValuePair<Guid, IEnumerable<PropertyBase>> _clipBoardCache;
        public void CopySelectedProperties()
        {
            Clipboard.Clear();
            var key = Guid.NewGuid();
            _clipBoardCache = new KeyValuePair<Guid, IEnumerable<PropertyBase>>(key, propertiesListView.SelectedItems.OfType<UIProperty>().Select(uip => uip.BusinessInstance).ToList());
            Clipboard.SetData(DataFormats.Text, key.ToString());
        }

        public void CutSelectedProperties()
        {
            CopySelectedProperties();
            DeleteSelectedProperties();
        }

        public void PasteClipboardProperties()
        {
            foreach (var property in _clipBoardCache.Value)
            {
                var duplicateProperty = UIType.BusinessInstance.DuplicateProperty(property);
                if (property is ComplexProperty)
                {
                    var uiComplexProperty = UIType.Properties.OfType<UIRelatedProperty>().First(uirp => uirp.BusinessInstance == duplicateProperty);
                    DrawComplexPropertyRelationAfterAddedComplexProperty((UIComplexType)uiComplexProperty.RelatedType, uiComplexProperty);
                }
            }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            Designer.UITypeToAdd = null;
            if (!IsSelected)
                Designer.UnselectOtherTypes(this);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            e.Handled = true;
        }

        private void Grid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _groupRightClick = true;
        }

        private void UserControl_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _groupRightClick = false;
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    Delete(e.OriginalSource);
                    e.Handled = true;
                    break;
            }
        }
    }
}
