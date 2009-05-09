// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Edge with position information.
	/// </summary>
	public class PositionedEdge : NamedEdge<PositionedNode>
	{
		private IList<Point> splinePoints = null;
		
		// ReadOnlyCollection<Point> ?
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
