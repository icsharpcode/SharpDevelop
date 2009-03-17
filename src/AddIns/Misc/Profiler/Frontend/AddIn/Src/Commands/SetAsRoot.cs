// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.Profiler.AddIn.Views;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.Profiler.Controller.Queries;
using ICSharpCode.Profiler.Controls;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	/// <summary>
	/// Description of SetAsRoot
	/// </summary>
	public class SetAsRoot : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			IList<CallTreeNodeViewModel> list = ((QueryView)Owner).SelectedItems.ToList();
			ProfilerView parent = (((((QueryView)Owner).Parent as TabItem).Parent as TabControl).Parent as Grid).Parent as ProfilerView;
			
			if (list.Count > 0) {
				var items = from item in list select item.Node;	
				
				List<string> parts = new List<string>();
				
				int? nameId = items.First().NameMapping.Id; // use nullable int to represent non assigned nameId
				
				foreach (CallTreeNode node in items) {
					if (nameId != null && nameId != node.NameMapping.Id)
						nameId = null;
					NodePath p = node.GetPath().First();
					if (p != null) {
						parts.Add("GetNodeByPath(" + string.Join(",", p.Select(i => i.ToString()).ToArray()) + ")");
					}
				}
				
				string header = "Merged Nodes: " + items.First().Name;
				if (nameId == null)
					header = "Merged Nodes";
				
				parent.CreateTab(header, "Merge(" + string.Join(",", parts.ToArray()) + ")");
			}	
		}
	}
}
