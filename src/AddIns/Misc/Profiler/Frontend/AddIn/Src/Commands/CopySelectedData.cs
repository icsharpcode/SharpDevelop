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
	public class CopySelectedData : ProfilerMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			var list = GetSelectedItems();
			
			if (list.FirstOrDefault() != null) {
				StringBuilder builder = new StringBuilder();
				
				foreach (CallTreeNodeViewModel node in list) {
					if (node != null)
						builder.AppendLine(
							new string('\t', node.Level) +
							node.Name + "\t" +
							node.CallCount + "\t" +
							node.TimeSpent + "\t" +
							node.TimeSpentSelf + "\t" +
							node.TimeSpentPerCall + "\t" +
							node.TimeSpentSelfPerCall + "\t" +
							node.TimePercentageOfParentAsText
						);
				}
				
				Clipboard.SetText(builder.ToString());
			}
		}
	}
}
