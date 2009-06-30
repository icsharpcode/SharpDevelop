// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
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
