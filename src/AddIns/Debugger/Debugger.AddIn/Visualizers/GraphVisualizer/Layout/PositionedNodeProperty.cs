// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			if (containingNode == null) throw new ArgumentNullException("containingNode");
			if (objectProperty == null) throw new ArgumentNullException("objectProperty");
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
		public GraphExpression Expression { get { return this.objectGraphProperty.Expression; } }
		
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
