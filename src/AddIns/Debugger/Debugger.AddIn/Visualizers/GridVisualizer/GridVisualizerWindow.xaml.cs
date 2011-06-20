// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Debugger.AddIn.TreeModel;
using Debugger.AddIn.Visualizers.PresentationBindings;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Interaction logic for GridVisualizerWindow.xaml
	/// </summary>
	public partial class GridVisualizerWindow : Window
	{
		WindowsDebugger debuggerService;
		GridViewColumnHider columnHider;
		
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
			try {
				// clear ListView
				listView.ItemsSource = null;
				ScrollViewer listViewScroller = listView.GetScrollViewer();
				if (listViewScroller != null) {
					listViewScroller.ScrollToVerticalOffset(0);
				}
				Value shownValue = null;
				ICSharpCode.NRefactory.Ast.Expression shownExpr = null;
				try	{
					shownExpr = debuggerService.GetExpression(txtExpression.Text);
					shownValue = shownExpr.Evaluate(debuggerService.DebuggedProcess);
				} catch(GetValueException e) {
					MessageService.ShowMessage(e.Message);
				}
				if (shownValue != null && !shownValue.IsNull) {
					GridValuesProvider gridValuesProvider;
					// Value is IList
					DebugType iListType, listItemType;
					if (shownValue.Type.ResolveIListImplementation(out iListType, out listItemType)) {
						gridValuesProvider = CreateListValuesProvider(shownExpr.CastToIList(), listItemType);
					} else	{
						// Value is IEnumerable
						DebugType iEnumerableType, itemType;
						if (shownValue.Type.ResolveIEnumerableImplementation(out iEnumerableType, out itemType)) {
							DebugType debugListType;
							var debugListExpression = DebuggerHelpers.CreateDebugListExpression(shownExpr, itemType, out debugListType);
							gridValuesProvider = CreateListValuesProvider(debugListExpression, itemType);
						} else	{
							// Not IList or IEnumerable<T> - can't be displayed in GridVisualizer
							return;
						}
					}
					
					IList<MemberInfo> itemTypeMembers = gridValuesProvider.GetItemTypeMembers();
					InitializeColumns((GridView)this.listView.View, itemTypeMembers);
					this.columnHider = new GridViewColumnHider((GridView)this.listView.View);
					cmbColumns.ItemsSource = this.columnHider.HideableColumns;
				}
			} catch (GetValueException e) {
				MessageService.ShowMessage(e.Message);
			} catch (DebuggerVisualizerException e) {
				MessageService.ShowMessage(e.Message);
			}
		}
		
		ListValuesProvider CreateListValuesProvider(ICSharpCode.NRefactory.Ast.Expression targetExpression, DebugType listItemType)
		{
			var listValuesProvider = new ListValuesProvider(targetExpression, listItemType);
			var virtCollection = new VirtualizingCollection<ObjectValue>(listValuesProvider);
			this.listView.ItemsSource = virtCollection;
			return listValuesProvider;
		}
		
		void InitializeColumns(GridView gridView, IList<MemberInfo> itemTypeMembers)
		{
			gridView.Columns.Clear();
			AddIndexColumn(gridView);
			AddMembersColumns(gridView, itemTypeMembers);
		}
		
		void AddIndexColumn(GridView gridView)
		{
			var indexColumn = new GridViewHideableColumn();
			indexColumn.CanBeHidden = false;
			indexColumn.Width = 36;
			indexColumn.Header = string.Empty;
			indexColumn.DisplayMemberBinding = new Binding("Index");
			gridView.Columns.Add(indexColumn);
		}
		
		void AddMembersColumns(GridView gridView, IList<MemberInfo> itemTypeMembers)
		{
			foreach (var member in itemTypeMembers)	{
				var memberColumn = new GridViewHideableColumn();
				memberColumn.CanBeHidden = true;
				memberColumn.Header = member.Name;
				// "{Binding Path=[Name].Value}"
				memberColumn.DisplayMemberBinding = new Binding("[" + member.Name + "].Value");
				gridView.Columns.Add(memberColumn);
			}
		}
	}
}
