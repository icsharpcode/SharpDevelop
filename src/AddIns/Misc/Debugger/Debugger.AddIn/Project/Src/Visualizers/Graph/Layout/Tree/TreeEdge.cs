// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Edge in the tree-layouted <see cref="PositionedGraph"/>.
	/// </summary>
	public class TreeGraphEdge : PositionedEdge
	{
		/// <summary>
		/// Is this a main edges making up the tree?
		/// </summary>
		public bool IsTreeEdge { get; set; }
	}
}
