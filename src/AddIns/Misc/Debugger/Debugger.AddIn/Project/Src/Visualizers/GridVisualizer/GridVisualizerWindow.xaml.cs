// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.MetaData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.AddIn.Visualizers.Common;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Interaction logic for GridVisualizerWindow.xaml
	/// </summary>
	public partial class GridVisualizerWindow : Window
	{
		private WindowsDebugger debuggerService;
		private GridViewColumnHider columnHider;
		private SelectedProperties selectedProperties;
		
		public GridVisualizerWindow()
		{
			InitializeComponent();
			
			this.debuggerService = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (debuggerService == null)
				throw new ApplicationException("Only windows debugger is currently supported");
			
			instance = this;
			this.Deactivated += GridVisualizerWindow_Deactivated;
		}

		void GridVisualizerWindow_Deactivated(object sender, EventArgs e)
		{
			this.Close();
		}
		
		private ICSharpCode.NRefactory.Ast.Expression shownExpression;
		public ICSharpCode.NRefactory.Ast.Expression ShownExpression
		{
			get {
				return shownExpression;
			}
			set {
				if (value == null) {
					shownExpression = null;
					txtExpression.Text = null;
					
					Refresh();
					return;
				}
				if (shownExpression == null || value.PrettyPrint() != shownExpression.PrettyPrint()) {
					txtExpression.Text = value.PrettyPrint();
					Refresh();
				}
			}
		}
		
		static GridVisualizerWindow instance;
		/// <summary> When Window is visible, returns reference to the Window. Otherwise returns null. </summary>
		public static GridVisualizerWindow Instance
		{
			get { return instance; }
		}
		
		public static GridVisualizerWindow EnsureShown()
		{
			var window = GridVisualizerWindow.Instance ?? new GridVisualizerWindow();
			window.Topmost = true;
			window.Show();
			return window;
		}
		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			this.Deactivated -= GridVisualizerWindow_Deactivated;
			base.OnClosing(e);
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			instance = null;
		}
		
		private void btnInspect_Click(object sender, RoutedEventArgs e)
		{
			Refresh();
		}
		
		public void Refresh()
		{
			// clear ListView
			listView.ItemsSource = null;
			ScrollViewer listViewScroller = listView.GetScrollViewer();
			if (listViewScroller != null) {
				listViewScroller.ScrollToVerticalOffset(0);
			}
			Value val = null;
			try	{
				val = debuggerService.GetValueFromName(txtExpression.Text);
			} catch(GetValueException) {
				// display ex.Message
			}
			if (val != null && !val.IsNull) {
				GridValuesProvider gridValuesProvider;
				
				// Value is IList?
				DebugType iListType, listItemType;
				if (val.Type.ResolveIListImplementation(out iListType, out listItemType)) {
					var listValuesProvider = new ListValuesProvider(val.ExpressionTree, iListType, listItemType);
					var virtCollection = new VirtualizingCollection<ObjectValue>(listValuesProvider);
					this.listView.ItemsSource = virtCollection;
					gridValuesProvider = listValuesProvider;
				} else	{
					// Value is IEnumerable?
					DebugType iEnumerableType, itemType;
					if (val.Type.ResolveIEnumerableImplementation(out iEnumerableType, out itemType)) {
						var lazyListViewWrapper = new LazyItemsControl<ObjectValue>(this.listView, 24);
						var enumerableValuesProvider = new EnumerableValuesProvider(val.ExpressionTree, iEnumerableType, itemType);
						lazyListViewWrapper.ItemsSource = new VirtualizingIEnumerable<ObjectValue>(enumerableValuesProvider.ItemsSource);
						gridValuesProvider = enumerableValuesProvider;
					} else	{
						// Value cannot be displayed in GridVisualizer
						return;
					}
				}
				
				IList<MemberInfo> itemTypeMembers = gridValuesProvider.GetItemTypeMembers();
				createGridViewColumns((GridView)this.listView.View, itemTypeMembers);
				this.columnHider = new GridViewColumnHider((GridView)this.listView.View);
				// fill column-choosing ComboBox
				this.selectedProperties = initializeSelectedPropertiesWithEvents(itemTypeMembers);
				cmbColumns.ItemsSource = this.selectedProperties;
			}
		}
		
		private void createGridViewColumns(GridView gridView, IList<MemberInfo> itemTypeMembers)
		{
			gridView.Columns.Clear();
			foreach (var member in itemTypeMembers)	{
				GridViewColumn column = new GridViewColumn();
				column.Header = member.Name;
				// "{Binding Path=[Name].Value}"
				column.DisplayMemberBinding = new Binding("[" + member.Name + "].Value");
				gridView.Columns.Add(column);
			}
		}
		
		private SelectedProperties initializeSelectedPropertiesWithEvents(IList<MemberInfo> itemTypeMembers)
		{
			var selectedProperties = new SelectedProperties(itemTypeMembers.Select(member => member.Name));
			foreach (var selectedProperty in selectedProperties) {
				selectedProperty.SelectedChanged += new EventHandler(selectedProperty_SelectedChanged);
			}
			return selectedProperties;
		}
		
		void selectedProperty_SelectedChanged(object sender, EventArgs e)
		{
			var propertySeleted = ((SelectedProperty)sender);
			var columnName = propertySeleted.Name;
			if (propertySeleted.IsSelected) {
				this.columnHider.ShowColumn(columnName);
			} else	{
				this.columnHider.HideColumn(columnName);
			}
		}
	}
}