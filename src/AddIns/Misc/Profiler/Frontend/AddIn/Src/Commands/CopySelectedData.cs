// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.Profiler.AddIn.Views;
using ICSharpCode.Profiler.Controls;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	/// <summary>
	/// Description of CopySelectedData
	/// </summary>
	public class CopySelectedData : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			IList<CallTreeNodeViewModel> list = ((QueryView)Owner).SelectedItems.ToList();
			ProfilerView parent = (((((QueryView)Owner).Parent as TabItem).Parent as TabControl).Parent as Grid).Parent as ProfilerView;
			
			if (list.Count > 0) {
				StringBuilder builder = new StringBuilder();
				
				foreach (CallTreeNodeViewModel node in list) {
					builder.AppendLine(new string('\t', node.Level) + node.Name + "\t" + node.CallCount + "\t" + node.TimeSpent + "\t" + node.TimePercentageOfParentAsText);
				}
				
				Clipboard.SetText(builder.ToString());
			}
		}
	}
}
