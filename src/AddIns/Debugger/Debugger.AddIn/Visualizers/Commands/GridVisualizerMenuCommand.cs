// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using Debugger.AddIn.Visualizers.GridVisualizer;
using System;
using ICSharpCode.Core;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Description of ShowGridVisualizerCommand.
	/// </summary>
	public class GridVisualizerMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			GridVisualizerWindow window = new GridVisualizerWindow();
			window.Topmost = true;
			window.Show();
		}
	}
}
