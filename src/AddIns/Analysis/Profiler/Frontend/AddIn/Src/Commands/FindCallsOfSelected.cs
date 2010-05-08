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
	/// Description of FindCallsOfSelected
	/// </summary>
	public class FindCallsOfSelected : ProfilerMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			var list = GetSelectedItems();
			
			if (list.Any()) {
				var items = from item in list select item.Node;	
				
				List<string> parts = new List<string>();
								
				foreach (CallTreeNode node in items) {
					NodePath p = node.GetPath().First();
					if (p != null) {
						parts.Add("c.NameMapping.Id == " + node.NameMapping.Id);
					}
				}
				
				string header = StringParser.Parse("${res:AddIns.Profiler.Commands.FindCallsOfSelected.TabTitle}");
				
				Parent.CreateTab(header, "from c in Calls where " + string.Join(" || ", parts.ToArray()) + " select c");
			}	
		}
	}
}
