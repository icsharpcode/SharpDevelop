// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Given <see cref="PositionedGraph" /> with positions of nodes, calculates positions of edges.
	/// </summary>
	public class NeatoEdgeRouter
	{
		public NeatoEdgeRouter()
		{
		}
		
		/// <summary>
		/// Given <see cref="PositionedGraph" /> with node positions, calculates edge positions.
		/// </summary>
		/// <param name="graphWithNodesPositioned"><see cref="PositionedGraph" />, the nodes must have positions filled.</param>
		/// <returns><see cref="PositionedGraph" /> with preserved node positions and calculated edge positions.</returns>
		public PositionedGraph CalculateEdges(PositionedGraph graphWithNodesPositioned)
		{
			DotFormatter dotFormatter = new BoxDotFormatter(graphWithNodesPositioned);
			
			// start Neato.exe
			NeatoProcess neatoProcess = NeatoProcess.Start();
			
			// convert PosGraph to .dot string
			string dotGraphString = dotFormatter.OutputGraphInDotFormat();
				
			// pass to neato.exe
			string dotGraphStringWithPositions = neatoProcess.CalculatePositions(dotGraphString);
			
			// parse edge positions from neato's plain output format
			PositionedGraph result = dotFormatter.ParseEdgePositions(dotGraphStringWithPositions);
			
			return result;
		}
	}
}
