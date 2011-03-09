// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.ComponentModel;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// ViewModel for property node in tree of properties, to be bound to View (PositionedGraphNodeControl).
	/// </summary>
	public class ContentPropertyNode : ContentNode, IEvaluate
	{
		PositionedNodeProperty positionedProperty;
		
		public ContentPropertyNode(PositionedGraphNode containingNode, ContentNode parent)
			: base(containingNode, parent)
		{
		}
		
		/// <summary>
		/// The PositionedNodeProperty this node contains.
		/// </summary>
		public PositionedNodeProperty Property
		{
			get { return this.positionedProperty; }
		}
		
		public bool IsEvaluated
		{
			get { return this.positionedProperty.IsEvaluated; }
		}
		
		public override bool IsPropertyExpanded
		{
			get { return this.positionedProperty.IsPropertyExpanded; }
			set { this.positionedProperty.IsPropertyExpanded = value; }
		}
		
		public void Evaluate()
		{
			this.positionedProperty.Evaluate();
			this.Text = this.positionedProperty.Value;
		}
		
		public override bool ShowExpandPropertyButton
		{
			get
			{
				// show expand button for non-null non-atomic objects
				return (!this.positionedProperty.IsAtomic && !this.positionedProperty.IsNull);
			}
		}
		
		public override void InitOverride(AbstractNode source, Expanded expanded)
		{
			if (!(source is PropertyNode))
				throw new InvalidOperationException(string.Format("{0} must initialize from {1}", typeof(ContentPropertyNode).Name, typeof(PropertyNode).Name));
			
			PropertyNode sourcePropertyNode = source as PropertyNode;
			
			this.Name = sourcePropertyNode.Property.Name;
			// Important to set Text here, as we might be just building new view over existing (evaluated) model.
			// If the model is not evaluated yet, this will be string.Empty and filled in Evaluate().
			this.Text = sourcePropertyNode.Property.Value;		
			this.IsNested = false;
			this.IsExpanded = false;			// always false, Property nodes are never expanded (they have IsPropertyExpanded)
			this.positionedProperty = new PositionedNodeProperty(
				sourcePropertyNode.Property, this.ContainingNode,
				expanded.Expressions.IsExpanded(sourcePropertyNode.Property.Expression));
		}
	}
}
