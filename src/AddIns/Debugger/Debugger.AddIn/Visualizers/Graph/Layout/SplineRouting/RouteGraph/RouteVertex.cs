// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Linq;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Description of RouteVertex.
	/// </summary>
	public class RouteVertex : IPoint
	{
		public double X { get; set; }
		public double Y { get; set; }
		
		public RouteVertex(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}
		
		List<RouteGraphEdge> neighbors = new List<RouteGraphEdge>();
		public List<RouteGraphEdge> Neighbors {
			get { return neighbors; }
		}
		
		public RouteVertex()
		{
			Reset();
			IsUsed = false;
			IsEdgeEndpoint = false;
		}
		
		public void AddNeighbor(RouteVertex target)
		{
			this.Neighbors.Add(new RouteGraphEdge(this, target));
		}
		
		public double Distance { get; set; }
		public bool IsPermanent { get; set; }
		public RouteVertex Predecessor { get; set; }
		public bool IsEdgeEndpoint { get; set; }
		public bool IsUsed { get; set; }
		public bool IsAvailable { get { return !IsUsed && !IsEdgeEndpoint; } }
		
		public void Reset()
		{
			Distance = 10e+6;
			IsPermanent = false;
			Predecessor = null;
			//IsUsed = false;
			//IsEdgeEndpoint = false;
		}
	}
}
