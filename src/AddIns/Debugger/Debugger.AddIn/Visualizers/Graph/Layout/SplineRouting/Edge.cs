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
	/// Description of Edge.
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
