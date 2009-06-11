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
	/// <see cref="ObjectProperty"/> with outgoing <see cref="PositionedEdge"/>.
	/// </summary>
	public class PositionedNodeProperty
	{
		/// <summary>
		/// Creates new PositionedNodeProperty.
		/// </summary>
		/// <param name="objectProperty">Underlying <see cref="ObjectProperty"/></param>
		public PositionedNodeProperty(ObjectProperty objectProperty, PositionedNode containingNode)
		{
			this.objectProperty = objectProperty;
			this.containingNode = containingNode;
		}
		
		public bool IsExpanded { get; set; }
		
		private ObjectProperty objectProperty;
		/// <summary>
		/// Underlying <see cref="ObjectProperty"/>.
		/// </summary>
		public ObjectProperty ObjectProperty
		{
			get { return this.objectProperty; }
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
		/// Edge outgoing from this property to another <see cref="PositionedNode"/>.
		/// </summary>
		public PositionedEdge Edge { get; set; }
		
		/// <summary>
		/// e.g. "Age"
		/// </summary>
		public string Name { get { return this.objectProperty.Name; } }
		
		/// <summary>
		/// e.g. "19"
		/// </summary>
		public string Value { get { return this.objectProperty.Value; } }
		
		/// <summary>
		/// Full Debugger expression used to obtain value of this property.
		/// </summary>
		public Debugger.Expressions.Expression Expression { get { return this.objectProperty.Expression; } }
		
		/// <summary>
        /// Is this property of atomic type? (int, string, etc.)
        /// </summary>
		public bool IsAtomic { get { return this.objectProperty.IsAtomic; } }
	}
}
