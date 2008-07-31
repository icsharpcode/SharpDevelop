using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.XamlDesigner
{
    public class OutlineTree : TreeView
    {
        public OutlineTree()
        {
            AllowDrop = true;
            new DragListener(this).DragStarted += new MouseEventHandler(TreeList_DragStarted);
            DragEnter += new DragEventHandler(TreeList_DragEnter);
            DragOver += new DragEventHandler(TreeList_DragOver);
            Drop += new DragEventHandler(TreeList_Drop);

            //Selection = new ObservableCollection<object>();
        }

        Border insertLine;
        OutlineTreeItem markedItem;
        OutlineTreeItem possibleSelection;
        List<OutlineTreeItem> selectionNodes = new List<OutlineTreeItem>();

        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty.Register("Selection", typeof(ObservableCollection<DocumentElement>), typeof(OutlineTree));

        public ObservableCollection<DocumentElement> Selection {
            get { return (ObservableCollection<DocumentElement>)GetValue(SelectionProperty); }
            set { SetValue(SelectionProperty, value); }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            insertLine = (Border)Template.FindName("PART_InsertLine", this);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new OutlineTreeItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is OutlineTreeItem;
        }

        protected virtual DragDropEffects CanDrag(object obj)
        {
            return DragDropEffects.Move;
        }

        protected virtual DragDropEffects CanDrop(object obj, object parent, int index)
        {
            return DragDropEffects.Move;
        }

        void TreeList_DragOver(object sender, DragEventArgs e)
        {
            ProcessDrag(e);
        }

        void TreeList_DragEnter(object sender, DragEventArgs e)
        {
            ProcessDrag(e);
        }

        void TreeList_Drop(object sender, DragEventArgs e)
        {
            ProcessDrop(e);
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            HideDropMarkers();
        }

        void TreeList_DragStarted(object sender, MouseEventArgs e)
        {
            possibleSelection = null;
            object obj = (e.OriginalSource as FrameworkElement).DataContext;
            if (obj != null) {
                DragDropEffects effects = CanDrag(obj);
                if (effects != DragDropEffects.None) {
                    DragDrop.DoDragDrop(this, obj, effects);
                }
            }
        }

        void ProcessDrag(DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;

            OutlineTreeItem treeItem = (e.OriginalSource as DependencyObject).FindAncestor<OutlineTreeItem>();
            if (treeItem != null) {
                HideDropMarkers();

                ContentPresenter header = treeItem.HeaderPresenter;
                Point p = e.GetPosition(header);
                int part = (int)(p.Y / (header.ActualHeight / 3));

                if (part == 1) {
                    markedItem = treeItem;
                    markedItem.Background = insertLine.Background;
                }
                else {
                    insertLine.Visibility = Visibility.Visible;
                    p = header.TransformToVisual(this).Transform(new Point());
                    double y = part == 0 ? p.Y : p.Y + header.ActualHeight;
                    insertLine.Margin = new Thickness(0, y, 0, 0);
                }
            }
        }

        void ProcessDrop(DragEventArgs e)
        {
            HideDropMarkers();
        }

        void HideDropMarkers()
        {
            insertLine.Visibility = Visibility.Collapsed;
            if (markedItem != null) {
                markedItem.ClearValue(OutlineTreeItem.BackgroundProperty);
            }
        }

        void Select(OutlineTreeItem item)
        {
            item.IsSelected = true;
            Selection.Add(item.Element);
            selectionNodes.Add(item);
        }

        void SelectOnly(OutlineTreeItem item)
        {
            foreach (var node in selectionNodes) {
                node.IsSelected = false;
            }
            Selection.Clear();
            Select(item);
        }

        void Unselect(OutlineTreeItem item)
        {
            item.IsSelected = false;
            Selection.Remove(item.Element);
            selectionNodes.Remove(item);
        }

        internal void HandleItemMouseDown(OutlineTreeItem item)
        {
            bool control = Keyboard.IsKeyDown(Key.LeftCtrl);
            if (item.IsSelected) {
                if (control) {
                    Unselect(item);
                }
                else {
                    possibleSelection = item;
                }
            }
            else {
                if (control) {
                    Select(item);
                }
                else {
                    SelectOnly(item);
                }
            }
        }

        internal void HandleItemMouseUp(OutlineTreeItem item)
        {
            if (possibleSelection != null) {
                SelectOnly(possibleSelection);
            }
        }
    }

    public class OutlineTreeItem : TreeViewItem
    {
        public new static readonly DependencyProperty IsSelectedProperty =
            Selector.IsSelectedProperty.AddOwner(typeof(OutlineTreeItem));

        public new bool IsSelected {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public ContentPresenter HeaderPresenter {
            get { return (ContentPresenter)Template.FindName("PART_Header", this); }
        }

        public DocumentElement Element {
            get { return DataContext as DocumentElement; }
        }

        public static readonly DependencyProperty IndentProperty =
            DependencyProperty.Register("Indent", typeof(Thickness), typeof(OutlineTreeItem));

        public Thickness Indent {
            get { return (Thickness)GetValue(IndentProperty); }
            set { SetValue(IndentProperty, value); }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            OutlineTreeItem parent = ItemsControl.ItemsControlFromItemContainer(this) as OutlineTreeItem;
            Indent = parent == null ? new Thickness() : new Thickness(parent.Indent.Left + 19, 0, 0, 0);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new OutlineTreeItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is OutlineTreeItem;
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (e.Source is ToggleButton || e.Source is ItemsPresenter) return;

            OutlineTree tree = this.FindAncestor<OutlineTree>();
            if (tree != null) {
                tree.HandleItemMouseDown(this);
                e.Handled = true;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            OutlineTree tree = this.FindAncestor<OutlineTree>();
            if (tree != null) {
                tree.HandleItemMouseUp(this);
            }
        }
    }

    public class TreeNode : ContentControl
    {
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(TreeNode));

        public ImageSource Image {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
    }
}
