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
