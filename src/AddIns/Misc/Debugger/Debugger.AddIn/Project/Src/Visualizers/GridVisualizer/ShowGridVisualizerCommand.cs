// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníèek" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.Core;
using System;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Description of ShowGridVisualizerCommand.
	/// </summary>
	public class ShowGridVisualizerCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			GridVisualizerWindow window = new GridVisualizerWindow();
			window.Topmost = true;
			window.Show();
		}
	}
}
