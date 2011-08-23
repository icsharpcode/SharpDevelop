// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Straight edge in the <see cref="RouteGraph />.
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
