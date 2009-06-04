// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using Debugger.AddIn.Visualizers.Graph.Drawing;
using System.Windows;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// ObjectNode with added position information.
	/// </summary>
	public class PositionedNode
	{
		private ObjectNode objectNode;
		/// <summary>
		/// Underlying ObjectNode.
		/// </summary>
		public ObjectNode ObjectNode
		{
			get { return objectNode; }
		}
		
		private List<PositionedNodeProperty> properties = new List<PositionedNodeProperty>();
		public List<PositionedNodeProperty> Properties
		{
			get
			{
				return this.properties;
			}
		}
		
		/// <summary>
		/// Creates new PositionedNode.
		/// </summary>
		/// <param name="objectNode">Underlying ObjectNode.</param>
		public PositionedNode(ObjectNode objectNode)
		{
			this.objectNode = objectNode;
			
			this.nodeVisualControl = new NodeControl();
			this.nodeVisualControl.GraphNode = this.objectNode;	// display
			this.nodeVisualControl.Measure(new Size(500, 500));
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
		
		public Point LeftTop
		{
			get { return new Point(Left, Top); }
		}
		
		public Point Center
		{
			get { return new Point(Left + Width / 2, Top + Height / 2); }
		}
		
		public Rect Rect { get {  return new Rect(Left, Top, Width, Height); } }
		
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
				foreach	(PositionedNodeProperty property in this.Properties)
				{
					yield return property.Edge;
				}
			}
		}
	}
}
