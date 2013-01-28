// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Simple implementation of <see cref="IEdge" />.
	/// Edge betweeen two <see cref="IRect" /> with given Start and End point on the borders of the rects.
	/// </summary>
	public class Edge : IEdge
	{
		public Edge()
		{
		}
		
		public IRect From { get; set; }
		public IRect To { get; set; }
		public IPoint StartPoint { get; set; }
		public IPoint EndPoint { get; set; }
	}
}
