// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.AddIn.Visualizers.Utils;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Edge with position information.
	/// </summary>
	public class PositionedEdge : NamedEdge<PositionedNodeProperty, PositionedNode>
	{
		private IList<Point> splinePoints = new List<Point>();
		
		// ReadOnlyCollection<Point> ?
		/// <summary>
		/// Control points of edge's spline, in standart format: 1 start point + 3 points per segment
		/// </summary>
		public IList<Point> SplinePoints 
		{
			get 
			{
				return splinePoints;
			}
			set 
			{
				splinePoints = value;
			}
		}
	}
}
