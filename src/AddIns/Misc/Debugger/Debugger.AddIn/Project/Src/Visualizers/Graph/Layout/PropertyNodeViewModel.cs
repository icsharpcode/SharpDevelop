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
	public class PropertyNodeViewModel : NestedNodeViewModel, IEvaluate
	{
		PositionedNodeProperty positionedProperty;
		
		public PropertyNodeViewModel(PositionedNodeProperty positionedNodeProperty)
		{
			if (positionedNodeProperty == null)
				throw new ArgumentNullException("positionedNodeProperty");
			
			this.positionedProperty = positionedNodeProperty;	
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
		
		public void Evaluate()
		{
			this.positionedProperty.Evaluate();
		}
	}
}
