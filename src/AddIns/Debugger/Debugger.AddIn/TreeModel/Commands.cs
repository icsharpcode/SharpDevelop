// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using ICSharpCode.SharpDevelop;

namespace Debugger.AddIn.TreeModel
{
	public class CopyCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			var grid = parameter as DataGrid;
			if (grid == null) return;
			var selection = FormatValue(grid.SelectedItems.OfType<ValueNode>());
			SD.Clipboard.SetText(selection);
		}
		
		string FormatValue(IEnumerable<ValueNode> nodes)
		{
			StringBuilder b = new StringBuilder();
			bool first = true;
			foreach (var node in nodes) {
				if (first)
					first = false;
				else
					b.AppendLine();
				b.Append(node.FullText);
			}
			
			return b.ToString();
		}
	}
	
	public class ShowFullErrorCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			var grid = parameter as DataGrid;
			if (grid == null) return;
			var error = grid.SelectedItems.OfType<ValueNode>().Select(node => node.error).Single();
			SD.MessageService.ShowException(error, null);
		}
	}
}
