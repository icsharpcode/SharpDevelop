// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
