// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Description of Point2D.
	/// </summary>
	public struct Point2D : IPoint
	{
		public Point2D(double x, double y) : this()
		{
			this.X = x;
			this.Y = y;
		}
		
		public double X { get; set; }
		public double Y { get; set; }
	}
}
