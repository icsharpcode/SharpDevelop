using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Collections.Specialized;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class TreeBoxItemCore : TreeViewItem
	{
		static TreeBoxItemCore()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeBoxItemCore),
				new FrameworkPropertyMetadata(typeof(TreeBoxItemCore)));
		}

		public TreeBoxItemCore()
		{
			Loaded += new RoutedEventHandler(TreeBoxItemCore_Loaded);
		}

		void TreeBoxItemCore_Loaded(object sender, RoutedEventArgs e)
		{
			if (updateWnenLoaded) {
				UpdateItems();
			}
		}

		public TreeBox ParentTree { get; internal set; }
		public object Item { get; internal set; }
		bool updateWnenLoaded;

		public static readonly DependencyProperty LevelProperty =
			DependencyProperty.Register("Level", typeof(int), typeof(TreeBoxItemCore));

		public int Level
		{
			get { return (int)GetValue(LevelProperty); }
			set { SetValue(LevelProperty, value); }
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == IsExpandedProperty) {
				var newValue = (bool)e.NewValue;

				if (IsLoaded) {
					UpdateItems();
				}
				else {
					updateWnenLoaded = true;
				}
			}
		}

		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnItemsChanged(e);
			if (IsLoaded) {
				ParentTree.OnItemsChanged(this, e);
			}
		}

		void UpdateItems()
		{
			if (IsExpanded) {
				ParentTree.Expand(this);
			}
			else {
				ParentTree.Collapse(this);
			}
		}
	}
}
