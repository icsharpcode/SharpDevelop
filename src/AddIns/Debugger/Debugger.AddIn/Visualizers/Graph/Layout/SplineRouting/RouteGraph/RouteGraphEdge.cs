// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Edge in the RouteGraph.
	/// </summary>
	public class RouteGraphEdge
	{
		public RouteVertex StartVertex { get; private set; }
		public RouteVertex EndVertex { get; private set; }
		
		public double Length { get; private set; }
		
		public RouteGraphEdge(RouteVertex startVertex, RouteVertex endVertex)
		{
			this.StartVertex = startVertex;
			this.EndVertex = endVertex;
			this.Length = GeomUtils.LineLenght(startVertex, endVertex);
		}
	}
}
