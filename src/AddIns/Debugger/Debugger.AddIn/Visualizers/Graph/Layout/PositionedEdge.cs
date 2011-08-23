// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using Debugger.AddIn.Visualizers.Utils;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Edge with position information.
	/// </summary>
	public class PositionedEdge : NamedEdge<PositionedNodeProperty, PositionedNode>, SplineRouting.IEdge
	{
		private IList<Point> splinePoints = new List<Point>();
		/// <summary>
		/// Control points of edge's spline, in standart format: 1 start point + 3 points per segment
		/// </summary>
		public IList<Point> SplinePoints
		{
			get { return splinePoints;	}
			set { splinePoints = value;	}
		}
		
		/// <summary>
		/// Drawn spline representation of this edge.
		/// </summary>
		public System.Windows.Shapes.Path Spline { get; set; }
		
		public SplineRouting.IRect From {
			get { return this.Source.ContainingNode; }
		}
		
		public SplineRouting.IRect To {
			get { return this.Target; }
		}
	}
}
