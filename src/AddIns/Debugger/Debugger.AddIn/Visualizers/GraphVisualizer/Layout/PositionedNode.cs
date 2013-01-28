// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Debugger.AddIn.Visualizers.Graph.Drawing;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// ObjectNode with added position information.
	/// </summary>
	public class PositionedNode : SplineRouting.IRect
	{
		/// <summary>
		/// Creates new PositionedNode.
		/// </summary>
		/// <param name="objectNode">Underlying ObjectNode.</param>
		public PositionedNode(ObjectGraphNode objectNode, Expanded expanded)
		{
			this.ObjectNode = objectNode;
			InitVisualControl();
			InitContentFromObjectNode(expanded);
		}
		
		public event EventHandler<PositionedPropertyEventArgs> PropertyExpanded;
		public event EventHandler<PositionedPropertyEventArgs> PropertyCollapsed;
		public event EventHandler<ContentNodeEventArgs> ContentNodeExpanded;
		public event EventHandler<ContentNodeEventArgs> ContentNodeCollapsed;
		
		/// <summary>
		/// Underlying ObjectNode.
		/// </summary>
		public ObjectGraphNode ObjectNode { get; private set; }
		
		/// <summary>
		/// Tree-of-properties content of this node.
		/// </summary>
		public ContentNode Content { get; set; }
		
		/// <summary>
		/// Name of the Type in the debuggee.
		/// </summary>
		public string TypeName { get { return this.ObjectNode.TypeName; } }
		
		/// <summary>
		/// The size of the subtree of this node in the layout.
		/// </summary>
		public double SubtreeSize { get; set; }
		
		/// <summary>
		/// Visual control to be shown for this node.
		/// </summary>
		public PositionedGraphNodeControl NodeVisualControl { get; private set; }
		
		public void ReleaseNodeVisualControl()
		{
			this.NodeVisualControl.SetDataContext(null);
			this.NodeVisualControl = null;
		}
		
		void InitContentFromObjectNode(Expanded expanded)
		{
			this.Content = new ContentNode(this, null);
			this.Content.InitOverride(this.ObjectNode.Content, expanded);
			this.NodeVisualControl.SetDataContext(this);
		}
		
		void InitVisualControl()
		{
			this.NodeVisualControl = new PositionedGraphNodeControl();
			// propagate events from nodeVisualControl
			this.NodeVisualControl.PropertyExpanded += new EventHandler<PositionedPropertyEventArgs>(NodeVisualControl_PropertyExpanded);
			this.NodeVisualControl.PropertyCollapsed += new EventHandler<PositionedPropertyEventArgs>(NodeVisualControl_PropertyCollapsed);
			this.NodeVisualControl.ContentNodeExpanded += new EventHandler<ContentNodeEventArgs>(NodeVisualControl_ContentNodeExpanded);
			this.NodeVisualControl.ContentNodeCollapsed += new EventHandler<ContentNodeEventArgs>(NodeVisualControl_ContentNodeCollapsed);
		}
		
		public IEnumerable<PositionedNodeProperty> Properties
		{
			get	{ return this.Content.FlattenProperties();	}
		}
		
		public virtual IEnumerable<PositionedEdge> Edges
		{
			get	{
				foreach	(PositionedNodeProperty property in this.Properties.Where(prop => prop.Edge != null)) {
					yield return property.Edge;
				}
			}
		}
		
		public double Left { get; set; }
		public double Top { get; set; }
		public double Width
		{
			get { return NodeVisualControl.Width; }
		}
		public double Height
		{
			get { return NodeVisualControl.Height; }
		}
		
		public Point LeftTop
		{
			get { return new Point(Left, Top); }
		}
		
		public Point Center
		{
			get { return new Point(Left + Width / 2, Top + Height / 2); }
		}
		
		public Rect Rect { get { return new Rect(Left, Top, Width, Height); } }
		
		#region event helpers
		private void NodeVisualControl_PropertyExpanded(object sender, PositionedPropertyEventArgs e)
		{
			if (this.PropertyExpanded != null)
				this.PropertyExpanded(sender, e);
		}
		
		private void NodeVisualControl_PropertyCollapsed(object sender, PositionedPropertyEventArgs e)
		{
			if (this.PropertyCollapsed != null)
				this.PropertyCollapsed(sender, e);
		}
		
		private void NodeVisualControl_ContentNodeExpanded(object sender, ContentNodeEventArgs e)
		{
			if (this.ContentNodeExpanded != null)
				this.ContentNodeExpanded(sender, e);
		}
		
		private void NodeVisualControl_ContentNodeCollapsed(object sender, ContentNodeEventArgs e)
		{
			if (this.ContentNodeCollapsed != null)
				this.ContentNodeCollapsed(sender, e);
		}
		#endregion
		
		SplineRouting.Box box;
		public SplineRouting.IRect Inflated(double padding)
		{
			if (box == null)
				box = new SplineRouting.Box(this);
			return box.Inflated(padding);
		}
		
		public void MeasureVisualControl()
		{
			if ((this.NodeVisualControl != null)) {
				this.NodeVisualControl.CalculateWidthHeight();
			}
		}
	}
}
