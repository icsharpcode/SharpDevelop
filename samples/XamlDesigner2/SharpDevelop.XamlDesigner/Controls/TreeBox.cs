using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Media;
using SharpDevelop.XamlDesigner.Converters;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class TreeBox : ListBox
	{
		static TreeBox()
		{
			SelectionModeProperty.OverrideMetadata(typeof(TreeBox),
				new FrameworkPropertyMetadata(SelectionMode.Extended));

			HorizontalContentAlignmentProperty.OverrideMetadata(typeof(TreeBox),
				new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));
		}

		public TreeBox()
		{
			SetResourceReference(StyleProperty, typeof(ListBox));
			listener = new CollectionListener(this, "TreeSource");
			listener.CollectionChanged += new NotifyCollectionChangedEventHandler(listener_CollectionChanged);
		}

		CollectionListener listener;

		void listener_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			GenerateRootItems();
		}

		public static readonly DependencyProperty CoreStyleProperty =
			DependencyProperty.Register("CoreStyle", typeof(Style), typeof(TreeBox));

		public Style CoreStyle
		{
			get { return (Style)GetValue(CoreStyleProperty); }
			set { SetValue(CoreStyleProperty, value); }
		}

		public static readonly DependencyProperty TreeSourceProperty =
			DependencyProperty.Register("TreeSource", typeof(IEnumerable), typeof(TreeBox));

		public IEnumerable TreeSource
		{
			get { return (IEnumerable)GetValue(TreeSourceProperty); }
			set { SetValue(TreeSourceProperty, value); }
		}

		public static readonly DependencyProperty AllowDragProperty =
			DependencyProperty.Register("AllowDrag", typeof(bool), typeof(TreeBox));

		public bool AllowDrag
		{
			get { return (bool)GetValue(AllowDragProperty); }
			set { SetValue(AllowDragProperty, value); }
		}

		internal void OnItemsChanged(TreeBoxItemCore core, NotifyCollectionChangedEventArgs e)
		{
			if (core.IsExpanded) {
				int index = Items.IndexOf(core);
				switch (e.Action) {
					case NotifyCollectionChangedAction.Add:
						var newCore = CreateCore(e.NewItems[0], core);
						Items.Insert(index + e.NewStartingIndex + 1, newCore);
						break;
					case NotifyCollectionChangedAction.Remove:
						Items.RemoveAt(index + e.OldStartingIndex + 1);
						break;
					default:
						Collapse(core);
						Expand(core);
						break;
				}
			}
		}

		void GenerateRootItems()
		{
			Items.Clear();

			if (TreeSource != null) {
				foreach (var item in TreeSource) {
					var core = CreateCore(item, null);
					Items.Add(core);
				}
			}
		}

		public void Expand(TreeBoxItemCore core)
		{
			int index = Items.IndexOf(core);
			foreach (var item in core.Items) {
				var newCore = CreateCore(item, core);
				Items.Insert(++index, newCore);
			}
		}

		public void Collapse(TreeBoxItemCore core)
		{
			int index = Items.IndexOf(core) + 1;
			while (index < Items.Count && (Items[index] as TreeBoxItemCore).Level > core.Level) {
				Items.RemoveAt(index);
			}
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			var listBoxItem = new TreeBoxItem();
			listBoxItem.SetBinding(ListBoxItem.IsSelectedProperty, "IsSelected");
			return listBoxItem;
		}

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);

			var core = item as TreeBoxItemCore;
			base.PrepareContainerForItemOverride(core, core.Item);
		}

		TreeBoxItemCore CreateCore(object item, TreeBoxItemCore parent)
		{
			var core = new TreeBoxItemCore();
			core.Item = item;
			core.ParentTree = this;
			if (parent != null) {
				core.Level = parent.Level + 1;
			}
			core.DataContext = item;
			core.SetBinding(FrameworkElement.StyleProperty,
				new Binding("CoreStyle") { Source = this });

			return core;
		}

		#region Drag & Drop

		InsertInfo insert;

		internal void TryStartDrag()
		{
			if (AllowDrag) {
				insert = new InsertInfo();
				insert.Items = SelectedItems.Cast<object>().ToArray();
				insert.Adorner = new GeneralAdorner(this) {
					Child = new OutlineInsertLine()
				};
				DragDrop.DoDragDrop(this, this, DragDropEffects.All);
			}
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			ProcessDrag(e);
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			ProcessDrag(e);
		}

		void ProcessDrag(DragEventArgs e)
		{
			e.Effects = DragDropEffects.None;
			e.Handled = true;

			if (insert != null) {

				HidePreview();

				insert.Copy = Keyboard.IsKeyDown(Key.LeftCtrl);

				var container = (e.OriginalSource as DependencyObject).FindAncestor<TreeBoxItem>();
				if (container != null) {

					insert.Hover = container;

					var y = e.GetPosition(container).Y;
					var h = container.ActualHeight;

					if (y < h / 5) {
						insert.Part = Part.Before;
					}
					else if (y > h - h / 5) {
						insert.Part = Part.After;
					}
					else {
						insert.Part = Part.Inside;
					}

					if (CanDrop()) {
						e.Effects = DragDropEffects.Move;
						ShowPreview();
						return;
					}
				}
			}
		}

		bool CanDrop()
		{
			if (insert.Part == Part.Inside) {
				insert.TargetParent = insert.Hover.Core.Item;
				insert.TargetIndex = insert.Hover.Core.Items.Count;
			}
			else if (insert.Part == Part.Before) {
				SetTarget(insert.Hover.Core, 0);
			}
			else {
				SetTarget(insert.Hover.Core, 1);
			}
			return CanInsert(insert.TargetParent, insert.Items, insert.TargetIndex, insert.Copy);
		}

		void SetTarget(TreeBoxItemCore core, int shift)
		{
			var index = Items.IndexOf(core);
			for (int i = index - 1; i >= 0; i--) {
				var item = Items[i] as TreeBoxItemCore;
				if (item.Level < core.Level) {
					insert.TargetParent = item.Item;
					insert.TargetIndex = item.Items.IndexOf(core) + shift;
					return;
				}
			}

			insert.TargetParent = TreeSource;
			insert.TargetIndex = Items.IndexOf(core) + shift;
		}

		void ShowPreview()
		{
			insert.Hover.Background = FindResource("OutlineInsertBackground") as Brush;

			if (insert.Part == Part.Before || insert.Part == Part.After) {
				var p = insert.Part == Part.Before ? new Point() : new Point(0, insert.Hover.ActualHeight);
				var y = insert.Hover.TransformToAncestor(this).Transform(p).Y;	
				var element = insert.Adorner.Child as FrameworkElement;

				element.Width = insert.Hover.ActualWidth;
				element.Margin = new Thickness(0, y, 0, 0);

				AdornerLayer.GetAdornerLayer(this).Add(insert.Adorner);
			}
		}

		void HidePreview()
		{
			if (insert.Hover != null) {
				insert.Hover.ClearValue(TreeBoxItem.BackgroundProperty);
				AdornerLayer.GetAdornerLayer(this).Remove(insert.Adorner);
			}
		}

		protected override void OnDrop(DragEventArgs e)
		{
			HidePreview();
		}

		protected virtual bool CanInsert(object parent, IEnumerable<object> items, int index, bool copy)
		{
			return true;
		}

		protected virtual void Insert(object parent, IEnumerable<object> items, int index, bool copy)
		{
		}

		protected virtual bool CanDelete(IEnumerable<object> items)
		{
			return true;
		}

		protected virtual void Delete(IEnumerable<object> items)
		{
		}

		class InsertInfo
		{
			public object[] Items;
			public bool Copy;
			public TreeBoxItem Hover;
			public Part Part;
			public object TargetParent;
			public int TargetIndex;
			public GeneralAdorner Adorner;
		}

		enum Part
		{
			Before, Inside, After
		}

		#endregion
	}
}
