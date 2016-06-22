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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Debugger.AddIn.Visualizers.PresentationBindings;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Services;

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
				
				ParameterizedType iListType, iEnumerableType;
				IType itemType;
				// Value is IList
				if (shownValue.Type.ResolveIListImplementation(out iListType, out itemType)) {
					// Ok
				} else {
					// Value is IEnumerable
					if (shownValue.Type.ResolveIEnumerableImplementation(out iEnumerableType, out itemType)) {
						shownValue = DebuggerHelpers.CreateListFromIEnumerable(shownValue);
						shownValue.Type.ResolveIListImplementation(out iListType, out itemType);
						//var ilistDef = iEnumerableType.Compilation.FindType(typeof(List<>)).GetDefinition();
			            //var ilistType = new ParameterizedType(ilistDef, ienumerableType.TypeArguments);
					} else	{
						// Not IList or IEnumerable<T> - can't be displayed in GridVisualizer
						return;
					}
				}
				shownValue = shownValue.GetPermanentReference(WindowsDebugger.EvalThread);
				
				var members = itemType.GetFieldsAndNonIndexedProperties().Where(m => m.IsPublic && !m.IsStatic).ToList();
				IProperty indexerProperty = iListType.GetProperties(p => p.Name == "Item").Single();
				int rowCount = (int)shownValue.GetPropertyValue(WindowsDebugger.EvalThread, iListType.GetProperties(p => p.Name == "Count").Single()).PrimitiveValue;
				int columnCount = members.Count + 1;
				
				var rowCollection = new VirtualizingCollection<VirtualizingCollection<string>>(
					rowCount,
					(rowIndex) => new VirtualizingCollection<string>(
						columnCount,
						(columnIndex) => {
							if (columnIndex == columnCount - 1) {
								return "[" + rowIndex + "]";
							}
							try {
								var rowValue = shownValue.GetPropertyValue(WindowsDebugger.EvalThread, indexerProperty, Eval.CreateValue(WindowsDebugger.EvalThread, rowIndex));
								return rowValue.GetMemberValue(WindowsDebugger.EvalThread, members[columnIndex]).InvokeToString(WindowsDebugger.EvalThread);
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
		
		void InitializeColumns(GridView gridView, IList<IMember> members)
		{
			gridView.Columns.Clear();
			
			// Index column
			var indexColumn = new GridViewHideableColumn();
			indexColumn.CanBeHidden = false;
			indexColumn.Width = 36;
			indexColumn.Header = string.Empty;
			indexColumn.DisplayMemberBinding = new Binding("[" + members.Count + "]");
			gridView.Columns.Add(indexColumn);
			
			// Member columns
			for (int i = 0; i < members.Count; i++) {				
				var memberColumn = new GridViewHideableColumn();
				memberColumn.CanBeHidden = true;
				memberColumn.Header = members[i].Name;
				memberColumn.IsVisibleDefault = members[i].IsPublic;
				memberColumn.DisplayMemberBinding = new Binding("[" + i + "]");
				gridView.Columns.Add(memberColumn);
			}
		}
	}
}
