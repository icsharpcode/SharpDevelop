// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
