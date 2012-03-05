// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Debugger.AddIn.Visualizers.PresentationBindings;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Interaction logic for GridVisualizerWindow.xaml
	/// </summary>
	public partial class GridVisualizerWindow : Window
	{
		Func<Value> getValue;
		
		public GridVisualizerWindow(string valueName, Func<Value> getValue)
		{
			InitializeComponent();
			
			this.Title = valueName;
			this.getValue = getValue;
			
			Refresh();
		}

		public void Refresh()
		{
			try	{
				// clear ListView
				listView.ItemsSource = null;
				
				Value shownValue = getValue();
				
				DebugType iListType, iEnumerableType, itemType;
				// Value is IList
				if (shownValue.Type.ResolveIListImplementation(out iListType, out itemType)) {
					// Ok
				} else {
					// Value is IEnumerable
					if (shownValue.Type.ResolveIEnumerableImplementation(out iEnumerableType, out itemType)) {
						shownValue = DebuggerHelpers.CreateListFromIEnumeralbe(shownValue, itemType, out iListType);
					} else	{
						// Not IList or IEnumerable<T> - can't be displayed in GridVisualizer
						return;
					}
				}
				shownValue = shownValue.GetPermanentReference();
				
				MemberInfo[] members = itemType.GetFieldsAndNonIndexedProperties(BindingFlags.Public | BindingFlags.Instance);
				PropertyInfo itemGetter = iListType.GetProperty("Item");
				int rowCount = (int)shownValue.GetPropertyValue(iListType.GetProperty("Count")).PrimitiveValue;
				int columnCount = members.Length + 1;
				
				var rowCollection = new VirtualizingCollection<VirtualizingCollection<string>>(
					rowCount,
					(rowIndex) => new VirtualizingCollection<string>(
						columnCount,
						(columnIndex) => {
							if (columnIndex == members.Length)
								return "[" + rowIndex + "]";
							try {
								var rowValue = shownValue.GetPropertyValue(itemGetter, Eval.CreateValue(shownValue.AppDomain, rowIndex));
								return rowValue.GetMemberValue(members[columnIndex]).InvokeToString();
							} catch (GetValueException e) {
								return "Exception: " + e.Message;
							}
						}
					)
				);
				this.listView.ItemsSource = rowCollection;
				
				InitializeColumns((GridView)this.listView.View, members);
				
				GridViewColumnHider columnHider = new GridViewColumnHider((GridView)this.listView.View);
				cmbColumns.ItemsSource = columnHider.HideableColumns;
				
			} catch(GetValueException e) {
				MessageService.ShowMessage(e.Message);
			}
		}
		
		void InitializeColumns(GridView gridView, MemberInfo[] members)
		{
			gridView.Columns.Clear();
			
			// Index column
			var indexColumn = new GridViewHideableColumn();
			indexColumn.CanBeHidden = false;
			indexColumn.Width = 36;
			indexColumn.Header = string.Empty;
			indexColumn.DisplayMemberBinding = new Binding("[" + members.Length + "]");
			gridView.Columns.Add(indexColumn);
			
			// Member columns
			for (int i = 0; i < members.Length; i++) {				
				var memberColumn = new GridViewHideableColumn();
				memberColumn.CanBeHidden = true;
				memberColumn.Header = members[i].Name;
				memberColumn.IsVisibleDefault = ((IDebugMemberInfo)members[i]).IsPublic;
				memberColumn.DisplayMemberBinding = new Binding("[" + i + "]");
				gridView.Columns.Add(memberColumn);
			}
		}
	}
}
