// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// One node in the <see cref="RouteGraph" />, represents a box corner.
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
		/// <summary>
		/// Edge endpoints are not available for routing through them.
		/// </summary>
		public bool IsEdgeEndpoint { get; set; }
		public bool IsUsed { get; set; }
		public bool IsAvailable { get { return !IsUsed && !IsEdgeEndpoint; } }
		
		public double Penalization { get; set; }
		
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
