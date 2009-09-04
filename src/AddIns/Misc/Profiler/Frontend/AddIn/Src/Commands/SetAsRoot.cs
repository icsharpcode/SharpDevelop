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
	public class SetAsRoot : ProfilerMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			var items = GetSelectedItems().Where(item => item != null && item.Node != null).Select(i => i.Node);
			
			if (items.FirstOrDefault() != null) {
				List<string> parts = new List<string>();
				
				int? nameId = items.First().NameMapping.Id; // use nullable int to represent non assigned nameId
				
				foreach (CallTreeNode node in items) {
					if (nameId != null && nameId != node.NameMapping.Id)
						nameId = null;
					foreach (var path in node.GetPath())
						parts.Add("GetNodeByPath(" + string.Join(",", path.Select(i => i.ToString()).ToArray()) + ")");
				}
				
				string header = string.Format(StringParser.Parse("${res:AddIns.Profiler.Commands.SetAsRoot.TabTitle}:"), items.First().Name);
				if (nameId == null)
					header = StringParser.Parse("${res:AddIns.Profiler.Commands.SetAsRoot.TabTitle}:");
				
				Parent.CreateTab(header, "Merge(" + string.Join(",", parts.ToArray()) + ")");
			}	
		}
	}
}
