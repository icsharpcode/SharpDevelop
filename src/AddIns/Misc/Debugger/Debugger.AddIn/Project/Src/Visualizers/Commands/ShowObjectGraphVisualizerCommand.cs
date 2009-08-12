// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníèek" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.AddIn.Visualizers.Graph;
using System;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Debugger.AddIn.Visualizers
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
			window.Show();
			//WorkbenchSingleton.Workbench.ShowView(new DebuggerVisualizerViewContent());
		}
	}
}
