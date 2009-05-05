// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using Debugger.AddIn.Visualizers.Graph.Drawing;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Node with position information.
	/// </summary>
	public class PositionedNode
	{
		public PositionedNode(NodeControl nodeVisualControl)
		{
			this.nodeVisualControl = nodeVisualControl;
		}
		
		public double Left { get; set; }
		public double Top { get; set; }
		public double Width 
		{
			get { return NodeVisualControl.DesiredSize.Width; }
		}
		public double Height
		{
			get { return NodeVisualControl.DesiredSize.Height; }
		}
		
		private NodeControl nodeVisualControl;
		/// <summary>
		/// Visual control to be shown for this node.
		/// </summary>
		public NodeControl NodeVisualControl 
		{ 
			get
			{
				return this.nodeVisualControl;
			}
		}
		
		public virtual IEnumerable<PositionedEdge> Edges
		{
			get
			{
				return new PositionedEdge[]{};
			}
		}
	}
}
