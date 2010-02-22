// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.TreeModel;
using Debugger.AddIn.Visualizers.GridVisualizer;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers
{
	public class GridVisualizerDescriptor : IVisualizerDescriptor
	{
		public bool IsVisualizerAvailable(DebugType type)
		{
			DebugType collectionType, itemType;
			// Visualizer available for IEnumerable<T> (that means also IList<T>)
			return type.ResolveIEnumerableImplementation(out collectionType, out itemType);
		}
		
		public IVisualizerCommand CreateVisualizerCommand(Expression expression)
		{
			return new GridVisualizerCommand(expression);
		}
	}
	
	/// <summary>
	/// Shows grid visualizer for a node.
	/// </summary>
	public class GridVisualizerCommand : ExpressionVisualizerCommand
	{
		public GridVisualizerCommand(Expression expression)
			:base(expression)
		{
		}
		
		public override bool CanExecute { 
			get { return true; }
		}
		
		public override string ToString()
		{
			return "Collection visualizer";
		}
		
		public override void Execute()
		{
			if (this.Expression != null)
			{
				var gridVisualizerWindow = GridVisualizerWindow.EnsureShown();
				gridVisualizerWindow.ShownExpression = this.Expression;
			}
		}
	}
}
