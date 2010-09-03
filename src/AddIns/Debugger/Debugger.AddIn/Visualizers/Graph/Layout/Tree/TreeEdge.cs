// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	// TODO this class is almost not necessary, is used only for TreeLayouter purposes.
	// TreeLayouter could remember the additional information in a Dictionary PositionedEdge -> bool
	
	/// <summary>
	/// Edge in the tree-layouted <see cref="PositionedGraph"/>.
	/// </summary>
	public class TreeGraphEdge : PositionedEdge
	{
		/// <summary>
		/// Is this an edges making up the main tree?
		/// </summary>
		public bool IsTreeEdge { get; set; }
	}
}
