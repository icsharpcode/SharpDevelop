// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using ICSharpCode.NRefactory.Ast;
using System;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// <see cref="ObjectProperty"/> with outgoing <see cref="PositionedEdge"/>.
	/// Implements evaluation of value on demand using IEvaluate.
	/// </summary>
	public class PositionedNodeProperty : IEvaluate
	{
		/// <summary>
		/// Creates new PositionedNodeProperty.
		/// </summary>
		/// <param name="objectProperty">Underlying <see cref="ObjectProperty"/></param>
		public PositionedNodeProperty(ObjectGraphProperty objectProperty, PositionedNode containingNode, bool isPropertyExpanded)
		{
			if (containingNode == null)
				throw new ArgumentNullException("containingNode");
			if (objectProperty == null)
				throw new ArgumentNullException("objectProperty");
			
			this.objectGraphProperty = objectProperty;
			this.containingNode = containingNode;
			this.IsPropertyExpanded = isPropertyExpanded;
		}
		
		/// <summary>
		/// Is this property expanded?
		/// </summary>
		public bool IsPropertyExpanded { get; set; }
		
		/// <summary>
		/// Edge outgoing from this property to another <see cref="PositionedNode"/>.
		/// </summary>
		public PositionedEdge Edge { get; set; }
		
		
		private ObjectGraphProperty objectGraphProperty;
		/// <summary>
		/// Underlying <see cref="ObjectProperty"/>.
		/// </summary>
		public ObjectGraphProperty ObjectGraphProperty
		{
			get { return this.objectGraphProperty; }
		}
		
		private PositionedNode containingNode;
		/// <summary>
		/// <see cref="PositionedNode"/> which contains this Property.
		/// </summary>
		public PositionedNode ContainingNode
		{
			get { return this.containingNode; }
		}
		
		/// <summary>
		/// e.g. "Age"
		/// </summary>
		public string Name { get { return this.objectGraphProperty.Name; } }
		
		/// <summary>
		/// e.g. "19"
		/// </summary>
		public string Value { get { return this.objectGraphProperty.Value; } }
		
		/// <summary>
		/// Full Debugger expression used to obtain value of this property.
		/// </summary>
		public Expression Expression { get { return this.objectGraphProperty.Expression; } }
		
		/// <summary>
        /// Is this property of atomic type? (int, string, etc.)
        /// </summary>
		public bool IsAtomic { get { return this.objectGraphProperty.IsAtomic; } }
		
		/// <summary>
        /// Is the value of this property null?
        /// </summary>
		public bool IsNull { get { return this.objectGraphProperty.IsNull; } }
		
		public bool IsEvaluated
		{
			get { return this.objectGraphProperty.IsEvaluated; }
		}
		
		public void Evaluate()
		{
			this.objectGraphProperty.Evaluate();
		}
	}
}
