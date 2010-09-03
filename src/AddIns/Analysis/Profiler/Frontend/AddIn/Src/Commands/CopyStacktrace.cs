// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	/// <summary>
	/// Description of CopyStacktrace
	/// </summary>
	public class CopyStacktrace : ProfilerMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			var selectedItem = GetSelectedItems().FirstOrDefault();
			
			if (selectedItem != null) {
				var node = selectedItem.Node;
				var data = new StringBuilder();
				while (node != null && node.NameMapping.Id != 0) { // TODO : Callers returns a "merged node" as a caller,
																   // is checking for Id == 0 safe???
					data.AppendLine(node.Signature);
					node = node.Callers.FirstOrDefault(); // TODO : only takes first path!
				}
				
				if (data.Length > 0)
					Clipboard.SetText(data.ToString());
			}
		}
	}
}
