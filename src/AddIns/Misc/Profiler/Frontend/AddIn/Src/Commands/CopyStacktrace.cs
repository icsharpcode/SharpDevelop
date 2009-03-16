// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Profiler.Controls;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	/// <summary>
	/// Description of CopyStacktrace
	/// </summary>
	public class CopyStacktrace : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			IList<CallTreeNodeViewModel> list = ((QueryView)Owner).SelectedItems.ToList();
			CallTreeNodeViewModel selectedItem = (list.Count > 0) ? list[0] as CallTreeNodeViewModel : null;
			
			if (selectedItem != null) {
				StringBuilder data = new StringBuilder();

				while (selectedItem.Parent != null) {
					data.AppendLine(selectedItem.GetSignature());
					selectedItem = selectedItem.Parent;
				}

				Clipboard.SetText(data.ToString());
			}
		}
	}
}
