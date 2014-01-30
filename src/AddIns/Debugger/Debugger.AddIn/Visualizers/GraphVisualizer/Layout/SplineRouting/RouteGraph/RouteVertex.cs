// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
