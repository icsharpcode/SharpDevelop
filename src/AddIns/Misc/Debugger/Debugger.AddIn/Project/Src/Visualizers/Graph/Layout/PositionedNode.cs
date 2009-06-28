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
		private ObjectGraphNode objectNode;
		/// <summary>
		/// Underlying ObjectNode.
		/// </summary>
		public ObjectGraphNode ObjectNode
		{
			get { return objectNode; }
		}
		
		public event EventHandler<PositionedPropertyEventArgs> Expanded;
		public event EventHandler<PositionedPropertyEventArgs> Collapsed;
		
		private List<PositionedNodeProperty> properties = new List<PositionedNodeProperty>();
		public List<PositionedNodeProperty> Properties
		{
			get
			{
				return this.properties;
			}
		}
		
		public PositionedNodeProperty AddProperty(ObjectGraphProperty objectProperty, bool isExpanded)
		{
			var newProperty = new PositionedNodeProperty(objectProperty, this);
			newProperty.IsExpanded = isExpanded;
			this.Properties.Add(newProperty);
			this.nodeVisualControl.AddProperty(newProperty);
			
			return newProperty;
		}
		
		/// <summary>
		/// Creates new PositionedNode.
		/// </summary>
		/// <param name="objectNode">Underlying ObjectNode.</param>
		public PositionedNode(ObjectGraphNode objectNode)
		{
			this.objectNode = objectNode;
			
			this.nodeVisualControl = new NodeControl(this);	// display
			this.nodeVisualControl.Expanded += new EventHandler<PositionedPropertyEventArgs>(NodeVisualControl_Expanded);
			this.nodeVisualControl.Collapsed += new EventHandler<PositionedPropertyEventArgs>(NodeVisualControl_Collapsed);
		}

		private void NodeVisualControl_Expanded(object sender, PositionedPropertyEventArgs e)
		{
			// propagage event
			OnPropertyExpanded(this, e);
		}
		
		private void NodeVisualControl_Collapsed(object sender, PositionedPropertyEventArgs e)
		{
			// propagate event
			OnPropertyCollapsed(this, e);
		}
		
		public void Measure()
		{
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
					if (property.Edge != null)
						yield return property.Edge;
				}
			}
		}
		
		#region event helpers
		protected virtual void OnPropertyExpanded(object sender, PositionedPropertyEventArgs propertyArgs)
		{
			if (this.Expanded != null)
			{
				this.Expanded(sender, propertyArgs);
			}
		}

		protected virtual void OnPropertyCollapsed(object sender, PositionedPropertyEventArgs propertyArgs)
		{
			if (this.Collapsed != null)
			{
				this.Collapsed(sender, propertyArgs);
			}
		}
		#endregion
	}
}
