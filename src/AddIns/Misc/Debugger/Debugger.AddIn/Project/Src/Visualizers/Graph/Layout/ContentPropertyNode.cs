// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;

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
		
		public bool IsPropertyExpanded
		{
			get { return this.positionedProperty.IsPropertyExpanded; }
			set { this.positionedProperty.IsPropertyExpanded = value; }
		}
		
		public void Evaluate()
		{
			this.positionedProperty.Evaluate();
			this.Text = this.positionedProperty.Value;
		}
		
		public override void InitFrom(AbstractNode source, Expanded expanded)
		{
			if (!(source is PropertyNode))
				throw new InvalidOperationException("PropertyNodeViewModel must initialize from PropertyNode");
			
			PropertyNode sourcePropertyNode = source as PropertyNode;
			
			this.Name = sourcePropertyNode.Property.Name;
			this.Text = sourcePropertyNode.Property.Value;		// lazy evaluated
			this.IsNested = false;
			this.IsExpanded = false;			// always false, property content nodes are never expanded
			this.positionedProperty = new PositionedNodeProperty(
				sourcePropertyNode.Property, this.ContainingNode, 
				expanded.Expressions.IsExpanded(sourcePropertyNode.Property.Expression));
		}
	}
}
