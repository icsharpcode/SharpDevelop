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
	}
}
