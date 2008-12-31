using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpDevelop.XamlDesigner.Controls;
using System.Collections.Specialized;
using System.Collections;
using SharpDevelop.XamlDesigner.Dom;
using SharpDevelop.XamlDesigner.Outline;

namespace SharpDevelop.XamlDesigner.PropertyGrid
{
	public partial class PropertyGridView : UserControl, IHasContext
	{
		public PropertyGridView()
		{
			InitializeComponent();
			Model = new PropertyGridModel(this);
			uxDataContextHolder.DataContext = Model;

			listener = new CollectionListener(this, "SelectionSource");
			listener.CollectionChanged += listener_CollectionChanged;

			SetBinding(SelectionSourceProperty, new Binding("Context.Selection") { Source = this });

			ContextMenu = new PropertyMenu();
			ContextMenuOpening += new ContextMenuEventHandler(PropertyGridView_ContextMenuOpening);
		}

		void PropertyGridView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			var propertyNode = e.GetDataContext() as PropertyNode;
			if (propertyNode != null) {
				ContextMenu.DataContext = new PropertyMenuModel(propertyNode);
			}
		}

		CollectionListener listener;

		void listener_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Model.Selection = SelectionSource != null ? 
				SelectionSource.Cast<DesignItem>().ToArray() : null;
		}

		public PropertyGridModel Model { get; private set; }

		public static readonly DependencyProperty ContextProperty =
		  OutlineView.ContextProperty.AddOwner(typeof(PropertyGridView));

		public DesignContext Context
		{
			get { return (DesignContext)GetValue(ContextProperty); }
			set { SetValue(ContextProperty, value); }
		}

		public static readonly DependencyProperty SelectionSourceProperty =
			DependencyProperty.Register("SelectionSource", typeof(IEnumerable), typeof(PropertyGridView));

		public IEnumerable SelectionSource
		{
			get { return (IEnumerable)GetValue(SelectionSourceProperty); }
			set { SetValue(SelectionSourceProperty, value); }
		}
	}
}
