// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
	public class ObjectGraphVisualizerMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ObjectGraphWindow.EnsureShown();
		}
	}
}
