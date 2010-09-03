// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Direction of tree layout.
	/// </summary>
	public enum LayoutDirection
	{
		[Display("Left to right")]
		LeftRight,
		[Display("Top to bottom")]
		TopBottom
	}
}
