// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.Profiler.AddIn.Views;
using ICSharpCode.Profiler.Controller;
using ICSharpCode.Profiler.Controls;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	/// <summary>
	/// Description of ShowFunctions
	/// </summary>
	public class ShowFunctions : ProfilerMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			var selectedItem = GetSelectedItems().FirstOrDefault();
			if (selectedItem != null)
				Parent.CreateTab(string.Format(StringParser.Parse("${res:AddIns.Profiler.Commands.ShowFunctions.TabTitle}"), selectedItem.GetSignature()),
				                 "from f in Functions where f.Signature == \"" + selectedItem.GetSignature() + "\" select f");
		}
	}
}
