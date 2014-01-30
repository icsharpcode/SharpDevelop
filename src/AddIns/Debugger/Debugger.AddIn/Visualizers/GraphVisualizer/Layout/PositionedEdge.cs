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
