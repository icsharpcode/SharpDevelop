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
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Interaction logic for GridVisualizerWindow.xaml
	/// </summary>
	public partial class GridVisualizerWindow : Window
	{
		private WindowsDebugger debuggerService;
		
		public GridVisualizerWindow()
		{
			InitializeComponent();
		}
		
		private void btnInspect_Click(object sender, RoutedEventArgs e)
		{
			this.debuggerService = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (debuggerService == null)
				throw new ApplicationException("Only windows debugger is currently supported");
			
			Value val = debuggerService.GetValueFromName(txtExpression.Text).GetPermanentReference();
			if (val != null && !val.IsNull)
			{
				// search val.Type.Interfaces for IList<T>, from it, get T
				DebugType iListType = val.Type.GetInterface(typeof(IList).FullName);
				if (iListType != null)
				{
					List<DebugType> genericArguments = val.Type.GenericArguments;
					if (genericArguments.Count == 1)
					{
						DebugType listItemType = genericArguments[0];
						
						var valuesProvider = new ListValuesProvider(val.Expression, iListType, listItemType);
						var virtCollection = new VirtualizingCollection<ObjectValue>(valuesProvider);
						
						IList<MemberInfo> listItemTypeMembers = valuesProvider.GetItemTypeMembers();
						createListViewColumns(listItemTypeMembers);
						
						this.listView.ItemsSource = virtCollection;
					}
				}
			}
		}
		
		private void createListViewColumns(IList<MemberInfo> itemTypeMembers)
		{
			var gridView = (GridView)listView.View;
			gridView.Columns.Clear();
			foreach (var member in itemTypeMembers)
			{
				GridViewColumn column = new GridViewColumn();
				column.Header = member.Name;
				// "{Binding Path=[Name]}"
				column.DisplayMemberBinding = new Binding("[" + member.Name + "].Value");
				gridView.Columns.Add(column);
			}
		}
	}
}