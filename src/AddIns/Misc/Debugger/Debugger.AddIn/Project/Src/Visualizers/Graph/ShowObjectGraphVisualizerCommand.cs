// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníèek" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Command in the tools menu for showing the object graph visualizer.
	/// </summary>
	public class ShowObjectGraphVisualizerCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			VisualizerWPFWindow window = new VisualizerWPFWindow();
			window.Topmost = true;
			// fix non-editable TextBox bug
			//System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window);
			window.Show();
			//WorkbenchSingleton.Workbench.ShowView(new DebuggerVisualizerViewContent());
		}
	}
}
